using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public SkyNetSessionDto StartSession(SkyNetSessionRequestDto request)
    {
        lock (_sync)
        {
            var steamId = request.SteamId != 0 ? request.SteamId : ToSteamId(request.AccountId);
            var user = EnsureUser(steamId, request.AccountId, request.AppId, request.PersonaName);
            var session = new ApiSession
            {
                SteamId = steamId,
                AccessToken = Guid.NewGuid().ToString("N"),
                RefreshToken = Guid.NewGuid().ToString("N")
            };

            _sessions[session.AccessToken] = session;
            return new SkyNetSessionDto
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken,
                User = CloneUser(user)
            };
        }
    }

    public SkyNetUserDto? GetSelf(string token)
    {
        lock (_sync)
        {
            return TryGetUserByToken(token, out var user) ? CloneUser(user) : null;
        }
    }

    public List<SkyNetUserDto>? GetFriends(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            EnsureSeedFriends(session.SteamId, _state.Users[session.SteamId].AppId);
            if (!_state.FriendLinks.TryGetValue(session.SteamId, out var links))
            {
                return new List<SkyNetUserDto>();
            }

            return links.Where(_state.Users.ContainsKey).Select(id => CloneUser(_state.Users[id])).ToList();
        }
    }

    public SkyNetUserDto? GetUser(string token, ulong steamId)
    {
        lock (_sync)
        {
            return _state.Users.TryGetValue(steamId, out var user) ? CloneUser(user) : null;
        }
    }

    public SkyNetUserDto? UpdatePersona(string token, string personaName)
    {
        lock (_sync)
        {
            if (!TryGetUserByToken(token, out var user))
            {
                return null;
            }

            user.PersonaName = personaName ?? string.Empty;
            SaveState();
            EnqueueFriendEvents(user.SteamId, "persona_state_changed", PersonaChangeName);
            return CloneUser(user);
        }
    }

    public bool SetPresence(string token, string key, string value)
    {
        lock (_sync)
        {
            if (!TryGetUserByToken(token, out var user))
            {
                return false;
            }

            user.RichPresence[key ?? string.Empty] = value ?? string.Empty;
            SaveState();
            EnqueueFriendEvents(user.SteamId, "friend_presence_changed", PersonaChangeRichPresence);
            return true;
        }
    }

    public byte[] GetAvatar(string token, ulong steamId)
    {
        lock (_sync)
        {
            return DefaultAvatarPng;
        }
    }

    public SkyNetStatsEnvelopeDto? GetStats(string token, ulong steamId, bool currentUser)
    {
        lock (_sync)
        {
            ApiSession? session = null;

            if (currentUser)
            {
                if (!TryGetSession(token, out session))
                {
                    return null;
                }

                steamId = session!.SteamId;
            }

            if (!_state.Stats.TryGetValue(steamId, out var stats))
            {
                stats = new SkyNetStatsEnvelopeDto { SteamId = steamId, CurrentPlayers = 1 };
                _state.Stats[steamId] = stats;
            }

            return CloneStats(stats);
        }
    }

    public bool StoreStats(string token, SkyNetStoreStatsRequestDto request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var steamId = request.SteamId != 0 ? request.SteamId : session.SteamId;
            _state.Stats[steamId] = new SkyNetStatsEnvelopeDto
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = request.Stats ?? new List<SkyNetStatDto>(),
                Achievements = request.Achievements ?? new List<SkyNetAchievementDto>()
            };

            SaveState();
            foreach (var stat in _state.Stats[steamId].Stats)
            {
                EnqueueEvent(steamId, new SkyNetEventDto
                {
                    Type = "stats_updated",
                    SteamId = steamId,
                    StatName = stat.Name,
                    StatValue = stat.Data
                });
            }

            foreach (var achievement in _state.Stats[steamId].Achievements.Where(a => a.Earned))
            {
                EnqueueEvent(steamId, new SkyNetEventDto
                {
                    Type = "achievement_unlocked",
                    SteamId = steamId,
                    AchievementName = achievement.Name,
                    AchievementEarned = achievement.Earned,
                    AchievementProgress = achievement.Progress,
                    AchievementMaxProgress = achievement.MaxProgress
                });
            }

            return true;
        }
    }

    public SkyNetEventEnvelopeDto PollEvents(string token, string? since)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new SkyNetEventEnvelopeDto();
            }

            var lastSeen = 0L;
            long.TryParse(since, out lastSeen);
            var matched = _events
                .Where(e => e.Sequence > lastSeen && (e.RecipientSteamId == 0 || e.RecipientSteamId == session.SteamId))
                .OrderBy(e => e.Sequence)
                .ToList();

            return new SkyNetEventEnvelopeDto
            {
                Cursor = matched.Count == 0 ? lastSeen.ToString() : matched.Max(e => e.Sequence).ToString(),
                Events = matched.Select(e => CloneEvent(e.Event)).ToList()
            };
        }
    }
}
