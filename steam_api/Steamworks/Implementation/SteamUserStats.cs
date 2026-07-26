using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Types;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUserStats : ISteamInterface
    {
        public static SteamUserStats Instance;

        private List<Leaderboard> Leaderboards;
        private List<Achievement> Achievements;
        private ConcurrentDictionary<ulong, List<PlayerStat>> PlayerStats;
        private readonly object StatsGate = new object();
        private readonly object AchievementGate = new object();
        private readonly object LeaderboardGate = new object();
        private readonly ConcurrentDictionary<ulong, LeaderboardEntryData[]> DownloadedLeaderboardEntries =
            new ConcurrentDictionary<ulong, LeaderboardEntryData[]>();
        private readonly ConcurrentQueue<ulong> DownloadedLeaderboardEntryOrder = new ConcurrentQueue<ulong>();
        private long NextLocalLeaderboardHandle;
        private long NextLeaderboardEntriesHandle;

        public SteamUserStats()
        {
            Instance = this;
            InterfaceName = "SteamUserStats";
            InterfaceVersion = "STEAMUSERSTATS_INTERFACE_VERSION013";
            Leaderboards = new List<Leaderboard>();
            Achievements = new List<Achievement>();
            PlayerStats = new ConcurrentDictionary<ulong, List<PlayerStat>>();
        }

        internal void SetLeaderboards(List<Leaderboard> leaderboards)
        {
            lock (LeaderboardGate)
            {
                Leaderboards = leaderboards ?? new List<Leaderboard>();
            }
        }

        internal void SetAchievements(List<Achievement> achievements)
        {
            lock (AchievementGate)
            {
                Achievements = achievements ?? new List<Achievement>();
            }
        }

        internal void SetPlayerStats(ulong steamID, List<PlayerStat> playerStats)
        {
            PlayerStats.TryAdd(steamID, playerStats);
        }


        public bool RequestCurrentStats()
        {
            try
            {
                Write($"RequestCurrentStats");
                var requestSucceeded = true;
                if (APIClient.IsEnabled)
                {
                    WorkQueue.Enqueue("RequestCurrentStats", () =>
                    {
                        var ok = APIClient.RefreshCurrentStats(true);
                        SyncSelfFromCache(false);
                        CallbackManager.AddCallback(new UserStatsReceived_t
                        {
                            m_nGameID = SteamEmulator.AppID,
                            m_eResult = ok ? EResult.k_EResultOK : EResult.k_EResultFail,
                            m_steamIDUser = SteamEmulator.SteamID
                        });
                    }, "stats:current", true);
                    return true;
                }

                UserStatsReceived_t data = new UserStatsReceived_t()
                {
                    m_nGameID = SteamEmulator.AppID,
                    m_eResult = requestSucceeded ? EResult.k_EResultOK : EResult.k_EResultFail,
                    m_steamIDUser = SteamEmulator.SteamID
                };
                CallbackManager.AddCallback(data);
                return requestSucceeded;
            }
            catch (Exception ex)
            {
                Write($"RequestCurrentStats {ex}");
                return false;
            }
        }

        public bool GetStat(string pchName, ref uint pData)
        {
            SyncSelfFromCache();
            bool Result = false;
            uint Data = 0;
            try
            {
                lock (StatsGate)
                {
                    if (PlayerStats.TryGetValue((ulong)SteamEmulator.SteamID, out var userStats))
                    {
                        var statsList = userStats.Find(n => n.Name == pchName);
                        if (statsList != null)
                        {
                            Data = statsList.Data;
                            Result = true;
                        }
                    }

                    if (!Result && StatDefinitionManager.TryGetIntDefault(pchName, out var defaultValue))
                    {
                        Data = unchecked((uint)defaultValue);
                        Result = true;
                    }
                }
                pData = Data;
            }
            catch (Exception ex)
            {
                Write($"GetStat ({pchName}) failed: {ex.Message}");
            }
            Write($"GetStat (Name = {pchName}, out Data = {pData}) = {Result}");
            return Result;
        }

        public bool GetStat(string pchName, ref float pData)
        {
            SyncSelfFromCache();
            bool Result = false;
            var Data = 0f;
            PlayerStat playerStat = null;
            lock (StatsGate)
            {
                if (PlayerStats.TryGetValue((ulong)SteamEmulator.SteamID, out var playerStats))
                {
                    playerStat = playerStats.Find(n => n.Name == pchName);
                    if (playerStat == null)
                    {
                        if (StatDefinitionManager.TryGetFloatDefault(pchName, out var defaultValue))
                        {
                            Data = defaultValue;
                            Result = true;
                        }
                    }
                    else
                    {
                        Data = DecodeFloatStat(playerStat.Data);
                        Result = true;
                    }
                }
                else
                {
                    Result = StatDefinitionManager.TryGetFloatDefault(pchName, out Data);
                }
            }
            pData = Data;
            Write($"GetStat (Name = {pchName}, out Data = {pData}) = {Result}");
            return Result;

        }

        public bool GetStatInt32(string pchName, IntPtr pData)
        {
            uint data = 0;
            bool result = GetStat(pchName, ref data);
            WriteInt32(pData, unchecked((int)data));
            return result;
        }

        public bool GetStatFloat(string pchName, IntPtr pData)
        {
            float data = 0;
            bool result = GetStat(pchName, ref data);
            WriteSingle(pData, data);
            return result;
        }

        public bool SetStat(string pchName, uint nData)
        {
            Write($"SetStat (Name = {pchName}, Data = {nData})");
            SyncSelfFromCache();

            bool Result = false;
            PlayerStat playerStat = null;
            if (PlayerStats.TryGetValue((ulong)SteamEmulator.SteamID, out var playerStats))
            {
                playerStat = playerStats.Find(n => n.Name == pchName);
                if (playerStat == null)
                {
                    playerStat = new PlayerStat() { Name = pchName, Data = nData };
                    playerStats.Add(playerStat);
                }
                else
                {
                    playerStat.Data = nData;
                }
                Result = true;
            }
            Write($"SetStat (Name = {pchName}, Data = {nData}) = {Result}");
            StateCache.SetLocalStat(pchName, nData);
            return Result;
        }

        public bool SetStat(string pchName, float fData)
        {
            if (string.IsNullOrWhiteSpace(pchName) || float.IsNaN(fData) || float.IsInfinity(fData))
            {
                return false;
            }

            return SetStat(pchName, EncodeFloatStat(fData));
        }

        public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateAvgRateStat ({pchName}, Count = {flCountThisSession}, Seconds = {dSessionLength})");
            if (string.IsNullOrWhiteSpace(pchName) ||
                float.IsNaN(flCountThisSession) ||
                float.IsInfinity(flCountThisSession) ||
                double.IsNaN(dSessionLength) ||
                double.IsInfinity(dSessionLength) ||
                dSessionLength <= 0)
            {
                return false;
            }

            var countName = AverageRateInternalName(pchName, "count");
            var durationName = AverageRateInternalName(pchName, "duration");
            var previousCount = ReadFloatStatOrDefault(countName);
            var previousDuration = ReadFloatStatOrDefault(durationName);
            var totalCount = previousCount + flCountThisSession;
            var totalDuration = previousDuration + dSessionLength;
            if (totalDuration <= 0 || totalCount < float.MinValue || totalCount > float.MaxValue ||
                totalDuration > float.MaxValue)
            {
                return false;
            }

            return SetStat(countName, (float)totalCount) &&
                SetStat(durationName, (float)totalDuration) &&
                SetStat(pchName, (float)(totalCount / totalDuration));
        }

        public bool GetAchievement(string pchName, ref bool pbAchieved)
        {
            Write($"GetAchievement (Name = {pchName})");
            SyncSelfFromCache();
            var Result = false;
            var achieved = false;
            lock (AchievementGate)
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achieved = false;
                    Result = AchievementDefinitionManager.HasDefinition(pchName);
                }
                else
                {
                    achieved = achievement.Earned;
                    Result = true;
                }
            }
            pbAchieved = achieved;
            return Result;
        }

        public bool GetAchievement(string pchName, IntPtr pbAchieved)
        {
            bool achieved = false;
            bool result = GetAchievement(pchName, ref achieved);
            WriteBool(pbAchieved, achieved);
            return result;
        }

        public bool SetAchievement(string pchName)
        {
            Write($"SetAchievement {pchName}");
            if (!AchievementDefinitionManager.IsKnownOrUnconfigured(pchName))
            {
                return false;
            }

            SyncSelfFromCache();
            var Result = false;
            lock (AchievementGate)
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achievement = new Achievement()
                    {
                        Name = pchName,
                        Date = DateTime.UtcNow,
                        Earned = true
                    };
                    Achievements.Add(achievement);
                    Result = true;
                }
                else
                {
                    achievement.Earned = true;
                    achievement.Date = DateTime.UtcNow;
                    Result = true;
                }
            }

            if (Result)
            {
                StateCache.SetLocalAchievement(pchName, true);
            }

            return Result;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement {pchName}");
            if (!AchievementDefinitionManager.IsKnownOrUnconfigured(pchName))
            {
                return false;
            }

            SyncSelfFromCache();
            lock (AchievementGate)
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    achievement.Earned = false;
                    achievement.Progress = 0;
                }
            }
            StateCache.SetLocalAchievement(pchName, false);
            return true;
        }

        public bool GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, ref uint punUnlockTime)
        {
            Write($"GetAchievementAndUnlockTime {pchName}");
            var Result = false;
            var Archived = false;
            uint UnlockTime = 0;
            lock (AchievementGate)
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    Archived = achievement.Earned;
                    UnlockTime = (uint)(new DateTimeOffset(achievement.Date)).ToUnixTimeSeconds();
                    Result = true;
                }
                else
                {
                    Result = AchievementDefinitionManager.HasDefinition(pchName);
                }
            }
            pbAchieved = Archived;
            punUnlockTime = UnlockTime;
            return Result;
        }

        public bool GetAchievementAndUnlockTime(string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            bool achieved = false;
            uint unlockTime = 0;
            bool result = GetAchievementAndUnlockTime(pchName, ref achieved, ref unlockTime);
            WriteBool(pbAchieved, achieved);
            WriteUInt32(punUnlockTime, unlockTime);
            return result;
        }

        public bool StoreStats()
        {
            try
            {
                Write($"StoreStats");
                var stored = true;
                if (APIClient.IsEnabled)
                {
                    SyncSelfFromCache(false);
                    WorkQueue.Enqueue("StoreStats", () =>
                    {
                        var ok = APIClient.StoreStats();
                        CallbackManager.AddCallback(new UserStatsStored_t
                        {
                            m_nGameID = SteamEmulator.AppID,
                            m_eResult = ok ? EResult.k_EResultOK : EResult.k_EResultFail
                        });
                    }, "stats:store", true);
                    return true;
                }

                UserStatsStored_t data = new UserStatsStored_t()
                {
                    m_nGameID = SteamEmulator.AppID,
                    m_eResult = stored ? EResult.k_EResultOK : EResult.k_EResultFail
                };
                CallbackManager.AddCallback(data);
                return stored;
            }
            catch (Exception ex)
            {
                Write($"StoreStats {ex}");
                return false;
            }
        }

        public int GetAchievementIcon(string pchName)
        {
            var achieved = false;
            GetAchievement(pchName, ref achieved);
            var handle = AchievementDefinitionManager.GetIcon(pchName, achieved);
            Write($"GetAchievementIcon ({pchName}) = {handle}");
            return handle;
        }

        public string GetAchievementDisplayAttribute(string pchName, string pchKey)
        {
            Write($"GetAchievementDisplayAttribute ({pchName}, {pchKey})");
            return AchievementDefinitionManager.GetDisplayAttribute(pchName, pchKey);
        }

        public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
        {
            Write($"IndicateAchievementProgress");
            if (nMaxProgress == 0 || nCurProgress > nMaxProgress ||
                !AchievementDefinitionManager.IsKnownOrUnconfigured(pchName))
            {
                return false;
            }

            SyncSelfFromCache();
            lock (AchievementGate)
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achievement = new Achievement
                    {
                        Name = pchName,
                        Date = DateTime.MinValue,
                        Earned = false
                    };
                    Achievements.Add(achievement);
                }

                achievement.Progress = nCurProgress;
                achievement.MaxProgress = nMaxProgress;
            }

            UserAchievementStored_t data = new UserAchievementStored_t()
            {
                m_nGameID = SteamEmulator.AppID,
                m_bGroupAchievement = false,
                m_rgchAchievementName = Encoding.UTF8.GetBytes(pchName),
                m_nCurProgress = nCurProgress,
                m_nMaxProgress = nMaxProgress
            };

            CallbackManager.AddCallback(data);
            return true;
        }

        public uint GetNumAchievements()
        {
            uint achievements;
            if (AchievementDefinitionManager.Count > 0)
            {
                achievements = (uint)AchievementDefinitionManager.Count;
            }
            else
            {
                lock (AchievementGate)
                {
                    achievements = (uint)Achievements.Count;
                }
            }

            Write($"GetNumAchievements {achievements}");
            return achievements;
        }

        public string GetAchievementName(uint iAchievement)
        {
            string achievementName;
            if (AchievementDefinitionManager.Count > 0)
            {
                achievementName = AchievementDefinitionManager.GetName(iAchievement);
            }
            else
            {
                lock (AchievementGate)
                {
                    achievementName = iAchievement < Achievements.Count
                        ? Achievements[(int)iAchievement].Name
                        : string.Empty;
                }
            }

            Write($"GetAchievementName {iAchievement} {achievementName}");
            return achievementName;
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            try
            {
                Write($"RequestUserStats {steamIDUser}");
                if (APIClient.IsEnabled)
                {
                    return WorkQueue.EnqueueCallbackResult(new UserStatsReceived_t
                    {
                        m_nGameID = SteamEmulator.AppID,
                        m_eResult = EResult.k_EResultFail,
                        m_steamIDUser = (CSteamID)steamIDUser
                    }, () =>
                    {
                        var ok = APIClient.RefreshStatsForUser(steamIDUser, true);
                        SyncUserFromCache(steamIDUser, false);
                        return new UserStatsReceived_t
                        {
                            m_nGameID = SteamEmulator.AppID,
                            m_eResult = ok ? EResult.k_EResultOK : EResult.k_EResultFail,
                            m_steamIDUser = (CSteamID)steamIDUser
                        };
                    }, name: "RequestUserStats " + steamIDUser, coalesceKey: "stats:user:" + steamIDUser);
                }

                UserStatsReceived_t data = new UserStatsReceived_t()
                {
                    m_nGameID = SteamEmulator.AppID,
                    m_eResult = EResult.k_EResultOK,
                    m_steamIDUser = (CSteamID)steamIDUser
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"RequestUserStats {ex}");
            }
            return k_uAPICallInvalid;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, uint pData)
        {
            SyncUserFromCache(steamIDUser);
            bool Result = false;
            if (PlayerStats.TryGetValue(steamIDUser, out var userStats))
            {
                var statsList = userStats.Find(n => n.Name == pchName);
                pData = (statsList == null) ? 0 : statsList.Data;
                Result = true;
            }
            else
            {
                // TODO: Request through socket
            }
            Write($"GetUserStat (SteamID = {steamIDUser}, Name = {pchName}, Data = {pData}) = {Result}");
            return Result;
        }

        public bool GetUserStatInt32(ulong steamIDUser, string pchName, IntPtr pData)
        {
            SyncUserFromCache(steamIDUser);
            bool result = false;
            uint data = 0;
            if (PlayerStats.TryGetValue(steamIDUser, out var userStats))
            {
                var statsList = userStats.Find(n => n.Name == pchName);
                data = statsList == null ? 0 : statsList.Data;
                result = true;
            }
            WriteInt32(pData, unchecked((int)data));
            Write($"GetUserStatInt32 (SteamID = {steamIDUser}, Name = {pchName}, Data = {data}) = {result}");
            return result;
        }

        public bool GetUserStatFloat(ulong steamIDUser, string pchName, IntPtr pData)
        {
            SyncUserFromCache(steamIDUser);
            bool result = false;
            float data = 0;
            if (PlayerStats.TryGetValue(steamIDUser, out var userStats))
            {
                var statsList = userStats.Find(n => n.Name == pchName);
                data = statsList == null ? 0 : DecodeFloatStat(statsList.Data);
                result = true;
            }
            WriteSingle(pData, data);
            Write($"GetUserStatFloat (SteamID = {steamIDUser}, Name = {pchName}, Data = {data}) = {result}");
            return result;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            SyncUserFromCache(steamIDUser);
            bool Result = false;
            bool Archived = false;
            var achievements = steamIDUser == SteamEmulator.SteamID
                ? Achievements
                : StateCache.GetAchievements(steamIDUser);

            if (achievements != null)
            {
                foreach (var achievement in achievements)
                {
                    if (achievement.Name == pchName)
                    {
                        Archived = achievement.Earned;
                        pbAchieved = Archived;
                        Result = true;
                        break;
                    }
                }
            }
            return Result;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, IntPtr pbAchieved)
        {
            bool achieved = false;
            bool result = GetUserAchievement(steamIDUser, pchName, achieved);
            if (result)
            {
                var achievements = steamIDUser == SteamEmulator.SteamID
                    ? Achievements
                    : StateCache.GetAchievements(steamIDUser);
                var achievement = achievements?.Find(a => a.Name == pchName);
                achieved = achievement?.Earned ?? false;
            }
            WriteBool(pbAchieved, achieved);
            return result;
        }

        public bool GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            return false;
        }

        public bool GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            SyncUserFromCache(steamIDUser);
            bool result = false;
            bool achieved = false;
            uint unlockTime = 0;
            var achievements = steamIDUser == SteamEmulator.SteamID
                ? Achievements
                : StateCache.GetAchievements(steamIDUser);

            var achievement = achievements?.Find(a => a.Name == pchName);
            if (achievement != null)
            {
                achieved = achievement.Earned;
                unlockTime = (uint)(new DateTimeOffset(achievement.Date)).ToUnixTimeSeconds();
                result = true;
            }

            WriteBool(pbAchieved, achieved);
            WriteUInt32(punUnlockTime, unlockTime);
            return result;
        }

        public bool ResetAllStats(bool bAchievementsToo)
        {
            Write($"ResetAllStats");
            PlayerStats.Clear();
            if (bAchievementsToo)
            {
                lock (AchievementGate)
                {
                    Achievements.Clear();
                }
            }
            StateCache.ApplyStats((ulong)SteamEmulator.SteamID, new List<APIClient.ApiStat>());
            if (bAchievementsToo)
            {
                StateCache.ApplyAchievements((ulong)SteamEmulator.SteamID, new List<APIClient.ApiAchievement>());
            }
            return true;
        }

        public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
        {
            var name = (pchLeaderboardName ?? string.Empty).Trim();
            Write($"FindOrCreateLeaderboard (Name = {name}, SortMethod = {eLeaderboardSortMethod}, DisplayType = {eLeaderboardDisplayType})");
            if (!IsValidLeaderboardRequest(name, eLeaderboardSortMethod, eLeaderboardDisplayType))
            {
                return CallbackManager.AddCallbackResult(new LeaderboardFindResult_t());
            }

            if (!APIClient.IsEnabled)
            {
                var local = GetOrCreateLocalLeaderboard(name, eLeaderboardSortMethod, eLeaderboardDisplayType);
                return CallbackManager.AddCallbackResult(new LeaderboardFindResult_t
                {
                    SteamLeaderboard = local.SteamLeaderboard,
                    LeaderboardFound = 1
                });
            }

            return WorkQueue.EnqueueCallbackResult(
                new LeaderboardFindResult_t(),
                () =>
                {
                    var leaderboard = APIClient.FindOrCreateLeaderboard(name, eLeaderboardSortMethod, eLeaderboardDisplayType);
                    if (leaderboard == null)
                    {
                        return new LeaderboardFindResult_t();
                    }

                    CacheLeaderboard(leaderboard);
                    return new LeaderboardFindResult_t
                    {
                        SteamLeaderboard = leaderboard.SteamLeaderboard,
                        LeaderboardFound = 1
                    };
                },
                name: "Find or create leaderboard");
        }

        public SteamAPICall_t FindLeaderboard(string pchLeaderboardName)
        {
            try
            {
                Write($"FindOrCreateLeaderboard");

                Leaderboard leaderboard = Leaderboards.Find(l => l.Name == pchLeaderboardName);

                if (leaderboard == null)
                {
                    leaderboard = new Leaderboard()
                    {
                        Name = pchLeaderboardName,
                        ShortMethod = ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                        DisplayType = ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
                    };
                    Leaderboards.Add(leaderboard);
                }

                //LeaderboardFindResult_t data = new LeaderboardFindResult_t()
                //{
                //    m_bLeaderboardFound = 1,
                //    m_hSteamLeaderboard = default
                //};

                //return CallbackManager.AddCallbackResult(data, LeaderboardFindResult_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"FindOrCreateLeaderboard {ex}");
            }
            return k_uAPICallInvalid;
        }

        public string GetLeaderboardName(ulong hSteamLeaderboard)
        {
            var leaderboard = FindCachedLeaderboard(hSteamLeaderboard);
            Write($"GetLeaderboardName ({hSteamLeaderboard}) = {leaderboard?.Name ?? string.Empty}");
            return leaderboard?.Name ?? string.Empty;
        }

        public int GetLeaderboardEntryCount(ulong hSteamLeaderboard)
        {
            var count = FindCachedLeaderboard(hSteamLeaderboard)?.EntryCount ?? 0;
            Write($"GetLeaderboardEntryCount ({hSteamLeaderboard}) = {count}");
            return count;
        }

        public int GetLeaderboardSortMethod(ulong hSteamLeaderboard)
        {
            return (int)(FindCachedLeaderboard(hSteamLeaderboard)?.ShortMethod ??
                ELeaderboardSortMethod.k_ELeaderboardSortMethodNone);
        }

        public int GetLeaderboardDisplayType(ulong hSteamLeaderboard)
        {
            return (int)(FindCachedLeaderboard(hSteamLeaderboard)?.DisplayType ??
                ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone);
        }

        public SteamAPICall_t DownloadLeaderboardEntries(ulong hSteamLeaderboard, int eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries ({hSteamLeaderboard}, {eLeaderboardDataRequest}, {nRangeStart}, {nRangeEnd})");
            return QueueLeaderboardEntriesDownload(
                hSteamLeaderboard,
                eLeaderboardDataRequest,
                nRangeStart,
                nRangeEnd,
                null);
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(ulong hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers ({hSteamLeaderboard}, {cUsers})");
            if (cUsers < 0 || cUsers > 1000 || (cUsers > 0 && prgUsers == IntPtr.Zero))
            {
                return CallbackManager.AddCallbackResult(new LeaderboardScoresDownloaded_t
                {
                    SteamLeaderboard = hSteamLeaderboard
                });
            }

            var users = new ulong[cUsers];
            for (var index = 0; index < cUsers; index++)
            {
                users[index] = unchecked((ulong)Marshal.ReadInt64(prgUsers, index * sizeof(ulong)));
            }

            return QueueLeaderboardEntriesDownload(
                hSteamLeaderboard,
                (int)ELeaderboardDataRequest.k_ELeaderboardDataRequestUsers,
                0,
                Math.Max(0, cUsers - 1),
                users);
        }

        public bool GetDownloadedLeaderboardEntry(ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
        {
            if (pLeaderboardEntry == IntPtr.Zero ||
                index < 0 ||
                cDetailsMax < 0 ||
                !DownloadedLeaderboardEntries.TryGetValue(hSteamLeaderboardEntries, out var entries) ||
                index >= entries.Length)
            {
                return false;
            }

            var entry = entries[index];
            Marshal.StructureToPtr(new NativeLeaderboardEntry
            {
                SteamIdUser = entry.SteamId,
                GlobalRank = entry.GlobalRank,
                Score = entry.Score,
                DetailsCount = entry.Details?.Length ?? 0,
                UgcHandle = entry.UgcHandle
            }, pLeaderboardEntry, false);

            if (pDetails != IntPtr.Zero && cDetailsMax > 0)
            {
                var count = Math.Min(cDetailsMax, entry.Details?.Length ?? 0);
                for (var detailIndex = 0; detailIndex < count; detailIndex++)
                {
                    Marshal.WriteInt32(pDetails, detailIndex * sizeof(int), entry.Details[detailIndex]);
                }
            }

            return true;
        }

        public SteamAPICall_t UploadLeaderboardScore(ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, int nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        {
            Write($"UploadLeaderboardScore ({hSteamLeaderboard}, {eLeaderboardUploadScoreMethod}, {nScore})");
            if (hSteamLeaderboard == 0 ||
                eLeaderboardUploadScoreMethod is < 1 or > 2 ||
                cScoreDetailsCount < 0 ||
                cScoreDetailsCount > 64 ||
                (cScoreDetailsCount > 0 && pScoreDetails == IntPtr.Zero))
            {
                return CallbackManager.AddCallbackResult(new LeaderboardScoreUploaded_t
                {
                    SteamLeaderboard = hSteamLeaderboard,
                    Score = nScore
                });
            }

            var details = new int[cScoreDetailsCount];
            if (cScoreDetailsCount > 0)
            {
                Marshal.Copy(pScoreDetails, details, 0, cScoreDetailsCount);
            }

            if (!APIClient.IsEnabled)
            {
                return CallbackManager.AddCallbackResult(new LeaderboardScoreUploaded_t
                {
                    Success = 0,
                    SteamLeaderboard = hSteamLeaderboard,
                    Score = nScore
                });
            }

            return WorkQueue.EnqueueCallbackResult(
                new LeaderboardScoreUploaded_t
                {
                    SteamLeaderboard = hSteamLeaderboard,
                    Score = nScore
                },
                () =>
                {
                    var result = APIClient.UploadLeaderboardScore(
                        hSteamLeaderboard,
                        eLeaderboardUploadScoreMethod,
                        nScore,
                        details);
                    if (result == null)
                    {
                        return new LeaderboardScoreUploaded_t
                        {
                            SteamLeaderboard = hSteamLeaderboard,
                            Score = nScore
                        };
                    }

                    RefreshCachedLeaderboard(hSteamLeaderboard);
                    return new LeaderboardScoreUploaded_t
                    {
                        Success = result.Success ? (byte)1 : (byte)0,
                        SteamLeaderboard = hSteamLeaderboard,
                        Score = result.Score,
                        ScoreChanged = result.ScoreChanged ? (byte)1 : (byte)0,
                        GlobalRankNew = result.GlobalRankNew,
                        GlobalRankPrevious = result.GlobalRankPrevious
                    };
                },
                name: "Upload leaderboard score");
        }

        public SteamAPICall_t AttachLeaderboardUGC(ulong hSteamLeaderboard, ulong hUGC)
        {
            Write($"AttachLeaderboardUGC");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t GetNumberOfCurrentPlayers()
        {
            try
            {
                if (APIClient.IsEnabled)
                {
                    WorkQueue.Enqueue("Refresh current players", () => APIClient.RefreshCurrentStats(),
                        "stats:current-players");
                }

                var UsersOnline = APIClient.IsEnabled ? StateCache.GetCurrentPlayers() : UserManager.Users.Count;
                NumberOfCurrentPlayers_t data = new NumberOfCurrentPlayers_t()
                {
                    m_bSuccess = 1,
                    m_cPlayers = UsersOnline
                };

                Write($"GetNumberOfCurrentPlayers = {UsersOnline}");

                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"GetNumberOfCurrentPlayers {ex}");
            }
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RequestGlobalAchievementPercentages()
        {
            Write($"RequestGlobalAchievementPercentages");
            // GlobalAchievementPercentagesReady_t
            return k_uAPICallInvalid;
        }

        public int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetMostAchievedAchievementInfo");
            return -1;
        }

        public int GetMostAchievedAchievementInfo(IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            Write($"GetMostAchievedAchievementInfo");
            NativeStringCache.WriteUtf8Buffer(pchName, checked((int)unNameBufLen), string.Empty);
            WriteSingle(pflPercent, 0);
            WriteBool(pbAchieved, false);
            return -1;
        }

        public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetNextMostAchievedAchievementInfo");
            return -1;
        }

        public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            Write($"GetNextMostAchievedAchievementInfo");
            NativeStringCache.WriteUtf8Buffer(pchName, checked((int)unNameBufLen), string.Empty);
            WriteSingle(pflPercent, 0);
            WriteBool(pbAchieved, false);
            return -1;
        }

        public bool GetAchievementAchievedPercent(string pchName, float pflPercent)
        {
            Write($"GetAchievementAchievedPercent");
            return false;
        }

        public bool GetAchievementAchievedPercent(string pchName, IntPtr pflPercent)
        {
            Write($"GetAchievementAchievedPercent");
            WriteSingle(pflPercent, 0);
            return false;
        }

        public SteamAPICall_t RequestGlobalStats(int nHistoryDays)
        {
            try
            {
                Write($"RequestGlobalStats {nHistoryDays} days");
                if (APIClient.IsEnabled)
                {
                    WorkQueue.Enqueue("RequestGlobalStats", () => APIClient.RefreshCurrentStats(true),
                        "stats:global", true);
                }

                GlobalStatsReceived_t data = new GlobalStatsReceived_t()
                {
                    m_eResult = EResult.k_EResultOK,
                    m_nGameID = SteamEmulator.AppID
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"RequestGlobalStats {ex}");
            }
            return k_uAPICallInvalid;
        }

        public bool GetGlobalStat(string pchStatName, uint pData)
        {
            Write($"GetGlobalStat {pchStatName}");
            // TODO
            return false;
        }

        public bool GetGlobalStatInt64(string pchStatName, IntPtr pData)
        {
            Write($"GetGlobalStatInt64 {pchStatName}");
            WriteInt64(pData, 0);
            return false;
        }

        public bool GetGlobalStatDouble(string pchStatName, IntPtr pData)
        {
            Write($"GetGlobalStatDouble {pchStatName}");
            WriteDouble(pData, 0);
            return false;
        }

        public uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
        {
            Write($"GetGlobalStatHistory {pchStatName}");
            return 0;
        }

        public int GetGlobalStatHistoryInt64(string pchStatName, IntPtr pData, uint cubData)
        {
            Write($"GetGlobalStatHistoryInt64 {pchStatName}");
            return 0;
        }

        public int GetGlobalStatHistoryDouble(string pchStatName, IntPtr pData, uint cubData)
        {
            Write($"GetGlobalStatHistoryDouble {pchStatName}");
            return 0;
        }

        public bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
        {
            Write($"GetAchievementProgressLimits");
            return false;
        }

        public bool GetAchievementProgressLimitsInt32(string pchName, IntPtr pnMinProgress, IntPtr pnMaxProgress)
        {
            Write($"GetAchievementProgressLimitsInt32");
            WriteInt32(pnMinProgress, 0);
            WriteInt32(pnMaxProgress, 0);
            return false;
        }

        public bool GetAchievementProgressLimitsFloat(string pchName, IntPtr pfMinProgress, IntPtr pfMaxProgress)
        {
            Write($"GetAchievementProgressLimitsFloat");
            WriteSingle(pfMinProgress, 0);
            WriteSingle(pfMaxProgress, 0);
            return false;
        }

        private float ReadFloatStatOrDefault(string name)
        {
            var value = 0f;
            return GetStat(name, ref value) ? value : 0f;
        }

        private static string AverageRateInternalName(string name, string component) =>
            "__skynet_avg_" + component + "::" + name;

        private static uint EncodeFloatStat(float value) =>
            BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

        private static float DecodeFloatStat(uint value)
        {
            // Builds before typed stat support stored a numeric cast instead of the
            // IEEE-754 payload. Preserve those ordinary positive values while new
            // writes retain the exact float, including fractional and negative data.
            if (value != 0 && (value & 0x7F800000u) == 0)
            {
                return value;
            }

            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        private SteamAPICall_t QueueLeaderboardEntriesDownload(
            ulong leaderboardHandle,
            int dataRequest,
            int rangeStart,
            int rangeEnd,
            ulong[] users)
        {
            var pending = new LeaderboardScoresDownloaded_t
            {
                SteamLeaderboard = leaderboardHandle
            };
            if (leaderboardHandle == 0 || dataRequest is < 0 or > 3)
            {
                return CallbackManager.AddCallbackResult(pending);
            }

            if (!APIClient.IsEnabled)
            {
                var entriesHandle = StoreDownloadedLeaderboardEntries(Array.Empty<LeaderboardEntryData>());
                pending.SteamLeaderboardEntries = entriesHandle;
                return CallbackManager.AddCallbackResult(pending);
            }

            return WorkQueue.EnqueueCallbackResult(
                pending,
                () =>
                {
                    var result = APIClient.QueryLeaderboardEntries(
                        leaderboardHandle,
                        dataRequest,
                        rangeStart,
                        rangeEnd,
                        users);
                    if (result == null)
                    {
                        throw new InvalidOperationException("Leaderboard entry query failed");
                    }

                    CacheLeaderboard(result.Leaderboard);
                    foreach (var entry in result.Entries)
                    {
                        APIClient.QueueUserProfileRefresh(entry.SteamId);
                    }

                    var entriesHandle = StoreDownloadedLeaderboardEntries(result.Entries);
                    return new LeaderboardScoresDownloaded_t
                    {
                        SteamLeaderboard = leaderboardHandle,
                        SteamLeaderboardEntries = entriesHandle,
                        CEntryCount = result.Entries.Length
                    };
                },
                name: "Download leaderboard entries");
        }

        private ulong StoreDownloadedLeaderboardEntries(LeaderboardEntryData[] entries)
        {
            var handle = unchecked((ulong)System.Threading.Interlocked.Increment(ref NextLeaderboardEntriesHandle));
            DownloadedLeaderboardEntries[handle] = entries ?? Array.Empty<LeaderboardEntryData>();
            DownloadedLeaderboardEntryOrder.Enqueue(handle);

            while (DownloadedLeaderboardEntries.Count > 256 &&
                DownloadedLeaderboardEntryOrder.TryDequeue(out var expiredHandle))
            {
                DownloadedLeaderboardEntries.TryRemove(expiredHandle, out _);
            }

            return handle;
        }

        private Leaderboard GetOrCreateLocalLeaderboard(
            string name,
            ELeaderboardSortMethod sortMethod,
            ELeaderboardDisplayType displayType)
        {
            lock (LeaderboardGate)
            {
                var leaderboard = Leaderboards.Find(item =>
                    string.Equals(item.Name, name, StringComparison.Ordinal));
                if (leaderboard != null)
                {
                    return leaderboard;
                }

                leaderboard = new Leaderboard
                {
                    SteamLeaderboard = unchecked((ulong)System.Threading.Interlocked.Increment(
                        ref NextLocalLeaderboardHandle)),
                    Name = name,
                    ShortMethod = sortMethod,
                    DisplayType = displayType
                };
                Leaderboards.Add(leaderboard);
                return leaderboard;
            }
        }

        private void CacheLeaderboard(Leaderboard leaderboard)
        {
            if (leaderboard == null || leaderboard.SteamLeaderboard == 0)
            {
                return;
            }

            lock (LeaderboardGate)
            {
                var index = Leaderboards.FindIndex(item =>
                    item.SteamLeaderboard == leaderboard.SteamLeaderboard ||
                    string.Equals(item.Name, leaderboard.Name, StringComparison.Ordinal));
                if (index >= 0)
                {
                    Leaderboards[index] = leaderboard;
                }
                else
                {
                    Leaderboards.Add(leaderboard);
                }
            }
        }

        private Leaderboard FindCachedLeaderboard(ulong handle)
        {
            lock (LeaderboardGate)
            {
                return Leaderboards.Find(item => item.SteamLeaderboard == handle);
            }
        }

        private void RefreshCachedLeaderboard(ulong handle)
        {
            var leaderboard = APIClient.GetLeaderboard(handle);
            if (leaderboard != null)
            {
                CacheLeaderboard(leaderboard);
            }
        }

        private static bool IsValidLeaderboardRequest(
            string name,
            ELeaderboardSortMethod sortMethod,
            ELeaderboardDisplayType displayType)
        {
            return name.Length is > 0 and <= 128 &&
                sortMethod is ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending
                    or ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending &&
                (int)displayType is >= 0 and <= 3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
        private struct NativeLeaderboardEntry
        {
            public ulong SteamIdUser;
            public int GlobalRank;
            public int Score;
            public int DetailsCount;
            public ulong UgcHandle;
        }

        private void SyncSelfFromCache(bool refresh = false)
        {
            if (refresh && APIClient.IsEnabled)
            {
                WorkQueue.Enqueue("Sync self stats", () => APIClient.RefreshCurrentStats(),
                    "stats:self");
            }

            SyncUserFromCache((ulong)SteamEmulator.SteamID, false);
            SetAchievements(StateCache.GetAchievements((ulong)SteamEmulator.SteamID));
        }

        private void SyncUserFromCache(ulong steamIDUser, bool refresh = false)
        {
            if (!APIClient.IsEnabled)
            {
                return;
            }

            if (refresh && steamIDUser == (ulong)SteamEmulator.SteamID)
            {
                WorkQueue.Enqueue("Sync current user stats", () => APIClient.RefreshCurrentStats(),
                    "stats:self");
            }
            else if (refresh)
            {
                WorkQueue.Enqueue("Sync user stats", () => APIClient.RefreshStatsForUser(steamIDUser),
                    "stats:user:" + steamIDUser);
            }

            PlayerStats[steamIDUser] = StateCache.GetStats(steamIDUser);
            if (steamIDUser == (ulong)SteamEmulator.SteamID)
            {
                SetAchievements(StateCache.GetAchievements(steamIDUser));
            }
        }

        private static void WriteBool(IntPtr destination, bool value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteByte(destination, value ? (byte)1 : (byte)0);
            }
        }

        private static void WriteInt32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, value);
            }
        }

        private static void WriteUInt32(IntPtr destination, uint value)
        {
            WriteInt32(destination, unchecked((int)value));
        }

        private static void WriteInt64(IntPtr destination, long value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt64(destination, value);
            }
        }

        private static void WriteSingle(IntPtr destination, float value)
        {
            if (destination != IntPtr.Zero)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Marshal.Copy(bytes, 0, destination, bytes.Length);
            }
        }

        private static void WriteDouble(IntPtr destination, double value)
        {
            if (destination != IntPtr.Zero)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Marshal.Copy(bytes, 0, destination, bytes.Length);
            }
        }

    }
}
