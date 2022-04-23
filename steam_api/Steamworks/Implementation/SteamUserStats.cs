using SKYNET;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using SteamLeaderboard_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUserStats : ISteamInterface
    {
        private List<Leaderboard> Leaderboards;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        internal class Leaderboard
        {
            public string Name { get; set; }
            public ELeaderboardSortMethod ShortMethod { get; set; }
            public ELeaderboardDisplayType DisplayType { get; set; }
            public SteamLeaderboard_t SteamLeaderboard { get; set; }
        }

        public SteamUserStats()
        {
            InterfaceVersion = "SteamUserStats";
            Leaderboards = new List<Leaderboard>();
        }

        public bool RequestCurrentStats()
        {
            try
            {
                Write($"RequestCurrentStats");
                //UserStatsReceived_t data = new UserStatsReceived_t()
                //{
                //    m_nGameID = SteamEmulator.GameID,
                //    m_eResult = SKYNET.Types.EResult.k_EResultOK,
                //    m_steamIDUser = SteamEmulator.SteamId
                //};
                //CallbackManager.AddCallbackResult(data, UserStatsReceived_t.k_iCallback);
                return true;
            }
            catch (Exception ex)
            {
                Write($"RequestCurrentStats {ex}");
                return false;
            }
        }

        public bool GetStat(string pchName, ref int pData)
        {
            Write($"GetStat {pchName}");
            if (string.IsNullOrEmpty(pchName) || pData == 0) return false;
            return false;
        }

        public bool GetStat(string pchName, ref float pData)
        {
            Write($"GetStat {pchName}");
            if (string.IsNullOrEmpty(pchName) || pData == 0) return false;
            return false;
        }

        public bool SetStat(string pchName, uint nData)
        {
            Write($"SetStat {pchName}");
            return false;
        }

        public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateAvgRateStat {pchName}");
            return false;
        }

        public bool GetAchievement(string pchName, bool pbAchieved)
        {
            Write($"GetAchievement {pchName}");
            return false;
        }

        public bool SetAchievement(string pchName)
        {
            Write($"SetAchievement {pchName}");
            return false;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement {pchName}");
            return false;
        }

        public bool GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetAchievementAndUnlockTime {pchName}");
            return false;
        }

        public bool StoreStats()
        {
            try
            {
                Write($"StoreStats");
                //UserStatsStored_t data = new UserStatsStored_t()
                //{
                //    m_nGameID = SteamEmulator.GameID,
                //    m_eResult = SKYNET.Types.EResult.k_EResultOK
                //};
                //CallbackManager.AddCallbackResult(data, UserStatsStored_t.k_iCallback);
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
            // UserAchievementStored_t
            // CallbackManager.AddCallbackResult(data);
            return false;
        }

        public uint GetNumAchievements()
        {
            Write($"GetNumAchievements");
            return 0;
        }

        public string GetAchievementName(uint iAchievement)
        {
            Write($"GetAchievementName");
            return "";
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            try
            {
                Write($"RequestUserStats");
                //UserStatsReceived_t data = new UserStatsReceived_t()
                //{
                //    m_nGameID = SteamEmulator.GameID,
                //    m_eResult = EResult.k_EResultOK,
                //    m_steamIDUser = steamIDUser
                //};
                //return CallbackManager.AddCallbackResult(data, UserStatsReceived_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"RequestUserStats {ex}");
            }
            return k_uAPICallInvalid;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, uint pData)
        {
            Write($"GetUserStat");
            return false;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement");
            return false;
        }

        public bool GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            return false;
        }

        public bool ResetAllStats(bool bAchievementsToo)
        {
            Write($"ResetAllStats");
            return false;
        }

        public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, int eLeaderboardSortMethod, int eLeaderboardDisplayType)
        {
            try
            {
                Write($"FindOrCreateLeaderboard");

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

        public SteamAPICall_t DownloadLeaderboardEntries(ulong hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(ulong hSteamLeaderboard, ulong prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public bool GetDownloadedLeaderboardEntry(ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
        {
            Write($"GetDownloadedLeaderboardEntry");
            return false;
        }

        public SteamAPICall_t UploadLeaderboardScore(ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
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
                Write($"GetNumberOfCurrentPlayers");
                //NumberOfCurrentPlayers_t data = new NumberOfCurrentPlayers_t()
                //{
                //    m_bSuccess = 1,
                //    m_cPlayers = 0
                //};
                //return CallbackManager.AddCallbackResult(data, NumberOfCurrentPlayers_t.k_iCallback);
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
                Write($"RequestGlobalStats");
                //GlobalStatsReceived_t data = new GlobalStatsReceived_t()
                //{
                //    m_eResult = EResult.k_EResultOK,
                //    m_nGameID = SteamEmulator.GameID
                //};
                //return CallbackManager.AddCallbackResult(data, GlobalStatsReceived_t.k_iCallback);
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