using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET.Helper;
using Steamworks;

public partial class SteamAPI_SteamGameServer : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamGameServer_GetHSteamUser()
    {
        DEBUG("SteamGameServer_GetHSteamUser\n");
        return (HSteamUser)1;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe SteamGameServer_GetHSteamPipe()
    {
        DEBUG("SteamGameServer_GetHSteamPipe\n");
        return (HSteamPipe)1;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_Init(IntPtr unIP, IntPtr usSteamPort, IntPtr usGamePort, IntPtr unknown, IntPtr eServerMode, IntPtr unknown1, IntPtr unknown2, IntPtr unknown3)
    {
        DEBUG("SteamGameServer_Init\n");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_InitSafe(IntPtr unIP, IntPtr usSteamPort, IntPtr usGamePort, IntPtr unknown, IntPtr eServerMode, IntPtr unknown1, IntPtr unknown2, IntPtr unknown3)
    {
        DEBUG("SteamGameServer_InitSafe\n");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamGameServer_Shutdown()
    {
        DEBUG("SteamGameServer_Shutdown\n");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamGameServer_RunCallbacks()
    {
        DEBUG("SteamGameServer_RunCallbacks\n");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_BSecure()
    {
        DEBUG("SteamGameServer_BSecure\n");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ulong SteamGameServer_GetSteamID()
    {
        DEBUG("SteamGameServer_GetSteamID\n");
        return modCommon.CreateSteamID();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ulong SteamGameServer_GetIPCCallCount()
    {
        DEBUG("SteamGameServer_GetIPCCallCount\n");
        return 0;
    }

    private static void DEBUG(string msg)
    {
        Log.Write(msg);
    }
}