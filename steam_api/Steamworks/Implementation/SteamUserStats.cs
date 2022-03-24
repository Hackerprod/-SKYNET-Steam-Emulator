using Core.Interface;
using SKYNET;

using SKYNET.Steamworks;
using Steamworks;
using System;

//[Map("STEAMUSERSTATS_INTERFACE_VERSION")]
//[Map("SteamUserStats")]
public class SteamUserStats : IBaseInterface
{
    public bool RequestCurrentStats(IntPtr _)
    {
        return false;
    }

    public bool GetStat(IntPtr _, string pchName, uint pData)
    {
        return false;
    }

    public bool SetStat(IntPtr _, string pchName, uint nData)
    {
        return false;
    }

    public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
    {
        return false;
    }

    public bool GetAchievement(string pchName, bool pbAchieved)
    {
        return false;
    }

    public bool SetAchievement(string pchName)
    {
        return false;
    }

    public bool ClearAchievement(string pchName)
    {
        return false;
    }

    public bool GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime)
    {
        return false;
    }

    public bool StoreStats(IntPtr _)
    {
        return false;
    }

    public int GetAchievementIcon(string pchName)
    {
        return 0;
    }

    public string GetAchievementDisplayAttribute(string pchName, string pchKey)
    {
        return "";
    }

    public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
    {
        return false;
    }

    public uint GetNumAchievements(IntPtr _)
    {
        return 0;
    }

    public string GetAchievementName(uint iAchievement)
    {
        return "";
    }

    public SteamAPICall_t RequestUserStats(IntPtr steamIDUser)
    {
        return default;
    }

    public bool GetUserStat(IntPtr _, IntPtr steamIDUser, string pchName, uint pData)
    {
        return false;
    }

    public bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
    {
        return false;
    }

    public bool GetUserAchievementAndUnlockTime(IntPtr steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
    {
        return false;
    }

    public bool ResetAllStats(bool bAchievementsToo)
    {
        return false;
    }

    public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, int eLeaderboardDisplayType)
    {
        return default;
    }

    public SteamAPICall_t FindLeaderboard(string pchLeaderboardName)
    {
        return default;
    }

    public string GetLeaderboardName(IntPtr hSteamLeaderboard)
    {
        return "";
    }

    public int GetLeaderboardEntryCount(IntPtr hSteamLeaderboard)
    {
        return 0;
    }

    public ELeaderboardSortMethod GetLeaderboardSortMethod(IntPtr hSteamLeaderboard)
    {
        return default;
    }

    public int GetLeaderboardDisplayType(IntPtr hSteamLeaderboard)
    {
        return 0;
    }

    public SteamAPICall_t DownloadLeaderboardEntries(IntPtr hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
    {
        return default;
    }

    public SteamAPICall_t DownloadLeaderboardEntriesForUsers(IntPtr hSteamLeaderboard, IntPtr prgUsers, int cUsers)
    {
        return default;
    }

    public bool GetDownloadedLeaderboardEntry(IntPtr hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
    {
        return false;
    }

    public SteamAPICall_t UploadLeaderboardScore(IntPtr hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
    {
        return default;
    }

    public SteamAPICall_t AttachLeaderboardUGC(IntPtr hSteamLeaderboard, UGCHandle_t hUGC)
    {
        return default;
    }

    public SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _)
    {
        return default;
    }

    public SteamAPICall_t RequestGlobalAchievementPercentages(IntPtr _)
    {
        return default;
    }

    public int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
    {
        return 0;
    }

    public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
    {
        return 0;
    }

    public bool GetAchievementAchievedPercent(string pchName, float pflPercent)
    {
        return false;
    }

    public SteamAPICall_t RequestGlobalStats(int nHistoryDays)
    {
        return default;
    }

    public bool GetGlobalStat(string pchStatName, uint pData)
    {
        return false;
    }

    public uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
    {
        return 0;
    }

    public bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
    {
        return false;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}