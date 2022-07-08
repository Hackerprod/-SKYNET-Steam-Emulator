using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using SKYNET.Callback;
using SKYNET.Helper;
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
            InterfaceVersion = "STEAMUSERSTATS_INTERFACE_VERSION011";
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
                UserStatsReceived_t data = new UserStatsReceived_t()
                {
                    m_nGameID = SteamEmulator.AppID,
                    m_eResult = EResult.k_EResultOK, 
                    m_steamIDUser = SteamEmulator.SteamID
                };
                CallbackManager.AddCallbackResult(data);
                return true;
            }
            catch (Exception ex)
            {
                Write($"RequestCurrentStats {ex}");
                return false;
            }
        }

        public bool GetStat(string pchName, ref uint pData)
        {
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
                        IPCManager.SendPlayerStat(playerStat);
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
                    IPCManager.SendPlayerStat(playerStat);
                }
            });
            pData = Data;
            Write($"GetStat (Name = {pchName}, out Data = {pData}) = {Result}");
            return Result;

        }

        public bool SetStat(string pchName, uint nData)
        {
            Write($"SetStat (Name = {pchName}, Data = {nData})");

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
            IPCManager.SendPlayerStat(playerStat);
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

        public bool SetAchievement(string pchName)
        {
            Write($"SetAchievement {pchName}");
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
                    IPCManager.SendAchievement(achievement);
                    // TODO: Show Overlay with Achievement
                    Result = true;
                }
            });

            return Result;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement {pchName}");
            MutexHelper.Wait("Achievements", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    achievement.Earned = false;
                    achievement.Progress = 0;
                    IPCManager.SendUpdateAchievement(achievement);
                }
            });
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

        public bool StoreStats()
        {
            try
            {
                Write($"StoreStats");
                UserStatsStored_t data = new UserStatsStored_t()
                {
                    m_nGameID = SteamEmulator.AppID,
                    m_eResult = EResult.k_EResultOK
                };
                CallbackManager.AddCallbackResult(data);
                return true;
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
                    IPCManager.SendUpdateAchievement(achievement);
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

            CallbackManager.AddCallbackResult(data);

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

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            bool Result = false;
            bool Archived = false;
            if (steamIDUser == SteamEmulator.SteamID)
            {
                MutexHelper.Wait("Achievements", delegate
                {
                    var achievement = Achievements.Find(a => a.Name == pchName);
                    if (achievement != null)
                    {
                        Archived = achievement.Earned;
                        pbAchieved = false;
                        Result = true;
                    }
                });
            }
            else
            {
                // TODO: Request through socket
            }
            return Result;
        }

        public bool GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            return false;
        }

        public bool ResetAllStats(bool bAchievementsToo)
        {
            Write($"ResetAllStats");
            PlayerStats.Clear();
            if (bAchievementsToo)
                Achievements.Clear();
            IPCManager.SendResetAllStats(bAchievementsToo);
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
                NumberOfCurrentPlayers_t data = new NumberOfCurrentPlayers_t()
                {
                    m_bSuccess = 1,
                    m_cPlayers = SteamFriends.Instance.Users.Count
                };

                Write($"GetNumberOfCurrentPlayers {SteamFriends.Instance.Users.Count}");

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

        public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetNextMostAchievedAchievementInfo");
            return -1;
        }

        public bool GetAchievementAchievedPercent(string pchName, float pflPercent)
        {
            Write($"GetAchievementAchievedPercent");
            return false;
        }

        public SteamAPICall_t RequestGlobalStats(int nHistoryDays)
        {
            try
            {
                Write($"RequestGlobalStats {nHistoryDays} days");
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

        public uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
        {
            Write($"GetGlobalStatHistory {pchStatName}");
            return 0;
        }

        public bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
        {
            Write($"GetAchievementProgressLimits");
            return false;
        }

    }
}