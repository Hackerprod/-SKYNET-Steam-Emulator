using System.Security.Cryptography;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public const string WebSessionCookieName = "skynet_web_token";

    private const string DefaultAdminUsername = "Hackerprod";
    private const string DefaultAdminPassword = "Steam2026";
    private const uint DefaultAdminAccountId = 9931;
    private const uint DefaultAppId = 570;
    private const int PasswordIterations = 100_000;
    private static readonly TimeSpan WebSessionLifetime = TimeSpan.FromHours(12);
    private static readonly TimeSpan RememberedWebSessionLifetime = TimeSpan.FromDays(30);

    public ApiWebLoginResult? LoginWeb(string username, string password, bool rememberMe, string? remoteIp)
    {
        lock (_sync)
        {
            var key = NormalizeUsername(username);
            if (!_state.WebAccounts.TryGetValue(key, out var account) || !VerifyPassword(password, account.PasswordHash))
            {
                return null;
            }

            if (!_state.Users.TryGetValue(account.SteamId, out var existingUser))
            {
                existingUser = EnsureUser(account.SteamId, SteamIdToAccountId(account.SteamId), DefaultAppId, account.Username);
            }

            account.LastLoginAt = DateTime.UtcNow;
            _state.ActiveWebSteamId = account.SteamId;
            var session = CreateSessionLocked(account.SteamId, rememberMe, remoteIp);
            SaveState();

            return new ApiWebLoginResult
            {
                AccessToken = session.AccessToken,
                User = CloneUser(existingUser),
                IsAdmin = account.IsAdmin
            };
        }
    }

    public ApiWebLoginResult? RegisterWeb(string username, string personaName, string password, string? remoteIp)
    {
        lock (_sync)
        {
            var key = NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(personaName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            if (_state.WebAccounts.ContainsKey(key))
            {
                return null;
            }

            var accountId = AllocateAccountIdLocked();
            var steamId = ToSteamId(accountId);
            var user = EnsureUser(steamId, accountId, DefaultAppId, personaName.Trim());
            var account = new ApiWebAccount
            {
                Username = username.Trim(),
                PasswordHash = HashPassword(password),
                SteamId = steamId,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _state.WebAccounts[key] = account;
            _state.ActiveWebSteamId = steamId;
            var session = CreateSessionLocked(steamId, false, remoteIp);
            SaveState();

            return new ApiWebLoginResult
            {
                AccessToken = session.AccessToken,
                User = CloneUser(user),
                IsAdmin = false
            };
        }
    }

    public void LogoutWeb(string token)
    {
        lock (_sync)
        {
            if (!string.IsNullOrWhiteSpace(token) && _sessions.TryGetValue(token, out var session))
            {
                RemoveSessionLocked(token);
                if (_state.ActiveWebSteamId == session.SteamId)
                {
                    _state.ActiveWebSteamId = 0;
                }

                SaveState();
            }
        }
    }

    public ApiUser? GetWebUser(string? token)
    {
        lock (_sync)
        {
            return TryGetUserByToken(token ?? string.Empty, out var user) ? CloneUser(user) : null;
        }
    }

    public bool IsWebAdmin(string? token)
    {
        lock (_sync)
        {
            return TryGetWebAccountByToken(token ?? string.Empty, out var account) && account.IsAdmin;
        }
    }

    public ApiWebAccountView? GetWebAccount(string? token)
    {
        lock (_sync)
        {
            if (!TryGetWebAccountByToken(token ?? string.Empty, out var account))
            {
                return null;
            }

            return CloneWebAccountView(account);
        }
    }

    public bool ChangeOwnWebPassword(string token, string currentPassword, string nextPassword)
    {
        lock (_sync)
        {
            if (!TryGetWebAccountByToken(token, out var account) ||
                !VerifyPassword(currentPassword, account.PasswordHash) ||
                string.IsNullOrWhiteSpace(nextPassword))
            {
                return false;
            }

            account.PasswordHash = HashPassword(nextPassword);
            SaveState();
            return true;
        }
    }

    public bool ResetUserPassword(string token, ulong targetSteamId, string newPassword)
    {
        lock (_sync)
        {
            if (!TryGetWebAccountByToken(token, out var admin) || !admin.IsAdmin)
                return false;

            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            var target = _state.WebAccounts.Values.FirstOrDefault(a => a.SteamId == targetSteamId);
            if (target == null)
                return false;

            target.PasswordHash = HashPassword(newPassword);
            SaveState();
            return true;
        }
    }

    public SteamUiSnapshot GetWebSnapshot(string? token)
    {
        lock (_sync)
        {
            ApiUser user;
            if (!TryGetUserByToken(token ?? string.Empty, out user))
            {
                user = GetDefaultWebUserLocked() ?? new ApiUser();
            }

            var stats = user == null || !_state.Stats.TryGetValue(user.SteamId, out var foundStats)
                ? new ApiStatsEnvelope()
                : foundStats;
            var friends = user == null ? new List<ApiUser>() : GetFriendUsersLocked(user.SteamId);
            var activeServers = _state.GameServers.Count;
            var activeLobbies = _state.Lobbies.Values.Count(l => l.Members.Count > 0);
            var statCount = stats.Stats.Count;
            var achievementCount = stats.Achievements.Count;

            return new SteamUiSnapshot
            {
                Profile = new SteamProfileCard
                {
                    DisplayName = user?.PersonaName ?? "Guest",
                    Status = user == null ? "Offline" : "Online",
                    Level = user?.PlayerLevel ?? 0,
                    LibraryCount = _state.Users.Count,
                    CompletedGames = achievementCount
                },
                FeaturedGame = new SteamGame
                {
                    Name = "Dota 2",
                    Genre = "MOBA",
                    Status = activeServers > 0 ? "Server online" : "Idle",
                    Description = "Emulated Steam API session backed by SKYNET server state.",
                    HoursPlayed = (int)(stats.Stats.FirstOrDefault(s => s.Name.Equals("playtime", StringComparison.OrdinalIgnoreCase))?.Data ?? 0),
                    Progress = Math.Min(100, achievementCount * 10),
                    AchievementsUnlocked = stats.Achievements.Count(a => a.Earned),
                    TotalAchievements = Math.Max(achievementCount, 1)
                },
                Metrics = new[]
                {
                    new SteamMetric { Label = "Users", Value = _state.Users.Count.ToString(), Detail = "registered profiles" },
                    new SteamMetric { Label = "Friends", Value = friends.Count.ToString(), Detail = "linked to session" },
                    new SteamMetric { Label = "Lobbies", Value = activeLobbies.ToString(), Detail = "active rooms" },
                    new SteamMetric { Label = "Servers", Value = activeServers.ToString(), Detail = "registered game servers" }
                },
                Games = new[]
                {
                    new SteamGame
                    {
                        Name = "Dota 2",
                        Genre = "MOBA",
                        Status = activeServers > 0 ? "Ready" : "Waiting",
                        Description = "Steamworks emulation, GC routing and local profile state.",
                        HoursPlayed = (int)(stats.Stats.FirstOrDefault(s => s.Name.Equals("playtime", StringComparison.OrdinalIgnoreCase))?.Data ?? 0),
                        Progress = Math.Min(100, statCount * 15),
                        AchievementsUnlocked = stats.Achievements.Count(a => a.Earned),
                        TotalAchievements = Math.Max(achievementCount, 1)
                    }
                },
                Friends = friends.Select(MapFriend).ToList(),
                Achievements = stats.Achievements.Select(a => new SteamAchievement
                {
                    Title = a.Name,
                    Game = "Dota 2",
                    Description = a.Earned ? "Unlocked" : "In progress",
                    Rarity = a.Earned ? "Unlocked" : "Locked",
                    Unlocked = a.Earned,
                    Progress = a.MaxProgress == 0 ? (a.Earned ? 100 : 0) : (int)Math.Min(100, (a.Progress * 100UL) / a.MaxProgress)
                }).ToList(),
                Activities = BuildActivitiesLocked(user, stats)
            };
        }
    }

    public ApiAdminOverview? GetAdminOverview(string token)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return null;
            }

            return new ApiAdminOverview
            {
                Users = _state.Users.Values.OrderBy(u => u.PersonaName).Select(CloneUser).ToList(),
                Accounts = _state.WebAccounts.Values.OrderBy(a => a.Username).Select(CloneWebAccountView).ToList(),
                Lobbies = _state.Lobbies.Values.OrderByDescending(l => l.Members.Count).Select(CloneLobby).ToList(),
                GameServers = _state.GameServers.Values.ToList(),
                FriendLinks = _state.FriendLinks.Values.Sum(links => links.Count),
                PendingFriendRequests = _state.FriendRequests.Count(IsPending),
                StatsProfiles = _state.Stats.Count,
                DotaCosmetics = BuildDotaCosmeticSummaryLocked(),
                DotaMatches = _state.DotaMatches.Values
                    .OrderByDescending(match => match.UpdatedAt)
                    .Take(12)
                    .Select(CloneDotaMatch)
                    .ToList(),
                ServerStartTime = _serverStartTime
            };
        }
    }

    public List<ApiUser> GetWebUsers(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new List<ApiUser>();
            }

            return _state.Users.Values.OrderBy(u => u.PersonaName).Select(user => CloneUserForViewerLocked(user, session!.SteamId)).ToList();
        }
    }

    public List<ApiUser> GetWebFriends(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new List<ApiUser>();
            }

            return GetFriendUsersLocked(session!.SteamId).Select(user => CloneUserForViewerLocked(user, session.SteamId)).ToList();
        }
    }

    public bool AddFriend(string token, string friend)
    {
        lock (_sync)
        {
            return SendFriendRequest(token, friend);
        }
    }

    public bool RemoveFriend(string token, ulong friendSteamId)
    {
        lock (_sync)
        {
            return RemoveFriendOrRequest(token, friendSteamId);
        }
    }

    public bool AdminSetPersona(string token, ulong steamId, string personaName)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token) || !_state.Users.TryGetValue(steamId, out var user) || string.IsNullOrWhiteSpace(personaName))
            {
                return false;
            }

            user.PersonaName = personaName.Trim();
            SaveState();
            EnqueueFriendEvents(user.SteamId, "persona_state_changed", PersonaChangeName);
            return true;
        }
    }

    public bool AdminLinkFriends(string token, ulong leftSteamId, ulong rightSteamId)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token) || !_state.Users.ContainsKey(leftSteamId) || !_state.Users.ContainsKey(rightSteamId) || leftSteamId == rightSteamId)
            {
                return false;
            }

            LinkFriendsLocked(leftSteamId, rightSteamId);
            CloseAnyPendingRequestsBetweenLocked(leftSteamId, rightSteamId, "accepted");
            SaveState();
            EnqueueRelationshipEventLocked(leftSteamId, rightSteamId, "friend_added", FriendRelationshipFriend, string.Empty);
            EnqueueRelationshipEventLocked(rightSteamId, leftSteamId, "friend_added", FriendRelationshipFriend, string.Empty);
            return true;
        }
    }

    public bool AdminUnlinkFriends(string token, ulong leftSteamId, ulong rightSteamId)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return false;
            }

            UnlinkFriendsLocked(leftSteamId, rightSteamId);
            CloseAnyPendingRequestsBetweenLocked(leftSteamId, rightSteamId, "cancelled");
            SaveState();
            EnqueueRelationshipEventLocked(leftSteamId, rightSteamId, "friend_removed", FriendRelationshipNone, string.Empty);
            EnqueueRelationshipEventLocked(rightSteamId, leftSteamId, "friend_removed", FriendRelationshipNone, string.Empty);
            return true;
        }
    }

    public bool AdminDeleteUser(string token, ulong steamId)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token) || !_state.Users.ContainsKey(steamId))
            {
                return false;
            }

            var account = _state.WebAccounts.Values.FirstOrDefault(a => a.SteamId == steamId);
            if (account != null && account.IsAdmin)
            {
                var adminCount = _state.WebAccounts.Values.Count(a => a.IsAdmin);
                if (adminCount <= 1)
                {
                    return false;
                }
            }

            _state.Users.Remove(steamId);
            _state.FriendLinks.Remove(steamId);
            foreach (var links in _state.FriendLinks.Values)
            {
                links.Remove(steamId);
            }
            _state.Stats.Remove(steamId);
            _state.Avatars.Remove(steamId);
            _state.DotaEquipment.Remove(steamId);

            if (account != null)
            {
                _state.WebAccounts.Remove(account.Username);
            }

            var pendingRequests = _state.FriendRequests
                .Where(r => r.FromSteamId == steamId || r.ToSteamId == steamId && r.Status == "pending")
                .ToList();
            foreach (var req in pendingRequests)
            {
                req.Status = "cancelled";
                req.RespondedAt = DateTime.UtcNow;
            }

            SaveState();
            return true;
        }
    }

    public bool AdminSetAdmin(string token, ulong steamId, bool makeAdmin)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return false;
            }

            var account = _state.WebAccounts.Values.FirstOrDefault(a => a.SteamId == steamId);
            if (account == null)
            {
                return false;
            }

            if (!makeAdmin)
            {
                var adminCount = _state.WebAccounts.Values.Count(a => a.IsAdmin);
                if (adminCount <= 1 && account.IsAdmin)
                {
                    return false;
                }
            }

            account.IsAdmin = makeAdmin;
            SaveState();
            return true;
        }
    }

    public ApiWebAccountView? AdminCreateWebAccount(string token, string username, string personaName, string password, bool isAdmin)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return null;
            }

            var key = NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(personaName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            if (_state.WebAccounts.ContainsKey(key))
            {
                return null;
            }

            var accountId = AllocateAccountIdLocked();
            var steamId = ToSteamId(accountId);
            var user = EnsureUser(steamId, accountId, DefaultAppId, personaName.Trim());
            var account = new ApiWebAccount
            {
                Username = username.Trim(),
                PasswordHash = HashPassword(password),
                SteamId = steamId,
                IsAdmin = isAdmin,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.MinValue
            };

            _state.WebAccounts[key] = account;
            SaveState();
            return CloneWebAccountView(account);
        }
    }

    private void EnsureDefaultAdminAccount()
    {
        lock (_sync)
        {
            if (_state.WebAccounts.Values.Any(a => a.IsAdmin))
            {
                return;
            }

            var steamId = ToSteamId(DefaultAdminAccountId);
            var user = EnsureUser(steamId, DefaultAdminAccountId, DefaultAppId, DefaultAdminUsername);
            user.HasFriend = false;
            _state.WebAccounts[NormalizeUsername(DefaultAdminUsername)] = new ApiWebAccount
            {
                Username = DefaultAdminUsername,
                PasswordHash = HashPassword(DefaultAdminPassword),
                SteamId = steamId,
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.MinValue
            };

            SaveState();
        }
    }

    private ApiSession CreateSessionLocked(ulong steamId, bool rememberMe, string? remoteIp)
    {
        var now = DateTime.UtcNow;
        var session = new ApiSession
        {
            SteamId = steamId,
            AccessToken = Guid.NewGuid().ToString("N"),
            RefreshToken = Guid.NewGuid().ToString("N"),
            ProcessRole = "web",
            RemoteIp = remoteIp,
            LastSeenUtc = now,
            WebSession = true,
            Persistent = rememberMe,
            ExpiresAtUtc = now + (rememberMe ? RememberedWebSessionLifetime : WebSessionLifetime)
        };

        _sessions[session.AccessToken] = session;
        if (rememberMe)
        {
            _state.WebSessions[session.AccessToken] = session;
        }

        return session;
    }

    private bool TryGetWebAccountByToken(string token, out ApiWebAccount account)
    {
        account = new ApiWebAccount();
        if (!TryGetSession(token, out var session) || session == null || !session.WebSession)
        {
            return false;
        }

        var found = _state.WebAccounts.Values.FirstOrDefault(a => a.SteamId == session!.SteamId);
        if (found == null)
        {
            return false;
        }

        account = found;
        return true;
    }

    private ApiUser? GetDefaultWebUserLocked()
    {
        var admin = _state.WebAccounts.Values.FirstOrDefault(a => a.IsAdmin);
        if (admin != null && _state.Users.TryGetValue(admin.SteamId, out var adminUser))
        {
            return adminUser;
        }

        return _state.Users.Values.OrderBy(u => u.PersonaName).FirstOrDefault();
    }

    private List<ApiUser> GetFriendUsersLocked(ulong steamId)
    {
        if (!_state.FriendLinks.TryGetValue(steamId, out var links))
        {
            return new List<ApiUser>();
        }

        return links
            .Where(_state.Users.ContainsKey)
            .Select(id =>
            {
                var user = CloneUser(_state.Users[id]);
                user.HasFriend = true;
                return user;
            })
            .OrderBy(u => u.PersonaName)
            .ToList();
    }

    private bool TryResolveUserLocked(string value, out ApiUser user)
    {
        user = new ApiUser();
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        if (ulong.TryParse(trimmed, out var steamId) && _state.Users.TryGetValue(steamId, out var bySteam))
        {
            user = bySteam;
            return true;
        }

        if (uint.TryParse(trimmed, out var accountId))
        {
            var fromAccount = _state.Users.Values.FirstOrDefault(u => u.AccountId == accountId);
            if (fromAccount != null)
            {
                user = fromAccount;
                return true;
            }
        }

        var byAccount = _state.WebAccounts.Values.FirstOrDefault(a => a.Username.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        if (byAccount != null && _state.Users.TryGetValue(byAccount.SteamId, out var accountUser))
        {
            user = accountUser;
            return true;
        }

        var byName = _state.Users.Values.FirstOrDefault(u => u.PersonaName.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
        if (byName != null)
        {
            user = byName;
            return true;
        }

        return false;
    }

    private void LinkFriendsLocked(ulong leftSteamId, ulong rightSteamId)
    {
        if (!_state.FriendLinks.TryGetValue(leftSteamId, out var leftLinks))
        {
            leftLinks = new HashSet<ulong>();
            _state.FriendLinks[leftSteamId] = leftLinks;
        }

        if (!_state.FriendLinks.TryGetValue(rightSteamId, out var rightLinks))
        {
            rightLinks = new HashSet<ulong>();
            _state.FriendLinks[rightSteamId] = rightLinks;
        }

        leftLinks.Add(rightSteamId);
        rightLinks.Add(leftSteamId);
    }

    private void UnlinkFriendsLocked(ulong leftSteamId, ulong rightSteamId)
    {
        if (_state.FriendLinks.TryGetValue(leftSteamId, out var leftLinks))
        {
            leftLinks.Remove(rightSteamId);
        }

        if (_state.FriendLinks.TryGetValue(rightSteamId, out var rightLinks))
        {
            rightLinks.Remove(leftSteamId);
        }
    }

    private void CloseAnyPendingRequestsBetweenLocked(ulong leftSteamId, ulong rightSteamId, string status)
    {
        foreach (var request in _state.FriendRequests.Where(request =>
                     IsPending(request) &&
                     ((request.FromSteamId == leftSteamId && request.ToSteamId == rightSteamId) ||
                      (request.FromSteamId == rightSteamId && request.ToSteamId == leftSteamId))))
        {
            request.Status = status;
            request.RespondedAt = DateTime.UtcNow;
        }
    }

    private uint AllocateAccountIdLocked()
    {
        var max = _state.Users.Values.Select(u => u.AccountId).DefaultIfEmpty(100_000U).Max();
        return Math.Max(max + 1U, 100_000U);
    }

    private ApiWebAccountView CloneWebAccountView(ApiWebAccount account)
    {
        _state.Users.TryGetValue(account.SteamId, out var user);
        return new ApiWebAccountView
        {
            Username = account.Username,
            SteamId = account.SteamId,
            PersonaName = user?.PersonaName ?? account.Username,
            IsAdmin = account.IsAdmin,
            CreatedAt = account.CreatedAt,
            LastLoginAt = account.LastLoginAt
        };
    }

    private static SteamFriend MapFriend(ApiUser user)
    {
        var status = user.RichPresence.TryGetValue("status", out var richPresence) && !string.IsNullOrWhiteSpace(richPresence)
            ? richPresence
            : (user.PersonaState == 0 ? "Offline" : "Online");

        return new SteamFriend
        {
            SteamId = user.SteamId,
            DisplayName = user.PersonaName,
            Initials = BuildInitials(user.PersonaName),
            Status = status.Contains("DOTA", StringComparison.OrdinalIgnoreCase) || status.Contains("game", StringComparison.OrdinalIgnoreCase) ? "In game" : status,
            CurrentGame = status,
            Note = $"SteamID {user.SteamId}"
        };
    }

    private List<SteamActivity> BuildActivitiesLocked(ApiUser? user, ApiStatsEnvelope stats)
    {
        var activities = new List<SteamActivity>();
        if (user != null)
        {
            activities.Add(new SteamActivity
            {
                Title = "Session ready",
                Description = $"{user.PersonaName} is linked to SteamID {user.SteamId}.",
                When = "now"
            });
        }

        activities.Add(new SteamActivity
        {
            Title = "State loaded",
            Description = $"{_state.Users.Count} users, {_state.Lobbies.Count} lobbies, {_state.GameServers.Count} game servers.",
            When = "server"
        });

        foreach (var stat in stats.Stats.Take(2))
        {
            activities.Add(new SteamActivity
            {
                Title = stat.Name,
                Description = $"Current value: {stat.Data}",
                When = "stats"
            });
        }

        return activities;
    }

    private static string NormalizeUsername(string value) => (value ?? string.Empty).Trim().ToLowerInvariant();

    private static string BuildInitials(string value)
    {
        var parts = (value ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0]).ToString())
            .ToArray();

        return parts.Length == 0 ? "U" : string.Concat(parts);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        using var deriveBytes = new Rfc2898DeriveBytes(password ?? string.Empty, salt, PasswordIterations, HashAlgorithmName.SHA256);
        var hash = deriveBytes.GetBytes(32);
        return $"pbkdf2-sha256${PasswordIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = (storedHash ?? string.Empty).Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            using var deriveBytes = new Rfc2898DeriveBytes(password ?? string.Empty, salt, iterations, HashAlgorithmName.SHA256);
            var actual = deriveBytes.GetBytes(expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
