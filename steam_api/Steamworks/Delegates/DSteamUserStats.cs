using Core.Interface;
using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamUserStats")]
    public class DSteamUserStats 
    {

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RequestCurrentStats(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetStat(IntPtr _, string pchName, uint pData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetStat(IntPtr _, string pchName, uint nData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAchievement(string pchName, bool pbAchieved);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetAchievement(string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ClearAchievement(string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool StoreStats(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAchievementIcon(string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetAchievementDisplayAttribute(string pchName, string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumAchievements(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetAchievementName(uint iAchievement);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestUserStats(IntPtr steamIDUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserStat(IntPtr _, IntPtr steamIDUser, string pchName, uint pData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserAchievementAndUnlockTime(IntPtr steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ResetAllStats(bool bAchievementsToo);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, int eLeaderboardDisplayType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t FindLeaderboard(string pchLeaderboardName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetLeaderboardName(IntPtr hSteamLeaderboard);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLeaderboardEntryCount(IntPtr hSteamLeaderboard);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ELeaderboardSortMethod GetLeaderboardSortMethod(IntPtr hSteamLeaderboard);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLeaderboardDisplayType(IntPtr hSteamLeaderboard);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DownloadLeaderboardEntries(IntPtr hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DownloadLeaderboardEntriesForUsers(IntPtr hSteamLeaderboard, IntPtr prgUsers, int cUsers);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetDownloadedLeaderboardEntry(IntPtr hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UploadLeaderboardScore(IntPtr hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AttachLeaderboardUGC(IntPtr hSteamLeaderboard, UGCHandle_t hUGC);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestGlobalAchievementPercentages(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAchievementAchievedPercent(string pchName, float pflPercent);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestGlobalStats(int nHistoryDays);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetGlobalStat(string pchStatName, uint pData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress);
    }
}
