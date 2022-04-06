using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;
using System;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUserStats : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamUserStats()
        {
            InterfaceVersion = "SteamUserStats";
        }

        public bool RequestCurrentStats(IntPtr _)
        {
            Write($"RequestCurrentStats");
            return false;
        }

        public bool GetStat(IntPtr _, string pchName, uint pData)
        {
            Write($"GetStat");
            return false;
        }

        public bool SetStat(IntPtr _, string pchName, uint nData)
        {
            Write($"SetStat");
            return false;
        }

        public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateAvgRateStat");
            return false;
        }

        public bool GetAchievement(string pchName, bool pbAchieved)
        {
            Write($"GetAchievement");
            return false;
        }

        public bool SetAchievement(string pchName)
        {
            Write($"SetAchievement");
            return false;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement");
            return false;
        }

        public bool GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetAchievementAndUnlockTime");
            return false;
        }

        public bool StoreStats(IntPtr _)
        {
            Write($"StoreStats");
            return false;
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
            return false;
        }

        public uint GetNumAchievements(IntPtr _)
        {
            Write($"GetNumAchievements");
            return 0;
        }

        public string GetAchievementName(uint iAchievement)
        {
            Write($"GetAchievementName");
            return "";
        }

        public SteamAPICall_t RequestUserStats(IntPtr steamIDUser)
        {
            Write($"RequestUserStats");
            return default;
        }

        public bool GetUserStat(IntPtr _, IntPtr steamIDUser, string pchName, uint pData)
        {
            Write($"GetUserStat");
            return false;
        }

        public bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement");
            return false;
        }

        public bool GetUserAchievementAndUnlockTime(IntPtr steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            return false;
        }

        public bool ResetAllStats(bool bAchievementsToo)
        {
            Write($"ResetAllStats");
            return false;
        }

        public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, int eLeaderboardDisplayType)
        {
            Write($"FindOrCreateLeaderboard");
            return default;
        }

        public SteamAPICall_t FindLeaderboard(string pchLeaderboardName)
        {
            Write($"FindLeaderboard");
            return default;
        }

        public string GetLeaderboardName(IntPtr hSteamLeaderboard)
        {
            Write($"GetLeaderboardName");
            return "";
        }

        public int GetLeaderboardEntryCount(IntPtr hSteamLeaderboard)
        {
            Write($"GetLeaderboardEntryCount");
            return 0;
        }

        public ELeaderboardSortMethod GetLeaderboardSortMethod(IntPtr hSteamLeaderboard)
        {
            Write($"GetLeaderboardSortMethod");
            return default;
        }

        public int GetLeaderboardDisplayType(IntPtr hSteamLeaderboard)
        {
            Write($"GetLeaderboardDisplayType");
            return 0;
        }

        public SteamAPICall_t DownloadLeaderboardEntries(IntPtr hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries");
            return default;
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(IntPtr hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers");
            return default;
        }

        public bool GetDownloadedLeaderboardEntry(IntPtr hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
        {
            Write($"GetDownloadedLeaderboardEntry");
            return false;
        }

        public SteamAPICall_t UploadLeaderboardScore(IntPtr hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
        {
            Write($"UploadLeaderboardScore");
            return default;
        }

        public SteamAPICall_t AttachLeaderboardUGC(IntPtr hSteamLeaderboard, UGCHandle_t hUGC)
        {
            Write($"AttachLeaderboardUGC");
            return default;
        }

        public SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _)
        {
            Write($"GetNumberOfCurrentPlayers");
            return default;
        }

        public SteamAPICall_t RequestGlobalAchievementPercentages(IntPtr _)
        {
            Write($"RequestGlobalAchievementPercentages");
            return default;
        }

        public int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetMostAchievedAchievementInfo");
            return 0;
        }

        public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetNextMostAchievedAchievementInfo");
            return 0;
        }

        public bool GetAchievementAchievedPercent(string pchName, float pflPercent)
        {
            Write($"GetAchievementAchievedPercent");
            return false;
        }

        public SteamAPICall_t RequestGlobalStats(int nHistoryDays)
        {
            Write($"RequestGlobalStats");
            return default;
        }

        public bool GetGlobalStat(string pchStatName, uint pData)
        {
            Write($"GetGlobalStat");
            return false;
        }

        public uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
        {
            Write($"GetGlobalStatHistory");
            return 0;
        }

        public bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
        {
            Write($"GetAchievementProgressLimits");
            return false;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}