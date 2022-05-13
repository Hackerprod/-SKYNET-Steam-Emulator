
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServerStats : ISteamInterface
    {
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamGameServerStats()
        {
            InterfaceName = "SteamGameServerStats";
            InterfaceVersion = "SteamGameServerStats001";
        }

        public bool ClearUserAchievement(ulong steamIDUser, string pchName)
        {
            Write("ClearUserAchievement");
            return false;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            return false;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, float pData)
        {
            Write("GetUserStat");
            return false;
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            Write("RequestUserStats");
            //GSStatsReceived_t data;
            //data.m_eResult = 0;//k_EResultOK;
            //data.m_steamIDUser = steamIDUser;
            return k_uAPICallInvalid;
        }

        public bool SetUserAchievement(ulong steamIDUser, string pchName)
        {
            Write("SetUserAchievement");
            return false;
        }

        public bool SetUserStat(ulong steamIDUser, string pchName, float nData)
        {
            Write("SetUserStat");
            return false;
        }

        public SteamAPICall_t StoreUserStats(ulong steamIDUser)
        {
            Write("StoreUserStats");
            return k_uAPICallInvalid;
        }

        public bool UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("UpdateUserAvgRateStat");
            return false;
        }
    }
}