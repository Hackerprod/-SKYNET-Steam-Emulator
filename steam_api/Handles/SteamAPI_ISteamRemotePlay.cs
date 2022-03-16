using SKYNET;
using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamRemotePlay : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamRemotePlay_GetSessionCount()
    {
        Write("SteamAPI_ISteamRemotePlay_GetSessionCount");
        return SteamEmulator.SteamRemotePlay.GetSessionCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamRemotePlay_GetSessionID(int iSessionIndex)
    {
        Write("SteamAPI_ISteamRemotePlay_GetSessionID");
        return SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamRemotePlay_GetSessionSteamID(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_IntPtrGetSessionSteamID");
        return SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static  string SteamAPI_ISteamRemotePlay_GetSessionClientName(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_stringGetSessionClientName");
        return SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamDeviceFormFactor SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor");
        return SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemotePlay_BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY)
    {
        Write("SteamAPI_ISteamRemotePlay_BGetSessionClientResolution");
        return SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite(IntPtr steamIDFriend)
    {
        Write("SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite");
        return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
    }

}

