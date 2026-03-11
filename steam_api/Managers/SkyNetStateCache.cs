using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.Managers
{
    public static class SkyNetStateCache
    {
        private static readonly object SyncRoot = new object();
        private static readonly ConcurrentDictionary<ulong, SteamPlayer> Friends = new ConcurrentDictionary<ulong, SteamPlayer>();
        private static readonly ConcurrentDictionary<ulong, List<PlayerStat>> PlayerStats = new ConcurrentDictionary<ulong, List<PlayerStat>>();
        private static readonly ConcurrentDictionary<ulong, List<Achievement>> PlayerAchievements = new ConcurrentDictionary<ulong, List<Achievement>>();
        private static readonly ConcurrentDictionary<ulong, DateTime> StatsRefreshedAt = new ConcurrentDictionary<ulong, DateTime>();

        private static SteamPlayer Self;
        private static DateTime LastSelfRefresh = DateTime.MinValue;
        private static DateTime LastFriendsRefresh = DateTime.MinValue;
        private static bool SelfStatsDirty;
        private static bool SelfAchievementsDirty;
        private static string EventCursor;
        private static int CurrentPlayers;
        private static int SelfPlayerLevel;

        public static bool NeedsSelfRefresh(TimeSpan maxAge)
        {
            return Self == null || DateTime.UtcNow - LastSelfRefresh > maxAge;
        }

        public static bool NeedsFriendsRefresh(TimeSpan maxAge)
        {
            return DateTime.UtcNow - LastFriendsRefresh > maxAge;
        }

        public static bool NeedsStatsRefresh(ulong steamId, TimeSpan maxAge)
        {
            if (!StatsRefreshedAt.TryGetValue(steamId, out var refreshedAt))
            {
                return true;
            }

            return DateTime.UtcNow - refreshedAt > maxAge;
        }

        public static void ApplySelf(SkyNetApiClient.SkyNetUserDto user)
        {
            if (user == null)
            {
                return;
            }

            lock (SyncRoot)
            {
                Self = MapUser(user);
                LastSelfRefresh = DateTime.UtcNow;
                SelfPlayerLevel = user.PlayerLevel;

                if (user.AccountId != 0)
                {
                    SteamEmulator.SteamID = new CSteamID(user.AccountId, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                }
                else if (user.SteamId != 0)
                {
                    SteamEmulator.SteamID = new CSteamID(user.SteamId);
                }

                if (!string.IsNullOrWhiteSpace(user.PersonaName))
                {
                    SteamEmulator.PersonaName = user.PersonaName;
                }
            }
        }

        public static void ApplyFriends(IEnumerable<SkyNetApiClient.SkyNetUserDto> friends)
        {
            if (friends == null)
            {
                return;
            }

            lock (SyncRoot)
            {
                Friends.Clear();
                var mirrored = new List<SteamPlayer>();

                foreach (var friend in friends)
                {
                    var mapped = MapUser(friend);
                    Friends[mapped.SteamID] = mapped;
                    mirrored.Add(ClonePlayer(mapped));
                }

                LastFriendsRefresh = DateTime.UtcNow;
                UserManager.ReplaceUsers(mirrored);
            }
        }

        public static void UpsertUser(SkyNetApiClient.SkyNetUserDto user)
        {
            if (user == null)
            {
                return;
            }

            var mapped = MapUser(user);
            Friends[mapped.SteamID] = mapped;
            UserManager.ReplaceUsers(GetFriends());
        }

        public static void ApplyStats(ulong steamId, IEnumerable<SkyNetApiClient.SkyNetStatDto> stats)
        {
            var mapped = stats == null
                ? new List<PlayerStat>()
                : stats.Select(s => new PlayerStat
                {
                    Name = s.Name,
                    Data = s.Data
                }).ToList();

            PlayerStats[steamId] = mapped;
            StatsRefreshedAt[steamId] = DateTime.UtcNow;

            if (steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfStatsDirty = false;
            }
        }

        public static void ApplyAchievements(ulong steamId, IEnumerable<SkyNetApiClient.SkyNetAchievementDto> achievements)
        {
            var mapped = achievements == null
                ? new List<Achievement>()
                : achievements.Select(a => new Achievement
                {
                    Name = a.Name,
                    Earned = a.Earned,
                    Date = a.Date,
                    Progress = a.Progress,
                    MaxProgress = a.MaxProgress
                }).ToList();

            PlayerAchievements[steamId] = mapped;
            StatsRefreshedAt[steamId] = DateTime.UtcNow;

            if (steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfAchievementsDirty = false;
            }
        }

        public static void SetCurrentPlayers(int currentPlayers)
        {
            CurrentPlayers = currentPlayers;
        }

        public static int GetCurrentPlayers()
        {
            return CurrentPlayers;
        }

        public static int GetSelfPlayerLevel()
        {
            return SelfPlayerLevel;
        }

        public static bool TryGetSelf(out SteamPlayer self)
        {
            self = Self == null ? null : ClonePlayer(Self);
            return self != null;
        }

        public static bool TryGetFriend(ulong steamId, out SteamPlayer friend)
        {
            friend = null;

            if (Friends.TryGetValue(steamId, out var cached))
            {
                friend = ClonePlayer(cached);
            }

            return friend != null;
        }

        public static List<SteamPlayer> GetFriends()
        {
            return Friends.Values.Select(ClonePlayer).OrderBy(f => f.PersonaName).ToList();
        }

        public static List<PlayerStat> GetStats(ulong steamId)
        {
            if (PlayerStats.TryGetValue(steamId, out var stats))
            {
                return stats.Select(s => new PlayerStat
                {
                    Name = s.Name,
                    Data = s.Data
                }).ToList();
            }

            return new List<PlayerStat>();
        }

        public static List<Achievement> GetAchievements(ulong steamId)
        {
            if (PlayerAchievements.TryGetValue(steamId, out var achievements))
            {
                return achievements.Select(a => new Achievement
                {
                    Name = a.Name,
                    Earned = a.Earned,
                    Date = a.Date,
                    Progress = a.Progress,
                    MaxProgress = a.MaxProgress
                }).ToList();
            }

            return new List<Achievement>();
        }

        public static void SetLocalStat(string name, uint data)
        {
            UpsertStat((ulong)SteamEmulator.SteamID, name, data, true);
        }

        public static void UpsertStat(ulong steamId, string name, uint data, bool markDirty)
        {
            var stats = PlayerStats.GetOrAdd(steamId, _ => new List<PlayerStat>());
            var stat = stats.Find(s => s.Name == name);

            if (stat == null)
            {
                stats.Add(new PlayerStat
                {
                    Name = name,
                    Data = data
                });
            }
            else
            {
                stat.Data = data;
            }

            if (markDirty && steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfStatsDirty = true;
            }

            StatsRefreshedAt[steamId] = DateTime.UtcNow;
        }

        public static void SetLocalAchievement(string name, bool earned, uint progress = 0, uint maxProgress = 0)
        {
            UpsertAchievement((ulong)SteamEmulator.SteamID, name, earned, progress, maxProgress, true);
        }

        public static void UpsertAchievement(ulong steamId, string name, bool earned, uint progress, uint maxProgress, bool markDirty)
        {
            var achievements = PlayerAchievements.GetOrAdd(steamId, _ => new List<Achievement>());
            var achievement = achievements.Find(a => a.Name == name);

            if (achievement == null)
            {
                achievements.Add(new Achievement
                {
                    Name = name,
                    Earned = earned,
                    Date = DateTime.UtcNow,
                    Progress = progress,
                    MaxProgress = maxProgress
                });
            }
            else
            {
                achievement.Earned = earned;
                achievement.Date = DateTime.UtcNow;
                achievement.Progress = progress;
                achievement.MaxProgress = maxProgress;
            }

            if (markDirty && steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfAchievementsDirty = true;
            }

            StatsRefreshedAt[steamId] = DateTime.UtcNow;
        }

        public static bool HasPendingStatsChanges()
        {
            return SelfStatsDirty || SelfAchievementsDirty;
        }

        public static void MarkStatsStored()
        {
            SelfStatsDirty = false;
            SelfAchievementsDirty = false;
        }

        public static string GetEventCursor()
        {
            return EventCursor ?? string.Empty;
        }

        public static void SetEventCursor(string cursor)
        {
            EventCursor = cursor ?? string.Empty;
        }

        public static void UpsertFriendFromEvent(SkyNetApiClient.SkyNetEventDto serverEvent)
        {
            if (serverEvent == null)
            {
                return;
            }

            var steamId = serverEvent.SteamId != 0
                ? serverEvent.SteamId
                : (ulong)new CSteamID(serverEvent.AccountId);

            var user = new SteamPlayer
            {
                AccountID = serverEvent.AccountId != 0 ? serverEvent.AccountId : steamId.GetAccountID(),
                SteamID = steamId,
                PersonaName = serverEvent.PersonaName ?? string.Empty,
                GameID = serverEvent.AppId,
                LobbyID = serverEvent.LobbyId,
                HasFriend = true,
                RichPresence = serverEvent.RichPresence ?? new Dictionary<string, string>()
            };

            Friends[steamId] = user;
            UserManager.ReplaceUsers(GetFriends());
        }

        public static void RemoveFriend(ulong steamId)
        {
            Friends.TryRemove(steamId, out _);
            UserManager.ReplaceUsers(GetFriends());
        }

        private static SteamPlayer MapUser(SkyNetApiClient.SkyNetUserDto user)
        {
            var steamId = user.SteamId != 0
                ? user.SteamId
                : (ulong)new CSteamID(user.AccountId);

            return new SteamPlayer
            {
                AccountID = user.AccountId != 0 ? user.AccountId : steamId.GetAccountID(),
                SteamID = steamId,
                PersonaName = user.PersonaName ?? string.Empty,
                GameID = user.AppId,
                LobbyID = user.LobbyId,
                HasFriend = user.HasFriend || steamId == (ulong)SteamEmulator.SteamID,
                IPAddress = string.Empty,
                RichPresence = user.RichPresence ?? new Dictionary<string, string>()
            };
        }

        private static SteamPlayer ClonePlayer(SteamPlayer player)
        {
            if (player == null)
            {
                return null;
            }

            return new SteamPlayer
            {
                AccountID = player.AccountID,
                SteamID = player.SteamID,
                PersonaName = player.PersonaName,
                GameID = player.GameID,
                LobbyID = player.LobbyID,
                HasFriend = player.HasFriend,
                IPAddress = player.IPAddress,
                RichPresence = new Dictionary<string, string>(player.RichPresence ?? new Dictionary<string, string>())
            };
        }
    }
}
