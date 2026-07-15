using SKYNET.Managers;
using System;
using SKYNET.Helpers;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    using SteamAPICall_t = System.UInt64;
    public class SteamAPI_ISteamUserStats
    {
        static SteamAPI_ISteamUserStats()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_RequestCurrentStats(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_RequestCurrentStats");
            return SteamEmulator.SteamUserStats.RequestCurrentStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetStatInt32(IntPtr _, string pchName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetStatInt32");
            return SteamEmulator.SteamUserStats.GetStatInt32(pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetStatFloat(IntPtr _, string pchName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetStatFloat");
            return SteamEmulator.SteamUserStats.GetStatFloat(pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_SetStatInt32(IntPtr _, string pchName, int nData)
        {
            Write("SteamAPI_ISteamUserStats_SetStatInt32");
            return SteamEmulator.SteamUserStats.SetStat(pchName, unchecked((uint)nData));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_SetStatFloat(IntPtr _, string pchName, float fData)
        {
            Write("SteamAPI_ISteamUserStats_SetStatFloat");
            return SteamEmulator.SteamUserStats.SetStat(pchName, (uint)fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_UpdateAvgRateStat(IntPtr _, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("SteamAPI_ISteamUserStats_UpdateAvgRateStat");
            return SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievement(IntPtr _, string pchName, IntPtr pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievement");
            return SteamEmulator.SteamUserStats.GetAchievement(pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_SetAchievement(IntPtr _, string pchName)
        {
            Write("SteamAPI_ISteamUserStats_SetAchievement");
            return SteamEmulator.SteamUserStats.SetAchievement(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_ClearAchievement(IntPtr _, string pchName)
        {
            Write("SteamAPI_ISteamUserStats_ClearAchievement");
            return SteamEmulator.SteamUserStats.ClearAchievement(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime(IntPtr _, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime");
            return SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, pbAchieved, punUnlockTime);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_StoreStats(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_StoreStats");
            return SteamEmulator.SteamUserStats.StoreStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetAchievementIcon(IntPtr _, string pchName)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementIcon");
            return SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute(IntPtr _, string pchName, string pchKey)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute");
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_IndicateAchievementProgress(IntPtr _, string pchName, uint nCurProgress, uint nMaxProgress)
        {
            Write("SteamAPI_ISteamUserStats_IndicateAchievementProgress");
            return SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUserStats_GetNumAchievements(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_GetNumAchievements");
            return SteamEmulator.SteamUserStats.GetNumAchievements();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamUserStats_GetAchievementName(IntPtr _, uint iAchievement)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementName");
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementName(iAchievement));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestUserStats(IntPtr _, ulong steamIDUser)
        {
            Write("SteamAPI_ISteamUserStats_RequestUserStats");
            return SteamEmulator.SteamUserStats.RequestUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserStatInt32(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetUserStatInt32");
            return SteamEmulator.SteamUserStats.GetUserStatInt32(steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserStatFloat(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetUserStatFloat");
            return SteamEmulator.SteamUserStats.GetUserStatFloat(steamIDUser, pchName, pData);
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetUserAchievement");
            return SteamEmulator.SteamUserStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            Write("SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime");
            return SteamEmulator.SteamUserStats.GetUserAchievementAndUnlockTime(steamIDUser, pchName, pbAchieved, punUnlockTime);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_ResetAllStats(IntPtr _, bool bAchievementsToo)
        {
            Write("SteamAPI_ISteamUserStats_ResetAllStats");
            return SteamEmulator.SteamUserStats.ResetAllStats(bAchievementsToo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_FindOrCreateLeaderboard(IntPtr _, string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
        {
            Write("SteamAPI_ISteamUserStats_FindOrCreateLeaderboard");
            return SteamEmulator.SteamUserStats.FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_FindLeaderboard(IntPtr _, string pchLeaderboardName)
        {
            Write("SteamAPI_ISteamUserStats_FindLeaderboard");
            return SteamEmulator.SteamUserStats.FindLeaderboard(pchLeaderboardName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamUserStats_GetLeaderboardName(IntPtr _, ulong hSteamLeaderboard)
        {
            Write("SteamAPI_ISteamUserStats_GetLeaderboardName");
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetLeaderboardEntryCount(IntPtr _, ulong hSteamLeaderboard)
        {
            Write("SteamAPI_ISteamUserStats_GetLeaderboardEntryCount");
            return SteamEmulator.SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetLeaderboardSortMethod(IntPtr _, ulong hSteamLeaderboard)
        {
            Write("SteamAPI_ISteamUserStats_GetLeaderboardSortMethod");
            return SteamEmulator.SteamUserStats.GetLeaderboardSortMethod(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetLeaderboardDisplayType(IntPtr _, ulong hSteamLeaderboard)
        {
            Write("SteamAPI_ISteamUserStats_GetLeaderboardDisplayType");
            return SteamEmulator.SteamUserStats.GetLeaderboardDisplayType(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntries(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntries");
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers(IntPtr _, ulong hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        {
            Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers");
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, prgUsers, cUsers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry(IntPtr _, ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
        {
            Write("SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry");
            return SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_UploadLeaderboardScore(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, int nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        {
            Write("SteamAPI_ISteamUserStats_UploadLeaderboardScore");
            return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, unchecked((uint)nScore), pScoreDetails, cScoreDetailsCount);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_AttachLeaderboardUGC(IntPtr _, ulong hSteamLeaderboard, ulong hUGC)
        {
            Write("SteamAPI_ISteamUserStats_AttachLeaderboardUGC");
            return SteamEmulator.SteamUserStats.AttachLeaderboardUGC(hSteamLeaderboard, hUGC);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers");
            return SteamEmulator.SteamUserStats.GetNumberOfCurrentPlayers();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages");
            return SteamEmulator.SteamUserStats.RequestGlobalAchievementPercentages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo(IntPtr _, IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo");
            return SteamEmulator.SteamUserStats.GetMostAchievedAchievementInfo(pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo(IntPtr _, int iIteratorPrevious, IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo");
            return SteamEmulator.SteamUserStats.GetNextMostAchievedAchievementInfo(iIteratorPrevious, pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementAchievedPercent(IntPtr _, string pchName, IntPtr pflPercent)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementAchievedPercent");
            return SteamEmulator.SteamUserStats.GetAchievementAchievedPercent(pchName, pflPercent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestGlobalStats(IntPtr _, int nHistoryDays)
        {
            Write("SteamAPI_ISteamUserStats_RequestGlobalStats");
            return SteamEmulator.SteamUserStats.RequestGlobalStats(nHistoryDays);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetGlobalStatInt64(IntPtr _, string pchStatName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStatInt64");
            return SteamEmulator.SteamUserStats.GetGlobalStatInt64(pchStatName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetGlobalStatDouble(IntPtr _, string pchStatName, IntPtr pData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStatDouble");
            return SteamEmulator.SteamUserStats.GetGlobalStatDouble(pchStatName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetGlobalStatHistoryInt64(IntPtr _, string pchStatName, IntPtr pData, uint cubData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStatHistoryInt64");
            return SteamEmulator.SteamUserStats.GetGlobalStatHistoryInt64(pchStatName, pData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetGlobalStatHistoryDouble(IntPtr _, string pchStatName, IntPtr pData, uint cubData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStatHistoryDouble");
            return SteamEmulator.SteamUserStats.GetGlobalStatHistoryDouble(pchStatName, pData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementProgressLimitsInt32(IntPtr _, string pchName, IntPtr pnMinProgress, IntPtr pnMaxProgress)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementProgressLimitsInt32");
            return SteamEmulator.SteamUserStats.GetAchievementProgressLimitsInt32(pchName, pnMinProgress, pnMaxProgress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementProgressLimitsFloat(IntPtr _, string pchName, IntPtr pfMinProgress, IntPtr pfMaxProgress)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementProgressLimitsFloat");
            return SteamEmulator.SteamUserStats.GetAchievementProgressLimitsFloat(pchName, pfMinProgress, pfMaxProgress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUserStats_v011()
        {
            Write("SteamAPI_SteamUserStats_v011");
            return InterfaceManager.FindOrCreateInterface("STEAMUSERSTATS_INTERFACE_VERSION011");
        }
            
        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
