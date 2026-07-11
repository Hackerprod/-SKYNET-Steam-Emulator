namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    // Live operational snapshot for the admin GC console: who is actually online
    // (derived presence), and the active Dota matches with players/heroes/cosmetics.
    // Admin-gated. Returns null when the caller is not a web admin.
    public object? GetLiveDiagnostics(string token)
    {
        if (!IsWebAdmin(token))
        {
            return null;
        }

        lock (_sync)
        {
            var now = DateTime.UtcNow;

            var sessions = _sessions.Values
                .Where(session => !session.WebSession)
                .GroupBy(session => session.SteamId)
                .Select(group =>
                {
                    var lastSeen = group.Max(session => session.LastSeenUtc);
                    _state.Users.TryGetValue(group.Key, out var user);
                    return new
                    {
                        steamId = group.Key.ToString(),
                        persona = user?.PersonaName ?? string.Empty,
                        appId = user?.AppId ?? 0,
                        online = IsUserOnlineLocked(group.Key),
                        lastSeenSeconds = (int)(now - lastSeen).TotalSeconds
                    };
                })
                .OrderBy(session => session.lastSeenSeconds)
                .ToList();

            var onlineUsers = _state.Users.Values.Count(user => user.PersonaState == 1);

            var matches = _state.DotaMatches.Values
                .OrderByDescending(match => match.UpdatedAt)
                .Take(10)
                .Select(match => new
                {
                    lobbyId = match.LobbyId.ToString(),
                    matchId = match.MatchId.ToString(),
                    serverSteamId = match.ServerSteamId.ToString(),
                    state = match.State,
                    gameState = match.GameState,
                    updatedSecondsAgo = (int)(now - match.UpdatedAt).TotalSeconds,
                    players = match.Players.Select(player => new
                    {
                        steamId = player.SteamId.ToString(),
                        persona = player.PersonaName,
                        team = player.Team,
                        heroId = player.HeroId,
                        equippedItems = player.Equipment?.Count ?? 0
                    }).ToList()
                })
                .ToList();

            return new
            {
                nowUtc = now,
                presenceTimeoutSeconds = (int)_presenceTimeout.TotalSeconds,
                sweepUsersOnline = onlineUsers,
                liveGameSessions = sessions.Count,
                sessions,
                matches
            };
        }
    }
}
