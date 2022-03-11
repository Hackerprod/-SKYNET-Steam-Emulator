using SKYNET.Interface;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Managers
{
    public  class SteamAPI_ISteamGameServerStats : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_ClearUserAchievement(IntPtr steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_ClearUserAchievement");
            return SteamClient.steam_GameServerStats.ClearUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserAchievement");
            return SteamClient.steam_GameServerStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_GetUserStat(IntPtr steamIDUser, string pchName, int pData)
        {
            Write("SteamAPI_ISteamGameServerStats_GetUserStat");
            return SteamClient.steam_GameServerStats.GetUserStat(steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamGameServerStats_RequestUserStats(IntPtr steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_RequestUserStats");
            return SteamClient.steam_GameServerStats.RequestUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserAchievement(IntPtr steamIDUser, string pchName)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserAchievement");
            return SteamClient.steam_GameServerStats.SetUserAchievement(steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_SetUserStat(IntPtr steamIDUser, string pchName, float fData)
        {
            Write("SteamAPI_ISteamGameServerStats_SetUserStat");
            return SteamClient.steam_GameServerStats.SetUserStat(steamIDUser, pchName, fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamGameServerStats_StoreUserStats(IntPtr steamIDUser)
        {
            Write("SteamAPI_ISteamGameServerStats_StoreUserStats");
            return SteamClient.steam_GameServerStats.StoreUserStats(steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(IntPtr steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write("SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat");
            return SteamClient.steam_GameServerStats.UpdateUserAvgRateStat(steamIDUser, pchName, flCountThisSession, dSessionLength);
        }
    }
}