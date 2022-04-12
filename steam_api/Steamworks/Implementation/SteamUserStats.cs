using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUserStats : ISteamInterface
    {
        public SteamUserStats()
        {
            InterfaceVersion = "SteamUserStats";
        }

        public bool RequestCurrentStats()
        {
            Write($"RequestCurrentStats");
            return false;
        }

        public bool GetStat(string pchName, uint pData)
        {
            Write($"GetStat");
            return false;
        }

        public bool SetStat(string pchName, uint nData)
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

        public bool StoreStats()
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

        public ulong RequestUserStats(ulong steamIDUser)
        {
            Write($"RequestUserStats");
            return default;
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

        public ulong FindOrCreateLeaderboard(string pchLeaderboardName, int eLeaderboardSortMethod, int eLeaderboardDisplayType)
        {
            Write($"FindOrCreateLeaderboard");
            return default;
        }

        public ulong FindLeaderboard(string pchLeaderboardName)
        {
            Write($"FindLeaderboard");
            return default;
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

        public ulong DownloadLeaderboardEntries(ulong hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries");
            return default;
        }

        public ulong DownloadLeaderboardEntriesForUsers(ulong hSteamLeaderboard, ulong prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers");
            return default;
        }

        public bool GetDownloadedLeaderboardEntry(ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
        {
            Write($"GetDownloadedLeaderboardEntry");
            return false;
        }

        public ulong UploadLeaderboardScore(ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
        {
            Write($"UploadLeaderboardScore");
            return default;
        }

        public ulong AttachLeaderboardUGC(ulong hSteamLeaderboard, ulong hUGC)
        {
            Write($"AttachLeaderboardUGC");
            return default;
        }

        public ulong GetNumberOfCurrentPlayers()
        {
            Write($"GetNumberOfCurrentPlayers");
            return default;
        }

        public ulong RequestGlobalAchievementPercentages()
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

        public ulong RequestGlobalStats(int nHistoryDays)
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
    }
}