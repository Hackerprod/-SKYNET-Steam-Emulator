using SKYNET;
using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//public class SteamAPI_ISteamUserStats : BaseCalls
//{
//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_RequestCurrentStats()
//    {
//        Write("SteamAPI_ISteamUserStats_RequestCurrentStats");
//        return SteamEmulator.SteamUserStats.RequestCurrentStats();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetStat(string pchName, uint pData)
//    {
//        Write("SteamAPI_ISteamUserStats_GetStat");
//        return SteamEmulator.SteamUserStats.GetStat(pchName, pData);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_SetStat(string pchName, uint nData)
//    {
//        Write("SteamAPI_ISteamUserStats_SetStat");
//        return SteamEmulator.SteamUserStats.SetStat(pchName, nData);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_UpdateAvgRateStat(string pchName, uint flCountThisSession, double dSessionLength)
//    {
//        Write("SteamAPI_ISteamUserStats_UpdateAvgRateStat");
//        return SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetAchievement(string pchName, bool pbAchieved)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievement");
//        return SteamEmulator.SteamUserStats.GetAchievement(pchName, pbAchieved);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_SetAchievement(string pchName)
//    {
//        Write("SteamAPI_ISteamUserStats_SetAchievement");
//        return SteamEmulator.SteamUserStats.SetAchievement(pchName);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_ClearAchievement(string pchName)
//    {
//        Write("SteamAPI_ISteamUserStats_ClearAchievement");
//        return SteamEmulator.SteamUserStats.ClearAchievement(pchName);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime");
//        return SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, pbAchieved, punUnlockTime);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_StoreStats()
//    {
//        Write("SteamAPI_ISteamUserStats_StoreStats");
//        return SteamEmulator.SteamUserStats.StoreStats();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamUserStats_GetAchievementIcon(string pchName)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementIcon");
//        return SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static string SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute(string pchName, string pchKey)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute");
//        return SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
//    {
//        Write("SteamAPI_ISteamUserStats_IndicateAchievementProgress");
//        return SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static uint SteamAPI_ISteamUserStats_GetNumAchievements()
//    {
//        Write("SteamAPI_ISteamUserStats_GetNumAchievements");
//        return SteamEmulator.SteamUserStats.GetNumAchievements();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static string SteamAPI_ISteamUserStats_GetAchievementName(uint iAchievement)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementName");
//        return SteamEmulator.SteamUserStats.GetAchievementName(iAchievement);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestUserStats(IntPtr steamIDUser)
//    {
//        Write("SteamAPI_ISteamUserStats_RequestUserStats");
//        return SteamEmulator.SteamUserStats.RequestUserStats(steamIDUser);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetUserStat(IntPtr steamIDUser, string pchName, uint pData)
//    {
//        Write("SteamAPI_ISteamUserStats_GetUserStat");
//        return SteamEmulator.SteamUserStats.GetUserStat(steamIDUser, pchName, pData);
//    }


//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
//    {
//        Write("SteamAPI_ISteamUserStats_GetUserAchievement");
//        return SteamEmulator.SteamUserStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime(IntPtr steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
//    {
//        Write("SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime");
//        return SteamEmulator.SteamUserStats.GetUserAchievementAndUnlockTime(steamIDUser, pchName, pbAchieved, punUnlockTime);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_ResetAllStats(bool bAchievementsToo)
//    {
//        Write("SteamAPI_ISteamUserStats_ResetAllStats");
//        return SteamEmulator.SteamUserStats.ResetAllStats(bAchievementsToo);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, int eLeaderboardDisplayType)
//    {
//        Write("SteamAPI_ISteamUserStats_FindOrCreateLeaderboard");
//        return SteamEmulator.SteamUserStats.FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_FindLeaderboard(string pchLeaderboardName)
//    {
//        Write("SteamAPI_ISteamUserStats_FindLeaderboard");
//        return SteamEmulator.SteamUserStats.FindLeaderboard(pchLeaderboardName);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static string SteamAPI_ISteamUserStats_GetLeaderboardName(IntPtr hSteamLeaderboard)
//    {
//        Write("SteamAPI_ISteamUserStats_GetLeaderboardName");
//        return SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamUserStats_GetLeaderboardEntryCount(IntPtr hSteamLeaderboard)
//    {
//        Write("SteamAPI_ISteamUserStats_GetLeaderboardEntryCount");
//        return SteamEmulator.SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static ELeaderboardSortMethod SteamAPI_ISteamUserStats_GetLeaderboardSortMethod(IntPtr hSteamLeaderboard)
//    {
//        Write("SteamAPI_ISteamUserStats_GetLeaderboardSortMethod");
//        return SteamEmulator.SteamUserStats.GetLeaderboardSortMethod(hSteamLeaderboard);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamUserStats_GetLeaderboardDisplayType(IntPtr hSteamLeaderboard)
//    {
//        Write("SteamAPI_ISteamUserStats_GetLeaderboardDisplayType");
//        return SteamEmulator.SteamUserStats.GetLeaderboardDisplayType(hSteamLeaderboard);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntries(IntPtr hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
//    {
//        Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntries");
//        return SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers(IntPtr hSteamLeaderboard, IntPtr prgUsers, int cUsers)
//    {
//        Write("SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers");
//        return SteamEmulator.SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, prgUsers, cUsers);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry(IntPtr hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
//    {
//        Write("SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry");
//        return SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_UploadLeaderboardScore(IntPtr hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
//    {
//        Write("SteamAPI_ISteamUserStats_UploadLeaderboardScore");
//        return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_AttachLeaderboardUGC(IntPtr hSteamLeaderboard, UGCHandle_t hUGC)
//    {
//        Write("SteamAPI_ISteamUserStats_AttachLeaderboardUGC");
//        return SteamEmulator.SteamUserStats.AttachLeaderboardUGC(hSteamLeaderboard, hUGC);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers()
//    {
//        Write("SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers");
//        return SteamEmulator.SteamUserStats.GetNumberOfCurrentPlayers();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages()
//    {
//        Write("SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages");
//        return SteamEmulator.SteamUserStats.RequestGlobalAchievementPercentages();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, uint pflPercent, bool pbAchieved)
//    {
//        Write("SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo");
//        return SteamEmulator.SteamUserStats.GetMostAchievedAchievementInfo(pchName, unNameBufLen, pflPercent, pbAchieved);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, uint pflPercent, bool pbAchieved)
//    {
//        Write("SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo");
//        return SteamEmulator.SteamUserStats.GetNextMostAchievedAchievementInfo(iIteratorPrevious, pchName, unNameBufLen, pflPercent, pbAchieved);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetAchievementAchievedPercent(string pchName, uint pflPercent)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementAchievedPercent");
//        return SteamEmulator.SteamUserStats.GetAchievementAchievedPercent(pchName, pflPercent);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamUserStats_RequestGlobalStats(int nHistoryDays)
//    {
//        Write("SteamAPI_ISteamUserStats_RequestGlobalStats");
//        return SteamEmulator.SteamUserStats.RequestGlobalStats(nHistoryDays);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetGlobalStat(string pchStatName, uint pData)
//    {
//        Write("SteamAPI_ISteamUserStats_GetGlobalStat");
//        return SteamEmulator.SteamUserStats.GetGlobalStat(pchStatName, pData);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static uint SteamAPI_ISteamUserStats_GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
//    {
//        Write("SteamAPI_ISteamUserStats_GetGlobalStatHistory");
//        return SteamEmulator.SteamUserStats.GetGlobalStatHistory(pchStatName, pData, cubData);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamUserStats_GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
//    {
//        Write("SteamAPI_ISteamUserStats_GetAchievementProgressLimits");
//        return SteamEmulator.SteamUserStats.GetAchievementProgressLimits(pchName, pnMinProgress, pnMaxProgress);
//    }


//}

