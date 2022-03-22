using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamUserStats
    {
        // Ask the server to send down this user's data and achievements for this game

        bool RequestCurrentStats(IntPtr _);

        // Data accessors

        bool GetStat(IntPtr _, string pchName, uint pData);


        // Set / update data

        bool SetStat(IntPtr _, string pchName, uint nData);


        bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength);

        // Achievement flag accessors
        bool GetAchievement(string pchName, bool pbAchieved);
        bool SetAchievement(string pchName);
        bool ClearAchievement(string pchName);

        // Get the achievement status, and the time it was unlocked if unlocked.
        // If the return value is true, but the unlock time is zero, that means it was unlocked before Steam 
        // began tracking achievement unlock times (December 2009). Time is seconds since January 1, 1970.
        bool GetAchievementAndUnlockTime(string pchName, bool pbAchieved, uint punUnlockTime);

        // Store the current data on the server, will get a callback when set
        // And one callback for every new achievement
        //
        // If the callback has a result of k_EResultInvalidParam, one or more stats 
        // uploaded has been rejected, either because they broke raints
        // or were out of date. In this case the server sends back updated values.
        // The stats should be re-iterated to keep in sync.
        bool StoreStats(IntPtr _);

        // Achievement / GroupAchievement metadata

        // Gets the icon of the achievement, which is a handle to be used in ISteamUtils::GetImageRGBA(), or 0 if none set. 
        // A return value of 0 may indicate we are still fetching data, and you can wait for the UserAchievementIconFetched_t callback
        // which will notify you when the bits are ready. If the callback still returns zero, then there is no image set for the
        // specified achievement.
        int GetAchievementIcon(string pchName);

        // Get general attributes for an achievement. Accepts the following keys:
        // - "name" and "desc" for retrieving the localized achievement name and description (returned in UTF8)
        // - "hidden" for retrieving if an achievement is hidden (returns "0" when not hidden, "1" when hidden)
        string GetAchievementDisplayAttribute(string pchName, string pchKey);

        // Achievement progress - triggers an AchievementProgress callback, that is all.
        // Calling this w/ N out of N progress will NOT set the achievement, the game must still do that.
        bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress);

        // Used for iterating achievements. In general games should not need these functions because they should have a
        // list of existing achievements compiled into them
        uint GetNumAchievements(IntPtr _);
        // Get achievement name iAchievement in [0,GetNumAchievements)
        string GetAchievementName(uint iAchievement);

        // Friends stats & achievements

        // downloads stats for the user
        // returns a UserStatsReceived_t received when completed
        // if the other user has no stats, UserStatsReceived_t.m_eResult will be set to k_EResultFail
        // these stats won't be auto-updated; you'll need to call RequestUserStats() again to refresh any data

        SteamAPICall_t RequestUserStats(IntPtr steamIDUser);

        // requests stat information for a user, usable after a successful call to RequestUserStats()

        bool GetUserStat(IntPtr _, IntPtr steamIDUser, string pchName, uint pData);


        bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved);
        // See notes for GetAchievementAndUnlockTime above
        bool GetUserAchievementAndUnlockTime(IntPtr steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime);

        // Reset stats 
        bool ResetAllStats(bool bAchievementsToo);

        // Leaderboard functions

        // asks the Steam back-end for a leaderboard by name, and will create it if it's not yet
        // This call is asynchronous, with the result returned in LeaderboardFindResult_t

        SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, int eLeaderboardDisplayType);

        // as above, but won't create the leaderboard if it's not found
        // This call is asynchronous, with the result returned in LeaderboardFindResult_t

        SteamAPICall_t FindLeaderboard(string pchLeaderboardName);

        // returns the name of a leaderboard
        string GetLeaderboardName(IntPtr hSteamLeaderboard);

        // returns the total number of entries in a leaderboard, as of the last request
        int GetLeaderboardEntryCount(IntPtr hSteamLeaderboard);

        // returns the sort method of the leaderboard
        ELeaderboardSortMethod GetLeaderboardSortMethod(IntPtr hSteamLeaderboard);

        // returns the display type of the leaderboard
        int GetLeaderboardDisplayType(IntPtr hSteamLeaderboard);

        // Asks the Steam back-end for a set of rows in the leaderboard.
        // This call is asynchronous, with the result returned in LeaderboardScoresDownloaded_t
        // LeaderboardScoresDownloaded_t will contain a handle to pull the results from GetDownloadedLeaderboardEntries() (below)
        // You can ask for more entries than exist, and it will return as many as do exist.
        // k_ELeaderboardDataRequestGlobal requests rows in the leaderboard from the full table, with nRangeStart & nRangeEnd in the range [1, TotalEntries]
        // k_ELeaderboardDataRequestGlobalAroundUser requests rows around the current user, nRangeStart being negate
        //   e.g. DownloadLeaderboardEntries( hLeaderboard, k_ELeaderboardDataRequestGlobalAroundUser, -3, 3 ) will return 7 rows, 3 before the user, 3 after
        // k_ELeaderboardDataRequestFriends requests all the rows for friends of the current user 

        SteamAPICall_t DownloadLeaderboardEntries(IntPtr hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd);
        // as above, but downloads leaderboard entries for an arbitrary set of users - ELeaderboardDataRequest is k_ELeaderboardDataRequestUsers
        // if a user doesn't have a leaderboard entry, they won't be included in the result
        // a max of 100 users can be downloaded at a time, with only one outstanding call at a time


        SteamAPICall_t DownloadLeaderboardEntriesForUsers(IntPtr hSteamLeaderboard, IntPtr prgUsers, int cUsers);

        // Returns data about a single leaderboard entry
        // use a for loop from 0 to LeaderboardScoresDownloaded_t::m_cEntryCount to get all the downloaded entries
        // e.g.
        //		void OnLeaderboardScoresDownloaded( LeaderboardScoresDownloaded_t pLeaderboardScoresDownloaded )
        //		{
        //			for ( int index; index < pLeaderboardScoresDownloaded->m_cEntryCount; index++ )
        //			{
        //				LeaderboardEntry_t leaderboardEntry;
        //				uint details[3];		// we know this is how many we've stored previously
        //				GetDownloadedLeaderboardEntry( pLeaderboardScoresDownloaded->m_hSteamLeaderboardEntries, index, &leaderboardEntry, details, 3 );
        //				assert( leaderboardEntry.m_cDetails == 3 );
        //				...
        //			}
        // once you've accessed all the entries, the data will be free'd, and the SteamLeaderboardEntries_t handle will become invalid
        bool GetDownloadedLeaderboardEntry(IntPtr hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax);

        // Uploads a user score to the Steam back-end.
        // This call is asynchronous, with the result returned in LeaderboardScoreUploaded_t
        // Details are extra game-defined information regarding how the user got that score
        // pScoreDetails points to an array of uint's, cScoreDetailsCount is the number of uint's in the list

        SteamAPICall_t UploadLeaderboardScore(IntPtr hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount);

        // Attaches a piece of user generated content the user's entry on a leaderboard.
        // hContent is a handle to a piece of user generated content that was shared using ISteamUserRemoteStorage::FileShare().
        // This call is asynchronous, with the result returned in LeaderboardUGCSet_t.

        SteamAPICall_t AttachLeaderboardUGC(IntPtr hSteamLeaderboard, UGCHandle_t hUGC);

        // Retrieves the number of players currently playing your game (online + offline)
        // This call is asynchronous, with the result returned in NumberOfCurrentPlayers_t

        SteamAPICall_t GetNumberOfCurrentPlayers(IntPtr _);

        // Requests that Steam fetch data on the percentage of players who have received each achievement
        // for the game globally.
        // This call is asynchronous, with the result returned in GlobalAchievementPercentagesReady_t.

        SteamAPICall_t RequestGlobalAchievementPercentages(IntPtr _);

        // Get the info on the most achieved achievement for the game, returns an iterator index you can use to fetch
        // the next most achieved afterwards.  Will return -1 if there is no data on achievement 
        // percentages (ie, you haven't called RequestGlobalAchievementPercentages and waited on the callback).
        int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved);

        // Get the info on the next most achieved achievement for the game. Call this after GetMostAchievedAchievementInfo or another
        // GetNextMostAchievedAchievementInfo call passing the iterator from the previous call. Returns -1 after the last
        // achievement has been iterated.
        int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved);

        // Returns the percentage of users who have achieved the specified achievement.
        bool GetAchievementAchievedPercent(string pchName, float pflPercent);

        // Requests global stats data, which is available for stats marked as "aggregated".
        // This call is asynchronous, with the results returned in GlobalStatsReceived_t.
        // nHistoryDays specifies how many days of day-by-day history to retrieve in addition
        // to the overall totals. The limit is 60.

        SteamAPICall_t RequestGlobalStats(int nHistoryDays);

        // Gets the lifetime totals for an aggregated stat

        bool GetGlobalStat(string pchStatName, uint pData);

        // Gets history for an aggregated stat. pData will be filled with daily values, starting with today.
        // So when called, pData[0] will be today, pData[1] will be yesterday, and pData[2] will be two days ago, 
        // etc. cubData is the size in bytes of the pubData buffer. Returns the number of 
        // elements actually set.


        uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData);

        // For achievements that have related Progress stats, use this to query what the bounds of that progress are.
        // You may want this info to selectively call IndicateAchievementProgress when appropriate milestones of progress
        // have been made, to show a progress notification to the user.

        bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress);

    }
    // the sort order of a leaderboard
    public enum ELeaderboardSortMethod : int
    {
        k_ELeaderboardSortMethodNone = 0,
        k_ELeaderboardSortMethodAscending = 1,  // top-score is lowest number
        k_ELeaderboardSortMethodDescending = 2, // top-score is highest number
    };
    public enum ELeaderboardUploadScoreMethod : int
    {
        k_ELeaderboardUploadScoreMethodNone = 0,
        k_ELeaderboardUploadScoreMethodKeepBest = 1,    // Leaderboard will keep user's best score
        k_ELeaderboardUploadScoreMethodForceUpdate = 2, // Leaderboard will always replace score with specified
    };
}
