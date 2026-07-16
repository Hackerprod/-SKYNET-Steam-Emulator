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
            Leaderboards = leaderboards;
        }

        internal void SetAchievements(List<Achievement> achievements)
        {
            Achievements = achievements;
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
                MutexHelper.Wait("UserStats", delegate
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
                });
                pData = Data;
            }
            catch (Exception)
            {
            }
            Write($"GetStat (Name = {pchName}, out Data = {pData}) = {Result}");
            return Result;
        }

        public bool GetStat(string pchName, ref float pData)
        {
            SyncSelfFromCache();
            bool Result = false;
            var Data = pData;
            PlayerStat playerStat = null;
            MutexHelper.Wait("UserStats", delegate
            {
                if (PlayerStats.TryGetValue((ulong)SteamEmulator.SteamID, out var playerStats))
                {
                    playerStat = playerStats.Find(n => n.Name == pchName);
                    if (playerStat == null)
                    {
                        playerStat = new PlayerStat();
                        playerStat.Name = pchName;
                        playerStat.Data = 0;

                        playerStats.Add(playerStat);
                    }
                    Data = playerStat.Data;
                    Result = true;
                }
                else
                {
                    playerStats = new List<PlayerStat>();

                    playerStat = new PlayerStat();
                    playerStat.Name = pchName;
                    playerStat.Data = 0;

                    playerStats.Add(playerStat);
                    PlayerStats.TryAdd((ulong)SteamEmulator.SteamID, playerStats);
                }
            });
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

        public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateAvgRateStat {pchName}");
            return false;
        }

        public bool GetAchievement(string pchName, ref bool pbAchieved)
        {
            Write($"GetAchievement (Name = {pchName})");
            SyncSelfFromCache();
            var Result = false;
            var achieved = false;
            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achieved = true;
                    Result = false;
                }
                else
                {
                    achieved = achievement.Earned;
                    Result = true;
                }
            });
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
            SyncSelfFromCache();
            var Result = false;
            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achievement = new Achievement()
                    {
                        Name = pchName,
                        Date = DateTime.Now,
                        Earned = true
                    };
                    Achievements.Add(achievement);
                    // TODO: Show Overlay with Achievement
                    Result = true;
                }
            });

            if (Result)
            {
                StateCache.SetLocalAchievement(pchName, true);
            }

            return Result;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement {pchName}");
            SyncSelfFromCache();
            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    achievement.Earned = false;
                    achievement.Progress = 0;
                }
            });
            StateCache.SetLocalAchievement(pchName, false);
            return true;
        }

        public bool GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, ref uint punUnlockTime)
        {
            Write($"GetAchievementAndUnlockTime {pchName}");
            var Result = false;
            var Archived = false;
            uint UnlockTime = 0;
            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    Archived = achievement.Earned;
                    UnlockTime = (uint)(new DateTimeOffset(achievement.Date)).ToUnixTimeSeconds();
                    Result = true;
                }
            });
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
            Write($"GetAchievementIcon");
            return 0;
        }

        public string GetAchievementDisplayAttribute(string pchName, string pchKey)
        {
            Write($"GetAchievementDisplayAttribute");
            return "";
        }

        public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
        {
            Write($"IndicateAchievementProgress");
            var Result = false;
            var Archived = false;

            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    achievement.Progress = nCurProgress;
                    achievement.MaxProgress = nMaxProgress;
                    Archived = achievement.Earned;
                    Result = true;
                }
            });

            UserAchievementStored_t data = new UserAchievementStored_t()
            {
                m_nGameID = SteamEmulator.AppID,
                m_bGroupAchievement = false,
                m_rgchAchievementName = Encoding.UTF8.GetBytes(pchName),
                m_nCurProgress = Archived ? nCurProgress : 0,
                m_nMaxProgress = Archived ? nMaxProgress : 0
            };

            CallbackManager.AddCallback(data);

            return Result;
        }

        public uint GetNumAchievements()
        {
            var achievements = (uint)Achievements.Count;
            Write($"GetNumAchievements {achievements}");
            return achievements;
        }

        public string GetAchievementName(uint iAchievement)
        {
            string achievementName = "";
            try
            {
                if (Achievements.Count <= iAchievement)
                    return "";
                achievementName = Achievements[(int)iAchievement].Name;
            }
            catch { }
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
                data = (statsList == null) ? 0 : statsList.Data;
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
                data = (statsList == null) ? 0 : statsList.Data;
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
                Achievements.Clear();
            StateCache.ApplyStats((ulong)SteamEmulator.SteamID, new List<APIClient.ApiStat>());
            if (bAchievementsToo)
            {
                StateCache.ApplyAchievements((ulong)SteamEmulator.SteamID, new List<APIClient.ApiAchievement>());
            }
            return true;
        }

        public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
        {
            try
            {
                Write($"FindOrCreateLeaderboard (Name = {pchLeaderboardName}, SortMethod = {(ELeaderboardSortMethod)eLeaderboardSortMethod}, DisplayType = {(ELeaderboardDisplayType)eLeaderboardDisplayType})");

                Leaderboard leaderboard = Leaderboards.Find( l => l.Name == pchLeaderboardName);

                if (leaderboard == null)
                {
                    leaderboard = new Leaderboard()
                    {
                        Name = pchLeaderboardName,
                        ShortMethod = (ELeaderboardSortMethod)eLeaderboardSortMethod,
                        DisplayType = (ELeaderboardDisplayType)eLeaderboardDisplayType
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
            Write($"GetLeaderboardName");
            return "";
        }

        public int GetLeaderboardEntryCount(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardEntryCount");
            return 0;
        }

        public int GetLeaderboardSortMethod(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardSortMethod");
            return default;
        }

        public int GetLeaderboardDisplayType(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardDisplayType");
            return 0;
        }

        public SteamAPICall_t DownloadLeaderboardEntries(ulong hSteamLeaderboard, int eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(ulong hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public bool GetDownloadedLeaderboardEntry(ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
        {
            Write($"GetDownloadedLeaderboardEntry");
            return false;
        }

        public SteamAPICall_t UploadLeaderboardScore(ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, uint nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        {
            Write($"UploadLeaderboardScore");
            // LeaderboardScoreUploaded_t
            return k_uAPICallInvalid;
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

        private void SyncSelfFromCache(bool refresh = false)
        {
            if (refresh && APIClient.IsEnabled)
            {
                WorkQueue.Enqueue("Sync self stats", () => APIClient.RefreshCurrentStats(),
                    "stats:self");
            }

            SyncUserFromCache((ulong)SteamEmulator.SteamID, false);
            Achievements = StateCache.GetAchievements((ulong)SteamEmulator.SteamID);
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
                Achievements = StateCache.GetAchievements(steamIDUser);
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
