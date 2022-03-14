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
        return SteamClient.SteamRemotePlay.GetSessionCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamRemotePlay_GetSessionID(int iSessionIndex)
    {
        Write("SteamAPI_ISteamRemotePlay_GetSessionID");
        return SteamClient.SteamRemotePlay.GetSessionID(iSessionIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamRemotePlay_GetSessionSteamID(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_IntPtrGetSessionSteamID");
        return SteamClient.SteamRemotePlay.GetSessionSteamID(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static  string SteamAPI_ISteamRemotePlay_GetSessionClientName(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_stringGetSessionClientName");
        return SteamClient.SteamRemotePlay.GetSessionClientName(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamDeviceFormFactor SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor(uint unSessionID)
    {
        Write("SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor");
        return SteamClient.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemotePlay_BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY)
    {
        Write("SteamAPI_ISteamRemotePlay_BGetSessionClientResolution");
        return SteamClient.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite(IntPtr steamIDFriend)
    {
        Write("SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite");
        return SteamClient.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
    }

}

