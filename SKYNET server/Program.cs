using SKYNET_server.Models;
using SKYNET_server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<GameCoordinatorTraceService>();
builder.Services.AddSingleton<LuaGameCoordinatorPlugin>();
builder.Services.AddSingleton<IGameCoordinatorPlugin>(sp => sp.GetRequiredService<LuaGameCoordinatorPlugin>());
builder.Services.AddSingleton<GameCoordinatorPluginRegistry>();
builder.Services.AddSingleton<SdrCertificateService>();
builder.Services.AddSingleton<SteamApiStateService>();
builder.Services.AddHostedService<SkyNetDiscoveryService>();
builder.Services.AddHostedService<GameCoordinatorTickService>();
builder.Services.AddHostedService<PresenceSweepService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapPost("/auth/steam/session", (HttpRequest request, SkyNetSessionRequestDto payload, SteamApiStateService state) =>
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

api.MapPost("/friends/request", (HttpRequest request, SkyNetFriendActionRequestDto payload, SteamApiStateService state) =>
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

api.MapMethods("/users/me/persona", new[] { "PATCH" }, (HttpRequest request, SkyNetPersonaUpdateDto update, SteamApiStateService state) =>
{
    var user = state.UpdatePersona(SteamApiStateService.GetBearerToken(request) ?? string.Empty, update.PersonaName);
    return user == null ? Results.Unauthorized() : Results.Ok(user);
});

api.MapPut("/presence", (HttpRequest request, SkyNetPresenceUpdateDto update, SteamApiStateService state) =>
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

api.MapPut("/users/me/avatar", (HttpRequest request, SkyNetAvatarUpdateDto payload, SteamApiStateService state) =>
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

api.MapPut("/stats/me", (HttpRequest request, SkyNetStoreStatsRequestDto payload, SteamApiStateService state) =>
{
    return state.StoreStats(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/events", (HttpRequest request, string? since, int? waitMs, SteamApiStateService state) =>
{
    return Results.Ok(state.PollEvents(SteamApiStateService.GetBearerToken(request) ?? string.Empty, since, waitMs ?? 0));
});

api.MapPost("/lobbies/query", (HttpRequest request, SkyNetLobbyQueryRequestDto payload, SteamApiStateService state) =>
{
    var result = state.QueryLobbies(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload);
    return result == null ? Results.Unauthorized() : Results.Ok(result);
});

api.MapPost("/lobbies", (HttpRequest request, SkyNetCreateLobbyRequestDto payload, SteamApiStateService state) =>
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

api.MapPost("/lobbies/{lobbyId}/leave", (HttpRequest request, ulong lobbyId, SteamApiStateService state) =>
{
    return state.LeaveLobby(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/data", (HttpRequest request, ulong lobbyId, SkyNetLobbyDataUpdateRequestDto payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key, payload.Value)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPost("/lobbies/{lobbyId}/data/delete", (HttpRequest request, ulong lobbyId, SkyNetLobbyDeleteDataRequestDto payload, SteamApiStateService state) =>
{
    return state.DeleteLobbyData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/settings", (HttpRequest request, ulong lobbyId, SkyNetLobbySettingsUpdateRequestDto payload, SteamApiStateService state) =>
{
    return state.UpdateLobbySettings(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/member-data", (HttpRequest request, ulong lobbyId, SkyNetLobbyDataUpdateRequestDto payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyMemberData(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload.Key, payload.Value)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/lobbies/{lobbyId}/gameserver", (HttpRequest request, ulong lobbyId, SkyNetLobbyGameServerUpdateRequestDto payload, SteamApiStateService state) =>
{
    return state.UpdateLobbyGameServer(SteamApiStateService.GetBearerToken(request) ?? string.Empty, lobbyId, payload)
        ? Results.Ok()
        : Results.BadRequest();
});

api.MapPut("/storage/files", (HttpRequest request, SkyNetRemoteStorageFileDto payload, SteamApiStateService state) =>
{
    return state.PutFile(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapGet("/storage/files", (HttpRequest request, SteamApiStateService state) =>
{
    var files = state.ListFiles(SteamApiStateService.GetBearerToken(request) ?? string.Empty);
    return files == null ? Results.Unauthorized() : Results.Ok(files);
});

api.MapGet("/storage/files/{*fileName}", (HttpRequest request, string fileName, SteamApiStateService state) =>
{
    var file = state.GetFile(SteamApiStateService.GetBearerToken(request) ?? string.Empty, fileName);
    return file == null ? Results.NotFound() : Results.Ok(file);
});

api.MapPost("/storage/files/delete", (HttpRequest request, SkyNetRemoteStorageDeleteRequestDto payload, SteamApiStateService state) =>
{
    return state.DeleteFile(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload.FileName)
        ? Results.Ok()
        : Results.NotFound();
});

api.MapPost("/storage/files/share", (HttpRequest request, SkyNetRemoteStorageDeleteRequestDto payload, SteamApiStateService state) =>
{
    var result = state.ShareFile(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload.FileName);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

api.MapGet("/storage/quota", (HttpRequest request, SteamApiStateService state) =>
{
    return Results.Ok(state.GetQuota(SteamApiStateService.GetBearerToken(request) ?? string.Empty));
});

api.MapPost("/auth/tickets/session", (SkyNetAuthTicketRequestDto payload, SteamApiStateService state) =>
{
    return Results.Ok(state.CreateTicket(payload));
});

api.MapPost("/auth/tickets/validate", (SkyNetAuthValidateRequestDto payload, SteamApiStateService state) =>
{
    return Results.Ok(state.ValidateTicket(payload));
});

api.MapPost("/gameservers/users/connect", (SkyNetConnectAuthRequestDto payload, SteamApiStateService state) =>
{
    return Results.Ok(state.ConnectAndAuthenticate(payload));
});

// Steam Datagram (SDR) networking certificate authority.
api.MapGet("/networking/sdr/ca", (SdrCertificateService sdr) => Results.Ok(new SkyNetSdrCaDto
{
    CaPublicKeyBase64 = Convert.ToBase64String(sdr.CaPublicKey),
    CaKeyId = sdr.CaKeyId
}));

api.MapPost("/networking/sdr/cert", (HttpRequest request, SkyNetSdrCertRequestDto payload, SteamApiStateService state, SdrCertificateService sdr) =>
{
    if (!state.TryResolveSessionIdentity(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload.SteamId, payload.AppId, out var steamId, out var appId))
    {
        return Results.Unauthorized();
    }

    var result = sdr.IssueCertificate(steamId, appId);
    return Results.Ok(new SkyNetSdrCertDto
    {
        CertBase64 = Convert.ToBase64String(result.Certificate),
        SignatureBase64 = Convert.ToBase64String(result.Signature),
        PrivateKeyBase64 = Convert.ToBase64String(result.PrivateKey),
        PublicKeyBase64 = Convert.ToBase64String(result.PublicKey),
        CaKeyId = result.CaKeyId
    });
});

api.MapPost("/auth/tickets/end-session", (SkyNetAuthEndSessionRequestDto payload, SteamApiStateService state) =>
{
    state.EndAuthSession(payload);
    return Results.Ok();
});

api.MapPost("/auth/tickets/cancel", (SkyNetCancelAuthTicketRequestDto payload, SteamApiStateService state) =>
{
    state.CancelTicket(payload);
    return Results.Ok();
});

api.MapPost("/gameservers/register", (SkyNetGameServerStateDto payload, SteamApiStateService state) => Results.Ok(state.RegisterGameServer(payload)));
api.MapPost("/gameservers/logon", (SkyNetGameServerStateDto payload, SteamApiStateService state) => Results.Ok(state.LogOnGameServer(payload)));
api.MapPost("/gameservers/logoff", (SteamApiStateService state) => { state.LogOffGameServer(); return Results.Ok(); });
api.MapPut("/gameservers/state", (SkyNetGameServerDto payload, SteamApiStateService state) => state.UpdateGameServerState(payload) ? Results.Ok() : Results.BadRequest());
api.MapPost("/gameservers/heartbeat", (SkyNetGameServerDto payload, SteamApiStateService state) => state.HeartbeatGameServer(payload) ? Results.Ok() : Results.BadRequest());
api.MapGet("/gameservers/public-ip", (SteamApiStateService state) => Results.Ok(state.GetPublicIp()));
api.MapPost("/gameservers/users/disconnect", (SkyNetDisconnectGameServerUserDto payload, SteamApiStateService state) => state.DisconnectGameServerUser(payload.SteamId) ? Results.Ok() : Results.BadRequest());
api.MapPut("/gameservers/users/data", (SkyNetGameServerUserDataDto payload, SteamApiStateService state) => state.UpdateGameServerUserData(payload) ? Results.Ok() : Results.BadRequest());
api.MapGet("/gameservers/stats/users/{steamId}", (ulong steamId, SteamApiStateService state) => Results.Ok(state.GetGameServerUserStats(steamId)));
api.MapPut("/gameservers/stats/users/{steamId}", (ulong steamId, SkyNetStoreStatsRequestDto payload, SteamApiStateService state) => state.StoreGameServerUserStats(steamId, payload) ? Results.Ok() : Results.BadRequest());

api.MapGet("/dota/cosmetics", (HttpRequest request, string? search, uint? heroId, int? take, SteamApiStateService state) =>
{
    var overview = state.GetDotaCosmeticsOverview(SteamApiStateService.GetBearerToken(request) ?? string.Empty, search, heroId, take ?? 300);
    return overview == null ? Results.Unauthorized() : Results.Ok(overview);
});

api.MapPost("/dota/cosmetics/import", (HttpRequest request, SkyNetDotaImportRequestDto payload, SteamApiStateService state) =>
{
    var result = state.ImportDotaCosmetics(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

api.MapPost("/dota/equipment", (HttpRequest request, SkyNetDotaEquipItemRequestDto payload, SteamApiStateService state) =>
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

api.MapPost("/network/p2p/send", (HttpRequest request, SkyNetP2PPacketSendDto payload, SteamApiStateService state) =>
{
    return state.SendP2P(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/network/p2p/send-batch", (HttpRequest request, SkyNetP2PPacketBatchDto payload, SteamApiStateService state) =>
{
    return state.SendP2PBatch(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/gamecoordinator/messages", (HttpRequest request, SkyNetGCMessageDto payload, SteamApiStateService state) =>
{
    return state.SendGCMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/gamecoordinator/exchange", (HttpRequest request, SkyNetGCExchangeRequestDto payload, SteamApiStateService state) =>
{
    var response = state.ExchangeGCMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload, SteamApiStateService.GetClientIp(request));
    return response == null ? Results.Unauthorized() : Results.Ok(response);
});

api.MapPost("/gamecoordinator/poll", (HttpRequest request, SkyNetGCPollRequestDto payload, SteamApiStateService state) =>
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
