using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public SkyNetSessionDto? StartSession(SkyNetSessionRequestDto request)
    {
        lock (_sync)
        {
            ExpireStaleSessionsLocked();
            var clientInstanceId = NormalizeClientInstanceId(request.ClientInstanceId, request.SteamId, request.AccountId);
            var activeWebUser = request.UseActiveWebUser ? GetActiveWebUserLocked() : null;
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
                LastSeenUtc = DateTime.UtcNow
            };

            _sessions[session.AccessToken] = session;
            MarkUserOnlineLocked(steamId);
            return new SkyNetSessionDto
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

            return GetKnownSocialUsersLocked(session!.SteamId);
        }
    }

    public SkyNetUserDto? GetUser(string token, ulong steamId)
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
            if (_state.Avatars.TryGetValue(steamId, out var avatarBase64) &&
                TryDecodeAvatar(avatarBase64, out var avatar))
            {
                return avatar;
            }

            return DefaultAvatarPng;
        }
    }

    public byte[] GetAvatarByAccountId(uint accountId)
    {
        lock (_sync)
        {
            var steamId = ToSteamId(accountId);
            if (_state.Avatars.TryGetValue(steamId, out var avatarBase64) &&
                TryDecodeAvatar(avatarBase64, out var avatar))
            {
                return avatar;
            }

            return DefaultAvatarPng;
        }
    }

    public bool PutSelfAvatar(string token, SkyNetAvatarUpdateDto request)
    {
        lock (_sync)
        {
            if (!TryGetUserByToken(token, out var user) ||
                string.IsNullOrWhiteSpace(request.ContentBase64) ||
                !TryDecodeAvatar(request.ContentBase64, out var avatar) ||
                avatar.Length == 0 ||
                avatar.Length > 512 * 1024)
            {
                return false;
            }

            _state.Avatars[user.SteamId] = Convert.ToBase64String(avatar);
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

    public SkyNetEventEnvelopeDto PollEvents(string token, string? since, int waitMs = 0)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new SkyNetEventEnvelopeDto();
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

            return new SkyNetEventEnvelopeDto
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

    private SkyNetUserDto? GetActiveWebUserLocked()
    {
        return _state.ActiveWebSteamId != 0 &&
            HasActiveWebSessionLocked(_state.ActiveWebSteamId) &&
            _state.Users.TryGetValue(_state.ActiveWebSteamId, out var user)
            ? user
            : null;
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
