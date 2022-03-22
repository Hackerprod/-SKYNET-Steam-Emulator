using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI_ISteamClient : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamClient_CreateSteamPipe()
    {
        Write("SteamAPI_ISteamClient_CreateSteamPipe");
        return (int)SteamEmulator.CreateSteamPipe();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BReleaseSteamPipe(int hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_BReleaseSteamPipe");
        return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamClient_ConnectToGlobalUser(int hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_ConnectToGlobalUser");
        return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamClient_CreateLocalUser(out int phSteamPipe, EAccountType eAccountType)
    {
        Write("SteamAPI_ISteamClient_CreateLocalUser");
        return SteamEmulator.SteamClient.CreateLocalUser(out phSteamPipe, eAccountType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_ReleaseUser(int hSteamPipe, int hUser)
    {
        Write("SteamAPI_ISteamClient_ReleaseUser");
        SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUser");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServer");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_SetLocalIPBinding(uint unIP, ushort usPort)
    {
        Write("SteamAPI_ISteamClient_SetLocalIPBinding");
        SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamFriends(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamFriends");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUtils(int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUtils");
        return InterfaceManager.FindOrCreateInterface(11, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmaking");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmakingServers");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGenericInterface");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUserStats");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServerStats");
        return InterfaceManager.FindOrCreateInterface((int)1, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamApps");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamNetworking");
        return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamRemoteStorage");
        return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamScreenshots(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamScreenshots");
        return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamClient_GetIPCCallCount()
    {
        Write("SteamAPI_ISteamClient_GetIPCCallCount");
        return SteamEmulator.SteamClient.GetIPCCallCount(IntPtr.Zero);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed(IntPtr _)
    {
        Write("SteamAPI_ISteamClient_BShutdownIfAllPipesClosed");
        return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamHTTP(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTTP");
        return SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamController");
        return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUGC");
        return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamAppList");
        return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusic");
        return SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusicRemote");
        return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTMLSurface");
        return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamInventory(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamInventory");
        return SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamVideo");
        return SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamParentalSettings");
        return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, pchVersion);
    }

}

