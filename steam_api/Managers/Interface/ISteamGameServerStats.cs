using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamGameServerStats
    {
        SteamAPICall_t RequestUserStats(IntPtr steamIDUser);

        // requests stat information for a user, usable after a successful call to RequestUserStats()

        bool GetUserStat(IntPtr steamIDUser, string pchName, int pData);


        bool GetUserStat(IntPtr steamIDUser, string pchName, float pData);

        bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved);

        // Set / update stats and achievements. 
        // Note: These updates will work only on stats game servers are allowed to edit and only for 
        // game servers that have been declared as officially controlled by the game creators. 
        // Set the IP range of your official servers on the Steamworks page


        bool SetUserStat(IntPtr steamIDUser, string pchName, int nData);


        bool SetUserStat(IntPtr steamIDUser, string pchName, float fData);

        bool UpdateUserAvgRateStat(IntPtr steamIDUser, string pchName, float flCountThisSession, double dSessionLength);

        bool SetUserAchievement(IntPtr steamIDUser, string pchName);
        bool ClearUserAchievement(IntPtr steamIDUser, string pchName);

        // Store the current data on the server, will get a GSStatsStored_t callback when set.
        //
        // If the callback has a result of k_EResultInvalidParam, one or more stats 
        // uploaded has been rejected, either because they broke constraints
        // or were out of date. In this case the server sends back updated values.
        // The stats should be re-iterated to keep in sync.

        SteamAPICall_t StoreUserStats(IntPtr steamIDUser);

    }
    public struct GSStatsReceived_t
    {
        public int m_eResult;      // Success / error fetching the stats
        public IntPtr m_steamIDUser; // The user for whom the stats are retrieved for
    };

}
