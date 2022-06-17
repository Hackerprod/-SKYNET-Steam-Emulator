using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamGameServerStats
    {
        static SteamAPI_ISteamGameServerStats()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_ClearUserAchievement(IntPtr _, ulong steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_ClearUserAchievement");
            return SteamEmulator.SteamGameServerStats.ClearUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserAchievement(IntPtr _, ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserAchievement");
            return SteamEmulator.SteamGameServerStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserStat(IntPtr _, ulong steamIDUser, string pchName, int pData)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserStat");
            return SteamEmulator.SteamGameServerStats.GetUserStat(steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamGameServerStats_RequestUserStats(IntPtr _, ulong steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_RequestUserStats");
            return SteamEmulator.SteamGameServerStats.RequestUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserAchievement(IntPtr _, ulong steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserAchievement");
            return SteamEmulator.SteamGameServerStats.SetUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserStat(IntPtr _, ulong steamIDUser, string pchName, int fData)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserStat");
            return SteamEmulator.SteamGameServerStats.SetUserStat(steamIDUser, pchName, fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamGameServerStats_StoreUserStats(IntPtr _, ulong steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_StoreUserStats");
            return SteamEmulator.SteamGameServerStats.StoreUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(IntPtr _, ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat");
            return SteamEmulator.SteamGameServerStats.UpdateUserAvgRateStat(steamIDUser, pchName, flCountThisSession, dSessionLength);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
