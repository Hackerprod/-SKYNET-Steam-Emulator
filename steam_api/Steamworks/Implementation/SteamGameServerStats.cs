using SKYNET;
using SKYNET.Helpers;
using SKYNET.Types;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServerStats : ISteamInterface
    {
        public SteamGameServerStats()
        {
            InterfaceVersion = "SteamGameServerStats";
        }

        public bool ClearUserAchievement(SteamID steamIDUser, string pchName)
        {
            Write("ClearUserAchievement");
            return false;
        }

        public bool GetUserAchievement(SteamID steamIDUser, string pchName, bool pbAchieved)
        {
            Write("GetUserAchievement");
            return false;
        }

        public bool GetUserStat(SteamID steamIDUser, string pchName, float pData)
        {
            Write("GetUserStat");
            return false;
        }

        public SteamAPICall_t RequestUserStats(SteamID steamIDUser)
        {
            Write("RequestUserStats");
            //GSStatsReceived_t data;
            //data.m_eResult = 0;//k_EResultOK;
            //data.m_steamIDUser = steamIDUser;
            return 0;
        }

        public bool SetUserAchievement(SteamID steamIDUser, string pchName)
        {
            Write("SetUserAchievement");
            return false;
        }

        public bool SetUserStat(SteamID steamIDUser, string pchName, float nData)
        {
            Write("SetUserStat");
            return false;
        }

        public SteamAPICall_t StoreUserStats(SteamID steamIDUser)
        {
            Write("StoreUserStats");
            return 0;
        }

        public bool UpdateUserAvgRateStat(SteamID steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("UpdateUserAvgRateStat");
            return false;
        }
    }
}