using SKYNET_server.Models;
using System.Security.Cryptography;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public ApiSessionResult? StartSession(ApiSessionRequest request, string? clientIp)
    {
        lock (_sync)
        {
            ExpireStaleSessionsLocked();
            var clientInstanceId = NormalizeClientInstanceId(request.ClientInstanceId, request.SteamId, request.AccountId);
            var activeWebUser = request.UseActiveWebUser ? GetActiveWebUserForIpLocked(clientIp) : null;
            if (request.UseActiveWebUser && activeWebUser == null)
            {
                return null;
            }

            var requestedSteamId = activeWebUser?.SteamId
                ?? (request.SteamId != 0 ? request.SteamId : ToSteamId(request.AccountId != 0 ? request.AccountId : NextAvailableAccountIdLocked()));
            var steamId = activeWebUser != null ? requestedSteamId : ResolveSessionSteamIdLocked(requestedSteamId, clientInstanceId);
            var user = activeWebUser != null
                ? EnsureUser(steamId, activeWebUser.AccountId, request.AppId != 0 ? request.AppId : activeWebUser.AppId, activeWebUser.PersonaName)
                : EnsureUser(steamId, SteamIdToAccountId(steamId), request.AppId, request.PersonaName);
            var session = new ApiSession
            {
                SteamId = steamId,
                AccessToken = Guid.NewGuid().ToString("N"),
                RefreshToken = Guid.NewGuid().ToString("N"),
                ClientInstanceId = clientInstanceId,
                RemoteIp = clientIp,
                LastSeenUtc = DateTime.UtcNow
            };

            _sessions[session.AccessToken] = session;
            MarkUserOnlineLocked(steamId);
            return new ApiSessionResult
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken,
                User = CloneUser(user)
            };
        }
    }

    // True when the user has a live (recently-seen) game session. Web-dashboard
    // sessions do not count as in-game presence.
    private bool IsUserOnlineLocked(ulong steamId)
    {
        var now = DateTime.UtcNow;
        foreach (var session in _sessions.Values)
        {
            if (session.SteamId != steamId || session.WebSession)
            {
                continue;
            }

            if (session.LastSeenUtc >= now - _presenceTimeout)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsKnownDotaUser(ulong steamId)
    {
        lock (_sync)
        {
            return steamId != 0 && _state.Users.ContainsKey(steamId);
        }
    }

    private bool IsOnlineDotaUser(ulong steamId)
    {
        lock (_sync)
        {
            return IsUserOnlineLocked(steamId);
        }
    }

    private void MarkUserOnlineLocked(ulong steamId)
    {
        if (_state.Users.TryGetValue(steamId, out var user) && user.PersonaState != 1)
        {
            user.PersonaState = 1;
            EnqueueFriendEvents(steamId, "persona_state_changed", PersonaChangeStatus);
            SaveState();
        }
    }

    // Reconciles every user's PersonaState with live-session presence, emitting
    // persona_state_changed on any transition so friends see online/offline in
    // real time. Called periodically by PresenceSweepService.
    public void ReconcilePresence()
    {
        lock (_sync)
        {
            var changed = false;
            foreach (var user in _state.Users.Values)
            {
                var online = IsUserOnlineLocked(user.SteamId) ? 1 : 0;
                if (user.PersonaState != online)
                {
                    user.PersonaState = online;
                    if (online == 0)
                    {
                        // Going offline: drop stale rich presence so friends don't
                        // keep seeing "in match / hero X" for someone who left.
                        user.RichPresence.Clear();
                        user.HeroId = 0;
                        user.GameState = "offline";
                    }

                    EnqueueFriendEvents(user.SteamId, "persona_state_changed", PersonaChangeStatus);
                    changed = true;
                }
            }

            if (changed)
            {
                SaveState();
            }
        }
    }

    // Explicit "going offline" from the client (e.g. on game shutdown). Drops the
    // caller's live game sessions so presence flips to offline immediately instead
    // of waiting out the presence timeout, and notifies friends.
    public bool MarkSelfOffline(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var steamId = session!.SteamId;
            var gameSessions = _sessions
                .Where(kv => kv.Value.SteamId == steamId && !kv.Value.WebSession)
                .Select(kv => kv.Key)
                .ToList();
            foreach (var key in gameSessions)
            {
                _sessions.Remove(key);
            }

            if (_state.Users.TryGetValue(steamId, out var user) && user.PersonaState != 0)
            {
                user.PersonaState = 0;
                user.GameState = "offline";
                user.HeroId = 0;
                user.RichPresence.Clear();
                EnqueueFriendEvents(steamId, "persona_state_changed", PersonaChangeStatus);
            }

            SaveState();
            return true;
        }
    }

    public ApiUser? GetSelf(string token)
    {
        lock (_sync)
        {
            return TryGetUserByToken(token, out var user) ? CloneUser(user) : null;
        }
    }

    public List<ApiUser>? GetFriends(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            return GetKnownSocialUsersLocked(session!.SteamId);
        }
    }

    public ApiUser? GetUser(string token, ulong steamId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Users.TryGetValue(steamId, out var user))
            {
                return null;
            }

            return CloneUserForViewerLocked(user, session!.SteamId);
        }
    }

    public ApiUser? UpdatePersona(string token, string personaName)
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

    public ApiAvatarContent GetAvatar(string token, ulong steamId)
    {
        lock (_sync)
        {
            if (_state.Avatars.TryGetValue(steamId, out var avatarBase64) &&
                TryDecodeAvatar(avatarBase64, out var avatar))
            {
                return BuildAvatarContent(steamId, avatar, false);
            }

            return BuildAvatarContent(steamId, DefaultAvatarPng, true);
        }
    }

    public ApiAvatarContent GetAvatarByAccountId(uint accountId)
    {
        lock (_sync)
        {
            var steamId = ToSteamId(accountId);
            if (_state.Avatars.TryGetValue(steamId, out var avatarBase64) &&
                TryDecodeAvatar(avatarBase64, out var avatar))
            {
                return BuildAvatarContent(steamId, avatar, false);
            }

            return BuildAvatarContent(steamId, DefaultAvatarPng, true);
        }
    }

    public bool PutSelfAvatar(string token, ApiAvatarUpdate request)
    {
        lock (_sync)
        {
            if (!TryGetUserByToken(token, out var user) ||
                string.IsNullOrWhiteSpace(request.ContentBase64) ||
                !TryDecodeAvatar(request.ContentBase64, out var avatar) ||
                avatar.Length == 0 ||
                !AvatarImage.TryNormalize(avatar, out var normalized))
            {
                return false;
            }

            var normalizedBase64 = Convert.ToBase64String(normalized);
            if (_state.Avatars.TryGetValue(user.SteamId, out var current) &&
                string.Equals(current, normalizedBase64, StringComparison.Ordinal))
            {
                return true;
            }

            _state.Avatars[user.SteamId] = normalizedBase64;
            SaveState();
            EnqueueFriendEvents(user.SteamId, "persona_state_changed", PersonaChangeAvatar);
            return true;
        }
    }

    private static bool TryDecodeAvatar(string avatarBase64, out byte[] avatar)
    {
        avatar = Array.Empty<byte>();
        try
        {
            avatar = Convert.FromBase64String(avatarBase64);
            return avatar.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static ApiAvatarContent BuildAvatarContent(ulong steamId, byte[] content, bool isDefault)
    {
        return new ApiAvatarContent
        {
            SteamId = steamId,
            Content = content,
            IsDefault = isDefault,
            ETag = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant()
        };
    }

    public ApiStatsEnvelope? GetStats(string token, ulong steamId, bool currentUser)
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
                stats = new ApiStatsEnvelope { SteamId = steamId, CurrentPlayers = 1 };
                _state.Stats[steamId] = stats;
            }

            return CloneStats(stats);
        }
    }

    public bool StoreStats(string token, ApiStoreStatsRequest request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var steamId = request.SteamId != 0 ? request.SteamId : session.SteamId;
            _state.Stats[steamId] = new ApiStatsEnvelope
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = request.Stats ?? new List<ApiStat>(),
                Achievements = request.Achievements ?? new List<ApiAchievement>()
            };

            SaveState();
            foreach (var stat in _state.Stats[steamId].Stats)
            {
                EnqueueEvent(steamId, new ApiEvent
                {
                    Type = "stats_updated",
                    SteamId = steamId,
                    StatName = stat.Name,
                    StatValue = stat.Data
                });
            }

            foreach (var achievement in _state.Stats[steamId].Achievements.Where(a => a.Earned))
            {
                EnqueueEvent(steamId, new ApiEvent
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

    public ApiEventEnvelope PollEvents(string token, string? since, int waitMs = 0)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new ApiEventEnvelope();
            }

            var lastSeen = 0L;
            long.TryParse(since, out lastSeen);
            var deadline = DateTime.UtcNow.AddMilliseconds(Math.Clamp(waitMs, 0, 30000));
            List<ApiQueuedEvent> matched;
            do
            {
                ExpireStaleSessionsLocked();
                matched = _events
                    .Where(e => e.Sequence > lastSeen && (e.RecipientSteamId == 0 || e.RecipientSteamId == session!.SteamId))
                    .OrderBy(e => e.Sequence)
                    .ToList();

                if (matched.Count > 0 || waitMs <= 0)
                {
                    break;
                }

                var remaining = deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    break;
                }

                Monitor.Wait(_sync, remaining > TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : remaining);
            }
            while (true);

            return new ApiEventEnvelope
            {
                Cursor = matched.Count == 0 ? lastSeen.ToString() : matched.Max(e => e.Sequence).ToString(),
                Events = matched.Select(e => CloneEvent(e.Event)).ToList()
            };
        }
    }

    private string NormalizeClientInstanceId(string? clientInstanceId, ulong steamId, uint accountId)
    {
        if (!string.IsNullOrWhiteSpace(clientInstanceId))
        {
            return clientInstanceId.Trim();
        }

        return $"legacy:{(steamId != 0 ? steamId.ToString() : accountId.ToString())}";
    }

    private ulong ResolveSessionSteamIdLocked(ulong requestedSteamId, string clientInstanceId)
    {
        var activeCollision = _sessions.Values.Any(session =>
            session.SteamId == requestedSteamId &&
            !string.IsNullOrWhiteSpace(session.ClientInstanceId) &&
            !string.Equals(session.ClientInstanceId, clientInstanceId, StringComparison.Ordinal));

        if (!activeCollision)
        {
            return requestedSteamId;
        }

        return ToSteamId(NextAvailableAccountIdLocked());
    }

    // Resolves the web-logged-in user bound to the calling machine's IP. Each PC
    // on the LAN logs in through the web with its own account, so a game client
    // is matched to whoever is logged in from the same address. No global "active
    // user", no manual pairing: if nobody logged in from this IP, there is no
    // identity to hand out and the game session is refused.
    private ApiUser? GetActiveWebUserForIpLocked(string? clientIp)
    {
        if (string.IsNullOrWhiteSpace(clientIp))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var session = _sessions.Values
            .Concat(_state.WebSessions.Values)
            .Where(s => s.WebSession
                && string.Equals(s.RemoteIp, clientIp, StringComparison.Ordinal)
                && s.ExpiresAtUtc > now)
            .OrderByDescending(s => s.LastSeenUtc)
            .FirstOrDefault();

        if (session == null)
        {
            return null;
        }

        return _state.Users.TryGetValue(session.SteamId, out var user) ? user : null;
    }

    private uint NextAvailableAccountIdLocked()
    {
        uint candidate = (uint)(100000 + _state.Users.Count + _sessions.Count);
        while (candidate == 0 || _state.Users.ContainsKey(ToSteamId(candidate)) || _sessions.Values.Any(session => session.SteamId == ToSteamId(candidate)))
        {
            candidate++;
        }

        return candidate;
    }
}
