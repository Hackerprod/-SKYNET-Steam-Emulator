using SKYNET.Helpers;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION001",
        "GetNumStats", "GetStatName", "GetStatType", "GetNumAchievements__V001",
        "GetAchievementName__V001", "GetNumGroupAchievements", "GetGroupAchievementName", "RequestCurrentStats__V001",
        "GetStatFloat__V001", "GetStatInt32__V001", "SetStatFloat__V001", "SetStatInt32__V001",
        "UpdateAvgRateStat__V001", "GetAchievement__V001", "GetGroupAchievement", "SetAchievement__V001",
        "SetGroupAchievement", "StoreStats__V001", "ClearAchievement__V001", "ClearGroupAchievement",
        "GetAchievementIcon__V001", "GetAchievementDisplayAttribute__V001")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION002",
        "GetNumStats", "GetStatName", "GetStatType", "GetNumAchievements__V001",
        "GetAchievementName__V001", "RequestCurrentStats__V001", "GetStatFloat__V001", "GetStatInt32__V001",
        "SetStatFloat__V001", "SetStatInt32__V001", "UpdateAvgRateStat__V001", "GetAchievement__V001",
        "SetAchievement__V001", "ClearAchievement__V001", "StoreStats__V001", "GetAchievementIcon__V001",
        "GetAchievementDisplayAttribute__V001", "IndicateAchievementProgress__V001")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION003",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "StoreStats", "GetAchievementIcon", "GetAchievementDisplayAttribute",
        "IndicateAchievementProgress")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION004",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "StoreStats", "GetAchievementIcon", "GetAchievementDisplayAttribute",
        "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat", "GetUserStatInt32",
        "GetUserAchievement")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION005",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "StoreStats", "GetAchievementIcon", "GetAchievementDisplayAttribute",
        "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat", "GetUserStatInt32",
        "GetUserAchievement", "ResetAllStats", "FindOrCreateLeaderboard", "FindLeaderboard",
        "GetLeaderboardName", "GetLeaderboardEntryCount", "GetLeaderboardSortMethod", "GetLeaderboardDisplayType",
        "DownloadLeaderboardEntries", "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore__V005")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION006",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "StoreStats", "GetAchievementIcon", "GetAchievementDisplayAttribute",
        "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat", "GetUserStatInt32",
        "GetUserAchievement", "ResetAllStats", "FindOrCreateLeaderboard", "FindLeaderboard",
        "GetLeaderboardName", "GetLeaderboardEntryCount", "GetLeaderboardSortMethod", "GetLeaderboardDisplayType",
        "DownloadLeaderboardEntries", "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore", "GetNumberOfCurrentPlayers")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION007",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32__V007", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat",
        "GetUserStatInt32", "GetUserAchievement", "GetUserAchievementAndUnlockTime", "ResetAllStats",
        "FindOrCreateLeaderboard", "FindLeaderboard", "GetLeaderboardName", "GetLeaderboardEntryCount",
        "GetLeaderboardSortMethod", "GetLeaderboardDisplayType", "DownloadLeaderboardEntries", "GetDownloadedLeaderboardEntry",
        "UploadLeaderboardScore", "GetNumberOfCurrentPlayers")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION008",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat",
        "GetUserStatInt32", "GetUserAchievement", "GetUserAchievementAndUnlockTime", "ResetAllStats",
        "FindOrCreateLeaderboard", "FindLeaderboard", "GetLeaderboardName", "GetLeaderboardEntryCount",
        "GetLeaderboardSortMethod", "GetLeaderboardDisplayType", "DownloadLeaderboardEntries", "GetDownloadedLeaderboardEntry",
        "UploadLeaderboardScore", "AttachLeaderboardUGC", "GetNumberOfCurrentPlayers")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION009",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat",
        "GetUserStatInt32", "GetUserAchievement", "GetUserAchievementAndUnlockTime", "ResetAllStats",
        "FindOrCreateLeaderboard", "FindLeaderboard", "GetLeaderboardName", "GetLeaderboardEntryCount",
        "GetLeaderboardSortMethod", "GetLeaderboardDisplayType", "DownloadLeaderboardEntries", "DownloadLeaderboardEntriesForUsers",
        "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore", "AttachLeaderboardUGC", "GetNumberOfCurrentPlayers")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION010",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "RequestUserStats", "GetUserStatFloat",
        "GetUserStatInt32", "GetUserAchievement", "GetUserAchievementAndUnlockTime", "ResetAllStats",
        "FindOrCreateLeaderboard", "FindLeaderboard", "GetLeaderboardName", "GetLeaderboardEntryCount",
        "GetLeaderboardSortMethod", "GetLeaderboardDisplayType", "DownloadLeaderboardEntries", "DownloadLeaderboardEntriesForUsers",
        "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore", "AttachLeaderboardUGC", "GetNumberOfCurrentPlayers",
        "RequestGlobalAchievementPercentages", "GetMostAchievedAchievementInfo", "GetNextMostAchievedAchievementInfo", "GetAchievementAchievedPercent",
        "RequestGlobalStats", "GetGlobalStatDouble", "GetGlobalStatInt64", "GetGlobalStatHistoryDouble",
        "GetGlobalStatHistoryInt64")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION011",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "GetNumAchievements", "GetAchievementName",
        "RequestUserStats", "GetUserStatFloat", "GetUserStatInt32", "GetUserAchievement",
        "GetUserAchievementAndUnlockTime", "ResetAllStats", "FindOrCreateLeaderboard", "FindLeaderboard",
        "GetLeaderboardName", "GetLeaderboardEntryCount", "GetLeaderboardSortMethod", "GetLeaderboardDisplayType",
        "DownloadLeaderboardEntries", "DownloadLeaderboardEntriesForUsers", "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore",
        "AttachLeaderboardUGC", "GetNumberOfCurrentPlayers", "RequestGlobalAchievementPercentages", "GetMostAchievedAchievementInfo",
        "GetNextMostAchievedAchievementInfo", "GetAchievementAchievedPercent", "RequestGlobalStats", "GetGlobalStatDouble",
        "GetGlobalStatInt64", "GetGlobalStatHistoryDouble", "GetGlobalStatHistoryInt64")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION012",
        "RequestCurrentStats", "GetStatFloat", "GetStatInt32", "SetStatFloat",
        "SetStatInt32", "UpdateAvgRateStat", "GetAchievement", "SetAchievement",
        "ClearAchievement", "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon",
        "GetAchievementDisplayAttribute", "IndicateAchievementProgress", "GetNumAchievements", "GetAchievementName",
        "RequestUserStats", "GetUserStatFloat", "GetUserStatInt32", "GetUserAchievement",
        "GetUserAchievementAndUnlockTime", "ResetAllStats", "FindOrCreateLeaderboard", "FindLeaderboard",
        "GetLeaderboardName", "GetLeaderboardEntryCount", "GetLeaderboardSortMethod", "GetLeaderboardDisplayType",
        "DownloadLeaderboardEntries", "DownloadLeaderboardEntriesForUsers", "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore",
        "AttachLeaderboardUGC", "GetNumberOfCurrentPlayers", "RequestGlobalAchievementPercentages", "GetMostAchievedAchievementInfo",
        "GetNextMostAchievedAchievementInfo", "GetAchievementAchievedPercent", "RequestGlobalStats", "GetGlobalStatDouble",
        "GetGlobalStatInt64", "GetGlobalStatHistoryDouble", "GetGlobalStatHistoryInt64", "GetAchievementProgressLimitsFloat",
        "GetAchievementProgressLimitsInt32")]
    [InterfaceLayout("STEAMUSERSTATS_INTERFACE_VERSION013",
        "GetStatFloat", "GetStatInt32", "SetStatFloat", "SetStatInt32",
        "UpdateAvgRateStat", "GetAchievement", "SetAchievement", "ClearAchievement",
        "GetAchievementAndUnlockTime", "StoreStats", "GetAchievementIcon", "GetAchievementDisplayAttribute",
        "IndicateAchievementProgress", "GetNumAchievements", "GetAchievementName", "RequestUserStats",
        "GetUserStatFloat", "GetUserStatInt32", "GetUserAchievement", "GetUserAchievementAndUnlockTime",
        "ResetAllStats", "FindOrCreateLeaderboard", "FindLeaderboard", "GetLeaderboardName",
        "GetLeaderboardEntryCount", "GetLeaderboardSortMethod", "GetLeaderboardDisplayType", "DownloadLeaderboardEntries",
        "DownloadLeaderboardEntriesForUsers", "GetDownloadedLeaderboardEntry", "UploadLeaderboardScore", "AttachLeaderboardUGC",
        "GetNumberOfCurrentPlayers", "RequestGlobalAchievementPercentages", "GetMostAchievedAchievementInfo", "GetNextMostAchievedAchievementInfo",
        "GetAchievementAchievedPercent", "RequestGlobalStats", "GetGlobalStatDouble", "GetGlobalStatInt64",
        "GetGlobalStatHistoryDouble", "GetGlobalStatHistoryInt64", "GetAchievementProgressLimitsFloat", "GetAchievementProgressLimitsInt32")]
    public class SteamUserStatsInterface : ISteamInterface
    {
        public bool RequestCurrentStats(IntPtr _) => SteamEmulator.SteamUserStats.RequestCurrentStats();

        public bool GetStatInt32(IntPtr _, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetStatInt32(pchName, pData);
        }

        public bool GetStatFloat(IntPtr _, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetStatFloat(pchName, pData);
        }

        public bool SetStatInt32__V007(IntPtr _, string pchName, int nData) => SteamEmulator.SteamUserStats.SetStat(pchName, unchecked((uint)nData));

        public bool SetStatInt32(IntPtr _, string pchName, int nData)
        {
            return SteamEmulator.SteamUserStats.SetStat(pchName, (uint)nData);
        }

        public bool SetStatFloat(IntPtr _, string pchName, float fData)
        {
            return SteamEmulator.SteamUserStats.SetStat(pchName, fData);
        }

        public bool UpdateAvgRateStat(IntPtr _, string pchName, float flCountThisSession, double dSessionLength)
        {
            return SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
        }

        public bool GetAchievement(IntPtr _, string pchName, IntPtr pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetAchievement(pchName, pbAchieved);
        }

        public bool SetAchievement(IntPtr _, string pchName)
        {
            return SteamEmulator.SteamUserStats.SetAchievement(pchName);
        }

        public bool ClearAchievement(IntPtr _, string pchName)
        {
            return SteamEmulator.SteamUserStats.ClearAchievement(pchName);
        }

        public bool GetAchievementAndUnlockTime(IntPtr _, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            return SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, pbAchieved, punUnlockTime);
        }

        public bool StoreStats(IntPtr _)
        {
            return SteamEmulator.SteamUserStats.StoreStats();
        }

        public int GetAchievementIcon(IntPtr _, string pchName)
        {
            return SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
        }

        public IntPtr GetAchievementDisplayAttribute(IntPtr _, string pchName, string pchKey)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey));
        }

        public bool IndicateAchievementProgress(IntPtr _, string pchName, uint nCurProgress, uint nMaxProgress)
        {
            return SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
        }

        public SteamAPICall_t RequestUserStats(IntPtr _, ulong steamIDUser)
        {
            return SteamEmulator.SteamUserStats.RequestUserStats(steamIDUser);
        }

        public bool GetUserStatInt32(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetUserStatInt32(steamIDUser, pchName, pData);
        }

        public bool GetUserStatFloat(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetUserStatFloat(steamIDUser, pchName, pData);
        }

        public bool GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        public bool GetUserAchievementAndUnlockTime(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime)
        {
            return SteamEmulator.SteamUserStats.GetUserAchievementAndUnlockTime(steamIDUser, pchName, pbAchieved, punUnlockTime);
        }

        public bool ResetAllStats(IntPtr _, bool bAchievementsToo)
        {
            return SteamEmulator.SteamUserStats.ResetAllStats(bAchievementsToo);
        }

        public SteamAPICall_t FindOrCreateLeaderboard(IntPtr _, string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
        {
            return SteamEmulator.SteamUserStats.FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
        }

        public SteamAPICall_t FindLeaderboard(IntPtr _, string pchLeaderboardName)
        {
            return SteamEmulator.SteamUserStats.FindLeaderboard(pchLeaderboardName);
        }

        public IntPtr GetLeaderboardName(IntPtr _, ulong hSteamLeaderboard)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard));
        }

        public int GetLeaderboardEntryCount(IntPtr _, ulong hSteamLeaderboard)
        {
            return SteamEmulator.SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
        }

        public int GetLeaderboardSortMethod(IntPtr _, ulong hSteamLeaderboard)
        {
            return SteamEmulator.SteamUserStats.GetLeaderboardSortMethod(hSteamLeaderboard);
        }

        public int GetLeaderboardDisplayType(IntPtr _, ulong hSteamLeaderboard)
        {
            return SteamEmulator.SteamUserStats.GetLeaderboardDisplayType(hSteamLeaderboard);
        }

        public SteamAPICall_t DownloadLeaderboardEntries(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
        }

        public bool GetDownloadedLeaderboardEntry(IntPtr _, ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
        {
            return SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
        }

        public SteamAPICall_t UploadLeaderboardScore(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, int nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        {
            return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
        }

        public SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _)
        {
            return SteamEmulator.SteamUserStats.GetNumberOfCurrentPlayers();
        }

        public uint GetNumAchievements(IntPtr _)
        {
            return SteamEmulator.SteamUserStats.GetNumAchievements();
        }

        public IntPtr GetAchievementName(IntPtr _, uint iAchievement)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementName(iAchievement));
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(IntPtr _, ulong hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        {
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, prgUsers, cUsers);
        }

        public SteamAPICall_t AttachLeaderboardUGC(IntPtr _, ulong hSteamLeaderboard, ulong hUGC)
        {
            return SteamEmulator.SteamUserStats.AttachLeaderboardUGC(hSteamLeaderboard, hUGC);
        }

        public SteamAPICall_t RequestGlobalAchievementPercentages(IntPtr _)
        {
            return SteamEmulator.SteamUserStats.RequestGlobalAchievementPercentages();
        }

        public int GetMostAchievedAchievementInfo(IntPtr _, IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetMostAchievedAchievementInfo(pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        public int GetNextMostAchievedAchievementInfo(IntPtr _, int iIteratorPrevious, IntPtr pchName, uint unNameBufLen, IntPtr pflPercent, IntPtr pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetNextMostAchievedAchievementInfo(iIteratorPrevious, pchName, unNameBufLen, pflPercent, pbAchieved);
        }

        public bool GetAchievementAchievedPercent(IntPtr _, string pchName, IntPtr pflPercent)
        {
            return SteamEmulator.SteamUserStats.GetAchievementAchievedPercent(pchName, pflPercent);
        }

        public SteamAPICall_t RequestGlobalStats(IntPtr _, int nHistoryDays)
        {
            return SteamEmulator.SteamUserStats.RequestGlobalStats(nHistoryDays);
        }

        public bool GetGlobalStatInt64(IntPtr _, string pchStatName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatInt64(pchStatName, pData);
        }

        public bool GetGlobalStatDouble(IntPtr _, string pchStatName, IntPtr pData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatDouble(pchStatName, pData);
        }

        public int GetGlobalStatHistoryInt64(IntPtr _, string pchStatName, IntPtr pData, uint cubData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatHistoryInt64(pchStatName, pData, cubData);
        }

        public int GetGlobalStatHistoryDouble(IntPtr _, string pchStatName, IntPtr pData, uint cubData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatHistoryDouble(pchStatName, pData, cubData);
        }

        public bool GetAchievementProgressLimitsInt32(IntPtr _, string pchName, IntPtr pnMinProgress, IntPtr pnMaxProgress)
        {
            return SteamEmulator.SteamUserStats.GetAchievementProgressLimitsInt32(pchName, pnMinProgress, pnMaxProgress);
        }

        public bool GetAchievementProgressLimitsFloat(IntPtr _, string pchName, IntPtr pfMinProgress, IntPtr pfMaxProgress)
        {
            return SteamEmulator.SteamUserStats.GetAchievementProgressLimitsFloat(pchName, pfMinProgress, pfMaxProgress);
        }

        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static void LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (!_stubsLogged.Add(method))
                {
                    return;
                }
            }

            SteamEmulator.Write("SteamUserStats", method + " not implemented");
        }

        // Steam2-era layouts (001/002): every method carries a leading CGameID. The emulator only serves the running app, so nGameID is ignored.
        public bool RequestCurrentStats__V001(IntPtr _, ulong nGameID) => SteamEmulator.SteamUserStats.RequestCurrentStats();
        public bool GetStatFloat__V001(IntPtr _, ulong nGameID, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetStatFloat(pchName, pData);
        public bool GetStatInt32__V001(IntPtr _, ulong nGameID, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetStatInt32(pchName, pData);
        public bool SetStatFloat__V001(IntPtr _, ulong nGameID, string pchName, float fData) => SteamEmulator.SteamUserStats.SetStat(pchName, fData);
        public bool SetStatInt32__V001(IntPtr _, ulong nGameID, string pchName, int nData) => SteamEmulator.SteamUserStats.SetStat(pchName, unchecked((uint)nData));
        public bool UpdateAvgRateStat__V001(IntPtr _, ulong nGameID, string pchName, float flCountThisSession, double dSessionLength) => SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
        public bool GetAchievement__V001(IntPtr _, ulong nGameID, string pchName, IntPtr pbAchieved) => SteamEmulator.SteamUserStats.GetAchievement(pchName, pbAchieved);
        public bool SetAchievement__V001(IntPtr _, ulong nGameID, string pchName) => SteamEmulator.SteamUserStats.SetAchievement(pchName);
        public bool ClearAchievement__V001(IntPtr _, ulong nGameID, string pchName) => SteamEmulator.SteamUserStats.ClearAchievement(pchName);
        public bool StoreStats__V001(IntPtr _, ulong nGameID) => SteamEmulator.SteamUserStats.StoreStats();
        public int GetAchievementIcon__V001(IntPtr _, ulong nGameID, string pchName) => SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
        public IntPtr GetAchievementDisplayAttribute__V001(IntPtr _, ulong nGameID, string pchName, string pchKey) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey));
        public bool IndicateAchievementProgress__V001(IntPtr _, ulong nGameID, string pchName, uint nCurProgress, uint nMaxProgress) => SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
        public uint GetNumAchievements__V001(IntPtr _, ulong nGameID) => SteamEmulator.SteamUserStats.GetNumAchievements();
        public IntPtr GetAchievementName__V001(IntPtr _, ulong nGameID, uint iAchievement) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementName(iAchievement));

        public uint GetNumStats(IntPtr _, ulong nGameID)
        {
            LogStub("GetNumStats");
            return 0;
        }

        public IntPtr GetStatName(IntPtr _, ulong nGameID, uint iStat)
        {
            LogStub("GetStatName");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int GetStatType(IntPtr _, ulong nGameID, string pchName)
        {
            LogStub("GetStatType");
            return 0;
        }

        public uint GetNumGroupAchievements(IntPtr _, ulong nGameID)
        {
            LogStub("GetNumGroupAchievements");
            return 0;
        }

        public IntPtr GetGroupAchievementName(IntPtr _, ulong nGameID, uint iAchievement)
        {
            LogStub("GetGroupAchievementName");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public bool GetGroupAchievement(IntPtr _, ulong nGameID, string pchName, IntPtr pbAchieved)
        {
            LogStub("GetGroupAchievement");
            return false;
        }

        public bool SetGroupAchievement(IntPtr _, ulong nGameID, string pchName)
        {
            LogStub("SetGroupAchievement");
            return false;
        }

        public bool ClearGroupAchievement(IntPtr _, ulong nGameID, string pchName)
        {
            LogStub("ClearGroupAchievement");
            return false;
        }

        public SteamAPICall_t UploadLeaderboardScore__V005(IntPtr _, ulong hSteamLeaderboard, int nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        {
            return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, 1, nScore, pScoreDetails, cScoreDetailsCount);
        }
    }
}
