
using System;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameServerStats001")]
    public class SteamGameServerStats001 : ISteamInterface
    {
        public SteamAPICall_t RequestUserStats(IntPtr _, ulong steamIDUser)
        {
            return SteamEmulator.SteamGameServerStats.RequestUserStats(steamIDUser);
        }

        public bool GetUserStatInt32(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamGameServerStats.GetUserStatInt32(steamIDUser, pchName, pData);
        }

        public bool GetUserStatFloat(IntPtr _, ulong steamIDUser, string pchName, IntPtr pData)
        {
            return SteamEmulator.SteamGameServerStats.GetUserStatFloat(steamIDUser, pchName, pData);
        }

        public bool GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, IntPtr pbAchieved)
        {
            return SteamEmulator.SteamGameServerStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        public bool SetUserStatInt32(IntPtr _, ulong steamIDUser, string pchName, int nData)
        {
            return SteamEmulator.SteamGameServerStats.SetUserStat(steamIDUser, pchName, nData);
        }

        public bool SetUserStatFloat(IntPtr _, ulong steamIDUser, string pchName, float fData)
        {
            return SteamEmulator.SteamGameServerStats.SetUserStat(steamIDUser, pchName, fData);
        }

        public bool UpdateUserAvgRateStat(IntPtr _, ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            return SteamEmulator.SteamGameServerStats.UpdateUserAvgRateStat(steamIDUser, pchName, flCountThisSession, dSessionLength);
        }

        public bool SetUserAchievement(IntPtr _, ulong steamIDUser, string pchName)
        {
            return SteamEmulator.SteamGameServerStats.SetUserAchievement(steamIDUser, pchName);
        }

        public bool ClearUserAchievement(IntPtr _, ulong steamIDUser, string pchName)
        {
            return SteamEmulator.SteamGameServerStats.ClearUserAchievement(steamIDUser, pchName);
        }

        public SteamAPICall_t StoreUserStats(IntPtr _, ulong steamIDUser)
        {
            return SteamEmulator.SteamGameServerStats.StoreUserStats(steamIDUser);
        }


    }
}
