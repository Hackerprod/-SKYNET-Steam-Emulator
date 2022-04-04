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
        Write("SteamGameServer_GetHSteamUser\n");
        return (HSteamUser)1;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe SteamGameServer_GetHSteamPipe()
    {
        Write("SteamGameServer_GetHSteamPipe\n");
        return (HSteamPipe)1;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_Init(IntPtr unIP, IntPtr usSteamPort, IntPtr usGamePort, IntPtr unknown, IntPtr eServerMode, IntPtr unknown1, IntPtr unknown2, IntPtr unknown3)
    {
        Write("SteamGameServer_Init\n");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_InitSafe(IntPtr unIP, IntPtr usSteamPort, IntPtr usGamePort, IntPtr unknown, IntPtr eServerMode, IntPtr unknown1, IntPtr unknown2, IntPtr unknown3)
    {
        Write("SteamGameServer_InitSafe\n");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamGameServer_Shutdown()
    {
        Write("SteamGameServer_Shutdown\n");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamGameServer_RunCallbacks()
    {
        Write("SteamGameServer_RunCallbacks\n");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamGameServer_BSecure()
    {
        Write("SteamGameServer_BSecure\n");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ulong SteamGameServer_GetSteamID()
    {
        Write("SteamGameServer_GetSteamID\n");
        return modCommon.CreateSteamID();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ulong SteamGameServer_GetIPCCallCount()
    {
        Write("SteamGameServer_GetIPCCallCount\n");
        return 0;
    }

    private static void Write(string msg)
    {
        SteamEmulator.Write(msg);
    }
}