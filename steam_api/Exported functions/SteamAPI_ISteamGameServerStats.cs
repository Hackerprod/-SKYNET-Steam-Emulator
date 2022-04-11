using SKYNET;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamGameServerStats : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_ClearUserAchievement(ulong steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_ClearUserAchievement");
            return SteamEmulator.SteamGameServerStats.ClearUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserAchievement");
            return SteamEmulator.SteamGameServerStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserStat(ulong steamIDUser, string pchName, int pData)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserStat");
            return SteamEmulator.SteamGameServerStats.GetUserStat(steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamGameServerStats_RequestUserStats(ulong steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_RequestUserStats");
            return SteamEmulator.SteamGameServerStats.RequestUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserAchievement(ulong steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserAchievement");
            return SteamEmulator.SteamGameServerStats.SetUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserStat(ulong steamIDUser, string pchName, int fData)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserStat");
            return SteamEmulator.SteamGameServerStats.SetUserStat(steamIDUser, pchName, fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamGameServerStats_StoreUserStats(ulong steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_StoreUserStats");
            return SteamEmulator.SteamGameServerStats.StoreUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat");
            return SteamEmulator.SteamGameServerStats.UpdateUserAvgRateStat(steamIDUser, pchName, flCountThisSession, dSessionLength);
        }
    }
}
