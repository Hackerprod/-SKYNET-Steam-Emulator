using SKYNET_server.Models;
using SKYNET_server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<SteamUiMockService>();
builder.Services.AddSingleton<SteamApiStateService>();

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

api.MapPost("/auth/steam/session", (SkyNetSessionRequestDto request, SteamApiStateService state) =>
{
    return Results.Ok(state.StartSession(request));
});

api.MapGet("/users/me", (HttpRequest request, SteamApiStateService state) =>
{
    var user = state.GetSelf(SteamApiStateService.GetBearerToken(request) ?? string.Empty);
    return user == null ? Results.Unauthorized() : Results.Ok(user);
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
    return Results.File(state.GetAvatar(SteamApiStateService.GetBearerToken(request) ?? string.Empty, steamId), "image/png");
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

api.MapGet("/events", (HttpRequest request, string? since, SteamApiStateService state) =>
{
    return Results.Ok(state.PollEvents(SteamApiStateService.GetBearerToken(request) ?? string.Empty, since));
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

api.MapPost("/network/p2p/send", (HttpRequest request, SkyNetP2PPacketSendDto payload, SteamApiStateService state) =>
{
    return state.SendP2P(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

api.MapPost("/gamecoordinator/messages", (HttpRequest request, SkyNetGCMessageDto payload, SteamApiStateService state) =>
{
    return state.SendGCMessage(SteamApiStateService.GetBearerToken(request) ?? string.Empty, payload)
        ? Results.Ok()
        : Results.Unauthorized();
});

app.MapRazorPages();
app.Run();
