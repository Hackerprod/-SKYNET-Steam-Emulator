using SKYNET_server.Models;
using System.Security.Cryptography;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public ApiSessionResult? StartSession(ApiSessionRequest request, string? clientIp) =>
        StartSession(request, clientIp, out _);

    public ApiSessionResult? StartSession(ApiSessionRequest request, string? clientIp, out SessionFailure failure)
    {
        lock (_sync)
        {
            var session = CreateSessionLocked(request, clientIp, out var user, out failure);
            if (session == null)
            {
                return null;
            }

            var steamId = session.SteamId;
            var isAdmin = _state.WebAccounts.Values.Any(a => a.SteamId == steamId && a.IsAdmin);

            return new ApiSessionResult
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken,
                User = CloneUser(user),
                IsAdmin = isAdmin,
                WorkshopSubscriptions = GetWorkshopSubscriptionsLocked(steamId, user.AppId),
                AchievementDefinitions = _achievementCatalog.Get(user.AppId),
                StatDefinitions = _statCatalog.Get(user.AppId)
            };
        }
    }

    // Registers a new session; returns null (and says why) when the user cannot be resolved.
    // A session never creates a user: the server is the only authority over who exists.
    private ApiSession? CreateSessionLocked(ApiSessionRequest request, string? clientIp, out ApiUser user, out SessionFailure failure)
    {
        user = null!;
        failure = SessionFailure.None;
        ExpireStaleSessionsLocked();
        var clientInstanceId = NormalizeClientInstanceId(request.ClientInstanceId, request.SteamId, request.AccountId);
        var processRole = NormalizeProcessRole(request.ProcessRole);
        var activeWebUser = request.UseActiveWebUser ? GetActiveWebUserForIpLocked(clientIp) : null;
        if (request.UseActiveWebUser && activeWebUser == null)
        {
            failure = SessionFailure.NoWebUser;
            return null;
        }

        ulong steamId;
        if (activeWebUser != null)
        {
            steamId = activeWebUser.SteamId;
        }
        else
        {
            // Without a web login the client can only name a user that is already
            // registered; an ID it made up (e.g. from its ini) creates nothing.
            steamId = request.SteamId != 0 ? request.SteamId : (request.AccountId != 0 ? ToSteamId(request.AccountId) : 0);
            if (steamId == 0 || !_state.Users.ContainsKey(steamId))
            {
                failure = SessionFailure.UnknownIdentity;
                return null;
            }

            if (IsSessionIdentityInUseLocked(steamId, clientInstanceId))
            {
                failure = SessionFailure.IdentityInUse;
                return null;
            }
        }

        var launcherLevel = IsLauncherLevelRole(processRole);
        if (processRole == ProcessRoleLauncher)
        {
            // Re-resolves must not pile up sessions; only replace launcher sessions.
            var previous = _sessions
                .Where(kv => kv.Value.SteamId == steamId
                    && string.Equals(kv.Value.ProcessRole, ProcessRoleLauncher, StringComparison.Ordinal)
                    && string.Equals(kv.Value.ClientInstanceId, clientInstanceId, StringComparison.Ordinal))
                .Select(kv => kv.Key)
                .ToList();
            foreach (var key in previous)
            {
                _sessions.Remove(key);
            }
        }

        // Launcher/probe sessions pass appId 0 so they never overwrite user.AppId.
        user = (activeWebUser != null
            ? GetOrUpdateUserLocked(steamId, launcherLevel ? 0 : (request.AppId != 0 ? request.AppId : activeWebUser.AppId), activeWebUser.PersonaName)
            : GetOrUpdateUserLocked(steamId, launcherLevel ? 0 : request.AppId, request.PersonaName))!;
        var changedUser = false;
        if (activeWebUser != null && !string.Equals(user.PersonaName, activeWebUser.PersonaName, StringComparison.Ordinal))
        {
            user.PersonaName = activeWebUser.PersonaName;
            changedUser = true;
        }

        var session = new ApiSession
        {
            SteamId = steamId,
            AppId = user.AppId,
            AccessToken = Guid.NewGuid().ToString("N"),
            RefreshToken = Guid.NewGuid().ToString("N"),
            ClientInstanceId = clientInstanceId,
            ProcessRole = processRole,
            RemoteIp = clientIp,
            LastSeenUtc = DateTime.UtcNow
        };

        _sessions[session.AccessToken] = session;
        if (!launcherLevel)
        {
            MarkUserOnlineLocked(steamId);
        }
        else if (processRole == ProcessRoleLauncher && ReconcileUserPresenceLocked(user))
        {
            changedUser = true;
        }

        if (changedUser)
        {
            SaveState();
        }

        return session;
    }

    // True when the user has a live (recently-seen) game session. Web-dashboard
    // sessions do not count as in-game presence.
    // Game-level presence only: launcher/probe sessions are excluded on purpose.
    // Do not relax this, the Dota GC depends on it via IsOnlineDotaUser.
    private bool IsUserOnlineLocked(ulong steamId)
    {
        var now = DateTime.UtcNow;
        foreach (var session in _sessions.Values)
        {
            if (session.SteamId != steamId || session.WebSession || IsLauncherLevelRole(session.ProcessRole))
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

    // True when the user's launcher is open and heartbeating.
    private bool IsLauncherOnlineLocked(ulong steamId)
    {
        var now = DateTime.UtcNow;
        foreach (var session in _sessions.Values)
        {
            if (session.SteamId == steamId
                && !session.WebSession
                && string.Equals(session.ProcessRole, ProcessRoleLauncher, StringComparison.Ordinal)
                && session.LastSeenUtc >= now - _presenceTimeout)
            {
                return true;
            }
        }

        return false;
    }

    // What friends see: the game is running OR the launcher is open. Unlike
    // IsUserOnlineLocked this is NOT used by the Dota GC or lobby liveness.
    private bool IsUserVisiblyOnlineLocked(ulong steamId) =>
        IsUserOnlineLocked(steamId) || IsLauncherOnlineLocked(steamId);

    // Brings one user's stored presence in line with live sessions and notifies
    // friends on any change. Visible presence (game OR launcher) drives
    // PersonaState; game-only data (rich presence, hero, advertised server) is
    // dropped as soon as the game session is gone, even if the launcher stays open.
    private bool ReconcileUserPresenceLocked(ApiUser user)
    {
        var gameOnline = IsUserOnlineLocked(user.SteamId);
        var online = gameOnline || IsLauncherOnlineLocked(user.SteamId) ? 1 : 0;
        var changed = false;

        if (!gameOnline && (user.RichPresence.Count > 0
            || user.HeroId != 0
            || user.GameServerSteamId != 0
            || !string.Equals(user.GameState, "offline", StringComparison.Ordinal)))
        {
            user.RichPresence.Clear();
            user.HeroId = 0;
            user.GameState = "offline";
            ClearAdvertisedGameServer(user);
            changed = true;
        }

        if (user.PersonaState != online)
        {
            user.PersonaState = online;
            changed = true;
        }

        if (changed)
        {
            EnqueueFriendEvents(user.SteamId, "persona_state_changed", PersonaChangeStatus);
        }

        return changed;
    }

    // "in_game" | "online" | "offline". Exposed only through GET /api/users.
    private string GetPresenceLevelLocked(ulong steamId)
    {
        if (IsUserOnlineLocked(steamId))
        {
            return "in_game";
        }

        return IsLauncherOnlineLocked(steamId) ? "online" : "offline";
    }

    private bool IsKnownDotaUser(ulong steamId)
    {
        lock (_sync)
        {
            return steamId != 0 && _state.Users.ContainsKey(steamId);
        }
    }

    // Called by the Dota GC script, including from its tick, which runs while it holds the script lock. Another
    // thread (a GC exchange) can hold the state lock while it waits for that script lock, so blocking here for the
    // state lock is a deadlock (it froze the whole server). When the state is busy report "online": the callers
    // only use this to clean up after users who left, and they ask again on their next tick.
    private bool IsOnlineDotaUser(ulong steamId)
    {
        if (!Monitor.TryEnter(_sync, 0))
        {
            return true;
        }

        try
        {
            return IsUserOnlineLocked(steamId);
        }
        finally
        {
            Monitor.Exit(_sync);
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
                // Going offline also drops stale rich presence so friends don't keep
                // seeing "in match / hero X" for someone who left.
                if (ReconcileUserPresenceLocked(user))
                {
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
            if (string.Equals(session.ProcessRole, ProcessRoleLauncher, StringComparison.Ordinal))
            {
                // Launcher closing: only its own launcher sessions go away. A game may
                // still be running, so persona/game state and friend events are untouched.
                var launcherSessions = _sessions
                    .Where(kv => kv.Value.SteamId == steamId
                        && string.Equals(kv.Value.ProcessRole, ProcessRoleLauncher, StringComparison.Ordinal)
                        && string.Equals(kv.Value.ClientInstanceId, session.ClientInstanceId, StringComparison.Ordinal))
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var key in launcherSessions)
                {
                    _sessions.Remove(key);
                }

                // Friends stop seeing the user online unless a game is still running.
                if (_state.Users.TryGetValue(steamId, out var launcherUser) && ReconcileUserPresenceLocked(launcherUser))
                {
                    SaveState();
                }

                return true;
            }

            var gameSessions = _sessions
                .Where(kv => kv.Value.SteamId == steamId && !kv.Value.WebSession && !IsLauncherLevelRole(kv.Value.ProcessRole))
                .Select(kv => kv.Key)
                .ToList();
            foreach (var key in gameSessions)
            {
                _sessions.Remove(key);
            }

            // Game gone: game data is dropped, but the user stays online for friends
            // while the launcher is still open.
            if (_state.Users.TryGetValue(steamId, out var user))
            {
                ReconcileUserPresenceLocked(user);
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

    public bool SetAdvertisedGameServer(string token, ApiGameServerPresenceUpdate update)
    {
        lock (_sync)
        {
            if (!TryGetUserByToken(token, out var user) || update == null)
            {
                return false;
            }

            var clear = update.Ip == 0 || update.Port == 0;
            user.GameServerSteamId = clear ? 0 : update.SteamId;
            user.GameServerIp = clear ? 0 : update.Ip;
            user.GameServerPort = clear ? (ushort)0 : update.Port;
            EnqueueFriendEvents(user.SteamId, "friend_presence_changed", PersonaChangeRichPresence);
            return true;
        }
    }

    private static void ClearAdvertisedGameServer(ApiUser user)
    {
        user.GameServerSteamId = 0;
        user.GameServerIp = 0;
        user.GameServerPort = 0;
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
                    .Where(e => e.Sequence > lastSeen && IsEventForSession(e, session!))
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

    private static string NormalizeProcessRole(string? processRole)
    {
        if (string.IsNullOrWhiteSpace(processRole))
        {
            return "client";
        }

        var normalized = processRole.Trim().ToLowerInvariant();
        return normalized switch
        {
            "client" => "client",
            "dedicated" => "dedicated",
            "server" => "dedicated",
            "web" => "web",
            ProcessRoleLauncher => ProcessRoleLauncher,
            ProcessRoleProbe => ProcessRoleProbe,
            // Unknown roles map to "client". An older server therefore treats
            // "launcher"/"probe" as a game-level client session.
            _ => "client"
        };
    }

    private const string ProcessRoleLauncher = "launcher";
    private const string ProcessRoleProbe = "probe";

    private static bool IsLauncherLevelRole(string? role)
    {
        return string.Equals(role, ProcessRoleLauncher, StringComparison.Ordinal)
            || string.Equals(role, ProcessRoleProbe, StringComparison.Ordinal);
    }

    private static bool IsEventForSession(ApiQueuedEvent queuedEvent, ApiSession session)
    {
        if (queuedEvent.RecipientSteamId != 0 && queuedEvent.RecipientSteamId != session.SteamId)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(queuedEvent.RecipientProcessRole) &&
            !string.Equals(queuedEvent.RecipientProcessRole, session.ProcessRole, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(queuedEvent.RecipientClientInstanceId) ||
            string.Equals(queuedEvent.RecipientClientInstanceId, session.ClientInstanceId, StringComparison.Ordinal);
    }

    // Another live client instance already holds this identity (two installs sharing one
    // ini identity). The second is refused rather than being given a made-up user.
    private bool IsSessionIdentityInUseLocked(ulong steamId, string clientInstanceId)
    {
        var liveSince = DateTime.UtcNow - _presenceTimeout;
        return _sessions.Values.Any(session =>
            session.SteamId == steamId &&
            !session.WebSession &&
            session.LastSeenUtc >= liveSince &&
            !string.IsNullOrWhiteSpace(session.ClientInstanceId) &&
            !string.Equals(session.ClientInstanceId, clientInstanceId, StringComparison.Ordinal));
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

        if (!_state.Users.TryGetValue(session.SteamId, out var user))
        {
            return null;
        }

        var account = _state.WebAccounts.Values.FirstOrDefault(item => item.SteamId == session.SteamId);
        if (account != null && !string.Equals(user.PersonaName, account.Username, StringComparison.Ordinal))
        {
            user.PersonaName = account.Username;
            SaveState();
        }

        return user;
    }

}

public enum SessionFailure
{
    None,
    NoWebUser,
    UnknownIdentity,
    IdentityInUse
}
