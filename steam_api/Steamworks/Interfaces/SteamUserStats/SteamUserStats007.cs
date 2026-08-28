using System;
using SKYNET.Helpers;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamUserStats ABI used by Left 4 Dead.</summary>
    [Interface("STEAMUSERSTATS_INTERFACE_VERSION007")]
    [MsvcVTableOverload("GetStatInt32", "GetStatFloat")]
    [MsvcVTableOverload("SetStatInt32", "SetStatFloat")]
    [MsvcVTableOverload("GetUserStatInt32", "GetUserStatFloat")]
    public class SteamUserStats007 : ISteamInterface
    {
        public bool RequestCurrentStats(IntPtr _) => SteamEmulator.SteamUserStats.RequestCurrentStats();
        public bool GetStatInt32(IntPtr _, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetStatInt32(pchName, pData);
        public bool GetStatFloat(IntPtr _, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetStatFloat(pchName, pData);
        public bool SetStatInt32(IntPtr _, string pchName, int nData) => SteamEmulator.SteamUserStats.SetStat(pchName, unchecked((uint)nData));
        public bool SetStatFloat(IntPtr _, string pchName, float fData) => SteamEmulator.SteamUserStats.SetStat(pchName, fData);
        public bool UpdateAvgRateStat(IntPtr _, string pchName, float flCountThisSession, double dSessionLength) => SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, flCountThisSession, dSessionLength);
        public bool GetAchievement(IntPtr _, string pchName, IntPtr pbAchieved) => SteamEmulator.SteamUserStats.GetAchievement(pchName, pbAchieved);
        public bool SetAchievement(IntPtr _, string pchName) => SteamEmulator.SteamUserStats.SetAchievement(pchName);
        public bool ClearAchievement(IntPtr _, string pchName) => SteamEmulator.SteamUserStats.ClearAchievement(pchName);
        public bool GetAchievementAndUnlockTime(IntPtr _, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime) => SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, pbAchieved, punUnlockTime);
        public bool StoreStats(IntPtr _) => SteamEmulator.SteamUserStats.StoreStats();
        public int GetAchievementIcon(IntPtr _, string pchName) => SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
        public IntPtr GetAchievementDisplayAttribute(IntPtr _, string pchName, string pchKey) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey));
        public bool IndicateAchievementProgress(IntPtr _, string pchName, uint nCurProgress, uint nMaxProgress) => SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
        public SteamAPICall_t RequestUserStats(IntPtr _, ulong steamIDUser) => SteamEmulator.SteamUserStats.RequestUserStats(steamIDUser);
        public bool GetUserStatInt32(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetUserStatInt32(steamIDUser, pchName, pData);
        public bool GetUserStatFloat(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData) => SteamEmulator.SteamUserStats.GetUserStatFloat(steamIDUser, pchName, pData);
        public bool GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved) => SteamEmulator.SteamUserStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        public bool GetUserAchievementAndUnlockTime(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved, IntPtr punUnlockTime) => SteamEmulator.SteamUserStats.GetUserAchievementAndUnlockTime(steamIDUser, pchName, pbAchieved, punUnlockTime);
        public bool ResetAllStats(IntPtr _, bool bAchievementsToo) => SteamEmulator.SteamUserStats.ResetAllStats(bAchievementsToo);
        public SteamAPICall_t FindOrCreateLeaderboard(IntPtr _, string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType) => SteamEmulator.SteamUserStats.FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
        public SteamAPICall_t FindLeaderboard(IntPtr _, string pchLeaderboardName) => SteamEmulator.SteamUserStats.FindLeaderboard(pchLeaderboardName);
        public IntPtr GetLeaderboardName(IntPtr _, ulong hSteamLeaderboard) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard));
        public int GetLeaderboardEntryCount(IntPtr _, ulong hSteamLeaderboard) => SteamEmulator.SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
        public int GetLeaderboardSortMethod(IntPtr _, ulong hSteamLeaderboard) => SteamEmulator.SteamUserStats.GetLeaderboardSortMethod(hSteamLeaderboard);
        public int GetLeaderboardDisplayType(IntPtr _, ulong hSteamLeaderboard) => SteamEmulator.SteamUserStats.GetLeaderboardDisplayType(hSteamLeaderboard);
        public SteamAPICall_t DownloadLeaderboardEntries(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardDataRequest, int nRangeStart, int nRangeEnd) => SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
        public bool GetDownloadedLeaderboardEntry(IntPtr _, ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax) => SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
        public SteamAPICall_t UploadLeaderboardScore(IntPtr _, ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, int nScore, IntPtr pScoreDetails, int cScoreDetailsCount) => SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
        public SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _) => SteamEmulator.SteamUserStats.GetNumberOfCurrentPlayers();
    }
}
