using SKYNET;
using Steamworks;
using System;
using System.Runtime.InteropServices;

public class SteamAPI_ISteamGameServerStats : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_ClearUserAchievement(IntPtr steamIDUser, string pchName)
    {
        Write("SteamAPI_ISteamGameServerStats_ClearUserAchievement");
        return SteamEmulator.SteamGameServerStats.ClearUserAchievement(steamIDUser, pchName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
    {
        Write("SteamAPI_ISteamGameServerStats_GetUserAchievement");
        return SteamEmulator.SteamGameServerStats.GetUserAchievement(steamIDUser, pchName, pbAchieved);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_GetUserStat(IntPtr steamIDUser, string pchName, int pData)
    {
        Write("SteamAPI_ISteamGameServerStats_GetUserStat");
        return SteamEmulator.SteamGameServerStats.GetUserStat(steamIDUser, pchName, pData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamGameServerStats_RequestUserStats(IntPtr steamIDUser)
    {
        Write("SteamAPI_ISteamGameServerStats_RequestUserStats");
        return SteamEmulator.SteamGameServerStats.RequestUserStats(steamIDUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_SetUserAchievement(IntPtr steamIDUser, string pchName)
    {
        Write("SteamAPI_ISteamGameServerStats_SetUserAchievement");
        return SteamEmulator.SteamGameServerStats.SetUserAchievement(steamIDUser, pchName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_SetUserStat(IntPtr steamIDUser, string pchName, int fData)
    {
        Write("SteamAPI_ISteamGameServerStats_SetUserStat");
        return SteamEmulator.SteamGameServerStats.SetUserStat(steamIDUser, pchName, fData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamGameServerStats_StoreUserStats(IntPtr steamIDUser)
    {
        Write("SteamAPI_ISteamGameServerStats_StoreUserStats");
        return SteamEmulator.SteamGameServerStats.StoreUserStats(steamIDUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat(IntPtr steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
    {
        Write("SteamAPI_ISteamGameServerStats_UpdateUserAvgRateStat");
        return SteamEmulator.SteamGameServerStats.UpdateUserAvgRateStat(steamIDUser, pchName, flCountThisSession, dSessionLength);
    }
}
