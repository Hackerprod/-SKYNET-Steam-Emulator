using SKYNET;
using SKYNET.Managers;
using SKYNET.Steamworks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;
using SteamLeaderboard_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamUserStats
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_RequestCurrentStats(IntPtr _)
        {
            Write("SteamAPI_ISteamUserStats_RequestCurrentStats");
            return SteamEmulator.SteamUserStats.RequestCurrentStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetStat(IntPtr _, string pchName, ref int pData)
        {
            Write("SteamAPI_ISteamUserStats_GetStat");
            return SteamEmulator.SteamUserStats.GetStat(pchName, ref pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_SetStat(IntPtr _, string pchName, uint nData)
        {
            Write("SteamAPI_ISteamUserStats_SetStat");
            return SteamEmulator.SteamUserStats.SetStat(pchName, nData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_UpdateAvgRateStat(IntPtr _, string pchName, uint flCountThisSession, double dSessionLength)
        {
            Write("SteamAPI_ISteamUserStats_UpdateAvgRateStat");
            return SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievement(IntPtr _, string pchName, ref bool pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievement");
            return SteamEmulator.SteamUserStats.GetAchievement(pchName, ref pbAchieved);
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
        public static bool SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime(IntPtr _, ref string pchName, ref bool pbAchieved, uint punUnlockTime)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime");
            return SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, ref pbAchieved, ref punUnlockTime);
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
        public static string SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute(IntPtr _, string pchName, string pchKey)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute");
            return SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey);
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
        public static string SteamAPI_ISteamUserStats_GetAchievementName(IntPtr _, uint iAchievement)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementName");
            return SteamEmulator.SteamUserStats.GetAchievementName(iAchievement);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestUserStats(IntPtr _, ulong steamIDUser)
        {
            Write("SteamAPI_ISteamUserStats_RequestUserStats");
            return SteamEmulator.SteamUserStats.RequestUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserStat(IntPtr _, ulong steamIDUser, string pchName, uint pData)
        {
            Write("SteamAPI_ISteamUserStats_GetUserStat");
            return SteamEmulator.SteamUserStats.GetUserStat(steamIDUser, pchName, pData);
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetUserAchievement");
            return SteamEmulator.SteamUserStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime(IntPtr _, ulong steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
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
        public static SteamAPICall_t SteamAPI_ISteamUserStats_FindOrCreateLeaderboard(IntPtr _, string pchLeaderboardName, int eLeaderboardSortMethod, int eLeaderboardDisplayType)
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
        public static string SteamAPI_ISteamUserStats_GetLeaderboardName(IntPtr _, ulong hSteamLeaderboard)
        {
            Write("SteamAPI_ISteamUserStats_GetLeaderboardName");
            return SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard);
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
        public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntries(IntPtr _, ulong hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntries");
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers(IntPtr _, ulong hSteamLeaderboard, ulong prgUsers, int cUsers)
        {
            Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers");
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, prgUsers, cUsers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry(IntPtr _, ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
        {
            Write("SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry");
            return SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUserStats_UploadLeaderboardScore(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
        {
            Write("SteamAPI_ISteamUserStats_UploadLeaderboardScore");
            return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
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
        public static int SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo(IntPtr _, string pchName, uint unNameBufLen, uint pflPercent, bool pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo");
            return SteamEmulator.SteamUserStats.GetMostAchievedAchievementInfo(pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo(IntPtr _, int iIteratorPrevious, string pchName, uint unNameBufLen, uint pflPercent, bool pbAchieved)
        {
            Write("SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo");
            return SteamEmulator.SteamUserStats.GetNextMostAchievedAchievementInfo(iIteratorPrevious, pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementAchievedPercent(IntPtr _, string pchName, uint pflPercent)
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
        public static bool SteamAPI_ISteamUserStats_GetGlobalStat(IntPtr _, string pchStatName, uint pData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStat");
            return SteamEmulator.SteamUserStats.GetGlobalStat(pchStatName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUserStats_GetGlobalStatHistory(IntPtr _, string pchStatName, uint pData, uint cubData)
        {
            Write("SteamAPI_ISteamUserStats_GetGlobalStatHistory");
            return SteamEmulator.SteamUserStats.GetGlobalStatHistory(pchStatName, pData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUserStats_GetAchievementProgressLimits(IntPtr _, string pchName, uint pnMinProgress, uint pnMaxProgress)
        {
            Write("SteamAPI_ISteamUserStats_GetAchievementProgressLimits");
            return SteamEmulator.SteamUserStats.GetAchievementProgressLimits(pchName, pnMinProgress, pnMaxProgress);
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

