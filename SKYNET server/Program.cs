using Microsoft.EntityFrameworkCore;
using SKYNET_server.Json;
using SKYNET_server.Models;
using SKYNET_server.Persistence;
using SKYNET_server.Services;

// One-shot split migration: app.db/api-state.json/older feature DBs are copied
// into steam.db and dota.db, then archived so only the split stores remain live.
if (args.Contains("--migrate-db-split"))
{
    var migrationConfiguration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Development.json", optional: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();
    var dataDir = DatabaseSplitMigrator.ResolveDataRoot(Directory.GetCurrentDirectory(), migrationConfiguration);
    DatabaseSplitMigrator.Migrate(dataDir, args.Contains("--force"), Console.WriteLine);
    Environment.ExitCode = 0;
    return;
}

// Self-check that a representative state survives a Save -> Load round-trip.
if (args.Contains("--verify-persistence"))
{
    Environment.ExitCode = PersistenceRoundTripCheck.Run(Console.WriteLine) ? 0 : 1;
    return;
}

// Self-check that TypeScript GC scripts load and answer representative Dota
// messages through the production script plugin and protobuf codec.
if (args.Contains("--verify-gc-ts"))
{
    Environment.ExitCode = GcScriptSelfCheck.Run(Console.WriteLine) ? 0 : 1;
    return;
}

// Self-check that Dota's item importer keeps equipable hero/global cosmetics
// while dropping tools, bundles, treasures, gems, and other non-loadout entries.
if (args.Contains("--verify-dota-cosmetics"))
{
    Environment.ExitCode = SteamApiStateService.RunDotaCosmeticParserSelfCheck(Console.WriteLine) ? 0 : 1;
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddJsonOptions(options => SkynetJsonSerializerOptions.AddCompatibilityConverters(options.JsonSerializerOptions));
builder.Services.ConfigureHttpJsonOptions(options =>
    SkynetJsonSerializerOptions.AddCompatibilityConverters(options.SerializerOptions));
builder.Services.AddSingleton<GameCoordinatorTraceService>();
builder.Services.AddSingleton<GameServerSettingsService>();
builder.Services.AddSingleton<GameCatalogService>();
builder.Services.AddSingleton<DotaDedicatedServerSupervisor>();
builder.Services.AddSingleton<DotaDB>();
builder.Services.AddSingleton<DedicatedServerService>();
builder.Services.AddSingleton<GameCoordinatorScriptPlugin>();
builder.Services.AddSingleton<IGameCoordinatorPlugin>(sp => sp.GetRequiredService<GameCoordinatorScriptPlugin>());
builder.Services.AddSingleton<GameCoordinatorPluginRegistry>();
builder.Services.AddSingleton<SdrCertificateService>();
var dataRoot = DatabaseSplitMigrator.ResolveDataRoot(builder.Environment.ContentRootPath, builder.Configuration);
builder.Services.AddPooledDbContextFactory<SteamDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(dataRoot, "steam.db")}"));
builder.Services.AddPooledDbContextFactory<DotaDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(dataRoot, "dota.db")}"));
builder.Services.AddSingleton<SteamApiStateService>();
builder.Services.AddHostedService<DiscoveryService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DotaDedicatedServerSupervisor>());
builder.Services.AddHostedService<GameCoordinatorTickService>();
builder.Services.AddHostedService<PresenceSweepService>();
builder.Services.AddHostedService<SKYNET_server.Services.Networking.SdrRelayService>();
builder.Services.AddHostedService<DatabaseBackupService>();

var app = builder.Build();

// Prepare split SQLite stores before any facade touches them. app.db and older
// per-feature DBs are migration inputs only and are archived on successful copy.
{
    Directory.CreateDirectory(dataRoot);
    DatabaseSplitMigrator.Migrate(dataRoot, force: false, m => app.Logger.LogInformation("{MigrationMessage}", m));

    using var steam = app.Services.GetRequiredService<IDbContextFactory<SteamDbContext>>().CreateDbContext();
    using var dota = app.Services.GetRequiredService<IDbContextFactory<DotaDbContext>>().CreateDbContext();
    steam.Database.EnsureCreated();
    dota.Database.EnsureCreated();
    DatabaseSchemaMaintenance.EnsureCurrent(steam, dota);
    steam.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
    dota.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

_ = app.Services.GetRequiredService<DotaDB>();
_ = app.Services.GetRequiredService<DedicatedServerService>();

app.Lifetime.ApplicationStopping.Register(() =>
{
    // Flush any pending in-memory changes, then checkpoint so a later force-kill
    // never leaves a large -wal sidecar behind.
    try { app.Services.GetRequiredService<SteamApiStateService>().FlushStateToDatabase(); }
    catch { /* best effort */ }

    try
    {
        using var steam = app.Services.GetRequiredService<IDbContextFactory<SteamDbContext>>().CreateDbContext();
        using var dota = app.Services.GetRequiredService<IDbContextFactory<DotaDbContext>>().CreateDbContext();
        steam.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint(TRUNCATE);");
        dota.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint(TRUNCATE);");
    }
    catch
    {
        // Best effort: a failed checkpoint is recovered automatically on next open.
    }
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Assets are referenced with asp-append-version="true", which appends a
        // content hash (?v=...) to every URL. A rebuilt file gets a new URL, so we
        // can cache the current URL aggressively without risking stale assets.
        var headers = ctx.Context.Response.GetTypedHeaders();
        if (ctx.Context.Request.Query.ContainsKey("v"))
        {
            headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(365),
                Extensions = { new Microsoft.Net.Http.Headers.NameValueHeaderValue("immutable") }
            };
        }
        else
        {
            // Un-versioned requests (e.g. direct hits): cache briefly but revalidate.
            headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromHours(1),
                MustRevalidate = true
            };
        }
    }
});
app.UseRouting();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapPost("/auth/steam/session", (HttpRequest request, ApiSessionRequest payload, SteamApiStateService state) =>
{
    var session = state.StartSession(payload, SteamApiStateService.GetClientIp(request));
    return session == null ? Results.Unauthorized() : Results.Ok(session);
});

api.MapGet("/users/me", (HttpRequest request, SteamApiStateService state) =>
{
    var user = state.GetSelf(SteamApiStateService.GetBearerToken(request) ?? string.Empty);
    return user == null ? Results.Unauthorized() : Results.Ok(user);
});

api.MapPost("/presence/offline", (HttpRequest request, SteamApiStateService state) =>
{
    return state.MarkSelfOffline(SteamApiStateService.GetBearerToken(request) ?? string.Empty)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/users/{steamId}", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    var user = state.GetUser(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId);
    return user == null ? Results.NotFound() : Results.Ok(user);
});

api.MapGet("/friends", (HttpRequest request, SteamApiStateService state) =>
{
    var friends = state.GetFriends(SteamApiStateService.GetBearerToken(request) ?? string.Empty);
    return friends == null ? Results.Unauthorized() : Results.Ok(friends);
});

api.MapGet("/users", (HttpRequest request, SteamApiStateService state) =>
{
    var users = state.GetWebUsersWithRelationships(SteamApiStateService.GetBearerToken(request) ?? string.Empty);
    return users.Count == 0 ? Results.Ok(users) : Results.Ok(users);
});

api.MapGet("/friends/requests/incoming", (HttpRequest request, SteamApiStateService state) =>
{
    return Results.Ok(state.GetIncomingFriendRequests(SteamApiStateService.GetBearerToken(request) ?? string.Empty));
});

api.MapGet("/friends/requests/outgoing", (HttpRequest request, SteamApiStateService state) =>
{
    return Results.Ok(state.GetOutgoingFriendRequests(SteamApiStateService.GetBearerToken(request) ?? string.Empty));
});

api.MapPost("/friends/request", (HttpRequest request, ApiFriendActionRequest payload, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    var ok = payload.SteamId != 0
        ? state.SendFriendRequest(token, payload.SteamId)
        : state.SendFriendRequest(token, payload.Identifier);
    return ok ? Results.Ok() : Results.BadRequest();
});

api.MapPost("/friends/{steamId}/accept", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    return state.AcceptFriendRequestFrom(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/friends/requests/{requestId}/accept", (HttpRequest request, string requestId, SteamApiStateService state) =>
{
    return state.AcceptFriendRequest(SteamApiStateService.GetBearerToken(request) ?? string.Empty, requestId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/friends/requests/{requestId}/decline", (HttpRequest request, string requestId, SteamApiStateService state) =>
{
    return state.DeclineFriendRequest(SteamApiStateService.GetBearerToken(request) ?? string.Empty, requestId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/friends/requests/{requestId}/cancel", (HttpRequest request, string requestId, SteamApiStateService state) =>
{
    return state.CancelFriendRequest(SteamApiStateService.GetBearerToken(request) ?? string.Empty, requestId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/friends/{steamId}/remove", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    return state.RemoveFriendOrRequest(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapMethods("/users/me/persona", new[] { "PATCH" }, (HttpRequest request, ApiPersonaUpdate update, SteamApiStateService state) =>
{
    var user = state.UpdatePersona(SteamApiStateService.GetBearerToken(request) ?? string.Empty, update.PersonaName);
    return user == null ? Results.Unauthorized() : Results.Ok(user);
});

api.MapPut("/presence", (HttpRequest request, ApiPresenceUpdate update, SteamApiStateService state) =>
{
    return state.SetPresence(SteamApiStateService.GetBearerToken(request) ?? string.Empty, update.Key, update.Value)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/users/{steamId}/avatar", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    var avatar = state.GetAvatar(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId);
    request.HttpContext.Response.Headers.CacheControl = "no-cache, must-revalidate";
    request.HttpContext.Response.Headers.ETag = $"\"{avatar.ETag}\"";
    request.HttpContext.Response.Headers["X-SKYNET-Avatar-SteamId"] = avatar.SteamId.ToString();
    request.HttpContext.Response.Headers["X-SKYNET-Avatar-Default"] = avatar.IsDefault ? "true" : "false";
    return Results.File(avatar.Content, "image/png");
});

api.MapPut("/users/me/avatar", (HttpRequest request, ApiAvatarUpdate payload, SteamApiStateService state) =>
{
    return state.PutSelfAvatar(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

app.MapGet("/Images/AvatarCache/{accountId}.jpg", (HttpRequest request, uint accountId, SteamApiStateService state) =>
{
    var avatar = state.GetAvatarByAccountId(accountId);
    request.HttpContext.Response.Headers.CacheControl = "no-cache, must-revalidate";
    request.HttpContext.Response.Headers.ETag = $"\"{avatar.ETag}\"";
    request.HttpContext.Response.Headers["X-SKYNET-Avatar-SteamId"] = avatar.SteamId.ToString();
    request.HttpContext.Response.Headers["X-SKYNET-Avatar-Default"] = avatar.IsDefault ? "true" : "false";
    return Results.File(avatar.Content, "image/png");
});

api.MapGet("/stats/me", (HttpRequest request, SteamApiStateService state) =>
{
    var stats = state.GetStats(SteamApiStateService.GetBearerToken(request) ?? string.Empty, 0, true);
    return stats == null ? Results.Unauthorized() : Results.Ok(stats);
});

api.MapGet("/stats/users/{steamId}", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    var stats = state.GetStats(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId, false);
    return stats == null ? Results.NotFound() : Results.Ok(stats);
});

api.MapPut("/stats/me", (HttpRequest request, ApiStoreStatsRequest payload, SteamApiStateService state) =>
{
    return state.StoreStats(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/events", (HttpRequest request, string? since, int? waitMs, SteamApiStateService state) =>
{
    return Results.Ok(state.PollEvents(SteamApiStateService.GetBearerToken(request) ?? string.Empty, since, waitMs ?? 0));
});

api.MapPost("/lobbies/query", (HttpRequest request, ApiLobbyQueryRequest payload, SteamApiStateService state) =>
{
    var result = state.QueryLobbies(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

api.MapPost("/lobbies", (HttpRequest request, ApiCreateLobbyRequest payload, SteamApiStateService state) =>
{
    var lobby = state.CreateLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload);
    return lobby == null ? Results.BadRequest() : Results.Ok(lobby);
});

api.MapGet("/lobbies/{lobbyId}", (HttpRequest request, ulong lobbyId, SteamApiStateService state) =>
{
    var lobby = state.GetLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId);
    return lobby == null ? Results.NotFound() : Results.Ok(lobby);
});

api.MapPost("/lobbies/{lobbyId}/join", (HttpRequest request, ulong lobbyId, SteamApiStateService state) =>
{
    var lobby = state.JoinLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId);
    return lobby == null ? Results.BadRequest() : Results.Ok(lobby);
});

api.MapGet("/apps/{appId}", (uint appId, GameCatalogService catalog) =>
    Results.Ok(catalog.Get(appId)));

api.MapPost("/lobbies/{lobbyId}/invites", (HttpRequest request, ulong lobbyId, ApiLobbyInviteRequest payload, SteamApiStateService state) =>
{
    return state.InviteUserToLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.InviteeSteamId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/game-invites", (HttpRequest request, ApiGameInviteRequest payload, SteamApiStateService state) =>
{
    return state.InviteUserToGame(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload.InviteeSteamId, payload.ConnectString)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/lobbies/{lobbyId}/leave", (HttpRequest request, ulong lobbyId, SteamApiStateService state) =>
{
    return state.LeaveLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/data", (HttpRequest request, ulong lobbyId, ApiLobbyDataUpdateRequest payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key, payload.Value)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/lobbies/{lobbyId}/data/delete", (HttpRequest request, ulong lobbyId, ApiLobbyDeleteDataRequest payload, SteamApiStateService state) =>
{
    return state.DeleteLobbyData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/settings", (HttpRequest request, ulong lobbyId, ApiLobbySettingsUpdateRequest payload, SteamApiStateService state) =>
{
    return state.UpdateLobbySettings(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/member-data", (HttpRequest request, ulong lobbyId, ApiLobbyDataUpdateRequest payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyMemberData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key, payload.Value)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/gameserver", (HttpRequest request, ulong lobbyId, ApiLobbyGameServerUpdateRequest payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyGameServer(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/lobbies/{lobbyId}/chat", (HttpRequest request, ulong lobbyId, ApiLobbyChatRequest payload, SteamApiStateService state) =>
{
    var body = string.IsNullOrEmpty(payload.MessageBase64)
        ? Array.Empty<byte>()
        : Convert.FromBase64String(payload.MessageBase64);
    return state.SendLobbyChatMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, body)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/storage/files", (HttpRequest request, ApiRemoteStorageUploadRequest payload, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    return state.PutFile(token, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/storage/files", (HttpRequest request, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    var files = state.ListFiles(token);
    return files == null ? Results.Unauthorized() : Results.Ok(files);
});

api.MapGet("/storage/files/{*fileName}", (HttpRequest request, string fileName, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    if (!state.IsValidToken(token))
    {
        return Results.Unauthorized();
    }
    var file = state.GetFile(token, fileName);
    return file == null ? Results.NotFound() : Results.Ok(file);
});

api.MapPost("/storage/files/delete", (HttpRequest request, ApiRemoteStorageDeleteRequest payload, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    if (!state.IsValidToken(token))
    {
        return Results.Unauthorized();
    }
    return state.DeleteFile(token, payload.FileName)
        ? Results.Ok()
        : Results.NotFound();
});

api.MapPost("/storage/files/share", (HttpRequest request, ApiRemoteStorageDeleteRequest payload, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    if (!state.IsValidToken(token))
    {
        return Results.Unauthorized();
    }
    var result = state.ShareFile(token, payload.FileName);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

api.MapGet("/storage/quota", (HttpRequest request, SteamApiStateService state) =>
{
    var token = SteamApiStateService.GetBearerToken(request) ?? string.Empty;
    var quota = state.GetQuota(token);
    return quota == null ? Results.Unauthorized() : Results.Ok(quota);
});

api.MapPost("/auth/tickets/session", (ApiAuthTicketRequest payload, SteamApiStateService state) =>
{
    return Results.Ok(state.CreateTicket(payload));
});

api.MapPost("/auth/tickets/validate", (ApiAuthValidateRequest payload, SteamApiStateService state) =>
{
    return Results.Ok(state.ValidateTicket(payload));
});

api.MapPost("/gameservers/users/connect", (ApiConnectAuthRequest payload, SteamApiStateService state) =>
{
    return Results.Ok(state.ConnectAndAuthenticate(payload));
});

// Steam Datagram (SDR) networking certificate authority.
api.MapGet("/networking/sdr/ca", (SdrCertificateService sdr) => Results.Ok(new ApiSdrCa
{
    CaPublicKeyBase64 = Convert.ToBase64String(sdr.CaPublicKey),
    CaKeyId = sdr.CaKeyId
}));

api.MapPost("/networking/sdr/cert", (HttpRequest request, ApiSdrCertRequest payload, SteamApiStateService state, SdrCertificateService sdr) =>
{
    if (!state.TryResolveSessionIdentity(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload.SteamId, payload.AppId, out var steamId, out var appId))
    {
        return Results.Unauthorized();
    }

    var result = sdr.IssueCertificate(steamId, appId);
    return Results.Ok(new ApiSdrCert
    {
        CertBase64 = Convert.ToBase64String(result.Certificate),
        SignatureBase64 = Convert.ToBase64String(result.Signature),
        PrivateKeyBase64 = Convert.ToBase64String(result.PrivateKey),
        PublicKeyBase64 = Convert.ToBase64String(result.PublicKey),
        CaKeyId = result.CaKeyId
    });
});

api.MapPost("/auth/tickets/end-session", (ApiAuthEndSessionRequest payload, SteamApiStateService state) =>
{
    state.EndAuthSession(payload);
    return Results.Ok();
});

api.MapPost("/auth/tickets/cancel", (ApiCancelAuthTicketRequest payload, SteamApiStateService state) =>
{
    state.CancelTicket(payload);
    return Results.Ok();
});

api.MapPost("/gameservers/register", (ApiGameServerState payload, SteamApiStateService state) => Results.Ok(state.RegisterGameServer(payload)));
api.MapPost("/gameservers/logon", (ApiGameServerState payload, SteamApiStateService state) => Results.Ok(state.LogOnGameServer(payload)));
api.MapPost("/gameservers/logoff", (SteamApiStateService state) => { state.LogOffGameServer(); return Results.Ok(); });
api.MapPut("/gameservers/state", (ApiGameServer payload, SteamApiStateService state) => state.UpdateGameServerState(payload) ? Results.Ok() : Results.BadRequest());
api.MapPost("/gameservers/heartbeat", (ApiGameServer payload, SteamApiStateService state) => state.HeartbeatGameServer(payload) ? Results.Ok() : Results.BadRequest());
api.MapGet("/gameservers/public-ip", (SteamApiStateService state) => Results.Ok(state.GetPublicIp()));
api.MapPost("/gameservers/users/disconnect", (ApiDisconnectGameServerUser payload, SteamApiStateService state) => state.DisconnectGameServerUser(payload.SteamId) ? Results.Ok() : Results.BadRequest());
api.MapPut("/gameservers/users/data", (ApiGameServerUserData payload, SteamApiStateService state) => state.UpdateGameServerUserData(payload) ? Results.Ok() : Results.BadRequest());
api.MapGet("/gameservers/stats/users/{steamId}", (ulong steamId, SteamApiStateService state) => Results.Ok(state.GetGameServerUserStats(steamId)));
api.MapPut("/gameservers/stats/users/{steamId}", (ulong steamId, ApiStoreStatsRequest payload, SteamApiStateService state) => state.StoreGameServerUserStats(steamId, payload) ? Results.Ok() : Results.BadRequest());

api.MapGet("/dota/cosmetics", (HttpRequest request, string? search, uint? heroId, int? take, SteamApiStateService state) =>
{
    var overview = state.GetDotaCosmeticsOverview(SteamApiStateService.GetBearerToken(request) ?? string.Empty, search, heroId, take ?? 300);
    return overview == null ? Results.Unauthorized() : Results.Ok(overview);
});

api.MapPost("/dota/cosmetics/import", (HttpRequest request, ApiDotaImportRequest payload, SteamApiStateService state) =>
{
    var result = state.ImportDotaCosmetics(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

api.MapPost("/dota/equipment", (HttpRequest request, ApiDotaEquipItemRequest payload, SteamApiStateService state) =>
{
    return state.EquipDotaItemFromAdmin(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/dota/equipment/{steamId}/clear", (HttpRequest request, ulong steamId, SteamApiStateService state) =>
{
    return state.ClearDotaEquipmentFromAdmin(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/network/p2p/send", (HttpRequest request, ApiP2PPacketSend payload, SteamApiStateService state) =>
{
    return state.SendP2P(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/network/p2p/send-batch", (HttpRequest request, ApiP2PPacketBatch payload, SteamApiStateService state) =>
{
    return state.SendP2PBatch(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/gamecoordinator/messages", (HttpRequest request, ApiGCMessage payload, SteamApiStateService state) =>
{
    return state.SendGCMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/gamecoordinator/exchange", (HttpRequest request, ApiGCExchangeRequest payload, SteamApiStateService state) =>
{
    var response = state.ExchangeGCMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload, SteamApiStateService.GetClientIp(request));
    return response == null ? Results.Unauthorized() : Results.Ok(response);
});

api.MapPost("/gamecoordinator/poll", (HttpRequest request, ApiGCPollRequest payload, SteamApiStateService state) =>
{
    var response = state.PollGCMessages(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload, SteamApiStateService.GetClientIp(request));
    return response == null ? Results.Unauthorized() : Results.Ok(response);
});

app.MapRazorPages();

// Flush state on graceful shutdown so a normal stop never loses recent changes.
app.Lifetime.ApplicationStopping.Register(() =>
{
    try { app.Services.GetRequiredService<SteamApiStateService>().FlushState(); }
    catch { /* best effort on shutdown */ }
});

app.Run();
