using SKYNET;
using SKYNET.Helpers;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamGameServerStats : ISteamInterface
    {
        public bool ClearUserAchievement(ulong steamIDUser, string pchName)
        {
            Write("ClearUserAchievement");
            return false;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write("GetUserAchievement");
            return false;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, float pData)
        {
            Write("GetUserStat");
            return false;
        }

        public ulong RequestUserStats(ulong steamIDUser)
        {
            Write("RequestUserStats");
            //GSStatsReceived_t data;
            //data.m_eResult = 0;//k_EResultOK;
            //data.m_steamIDUser = steamIDUser;
            return 0;
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

        public ulong StoreUserStats(ulong steamIDUser)
        {
            Write("StoreUserStats");
            return 0;
        }

        public bool UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("UpdateUserAvgRateStat");
            return false;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamGameServerStats()
        {
            InterfaceVersion = "SteamGameServerStats";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}