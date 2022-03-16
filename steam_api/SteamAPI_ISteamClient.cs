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
    public static HSteamPipe SteamAPI_ISteamClient_CreateSteamPipe()
    {
        Write("SteamAPI_ISteamClient_CreateSteamPipe");
        return SteamEmulator.CreateSteamPipe();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BReleaseSteamPipe(HSteamPipe hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_BReleaseSteamPipe");
        return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_ISteamClient_ConnectToGlobalUser(HSteamPipe hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_ConnectToGlobalUser");
        return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_ISteamClient_CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
    {
        Write("SteamAPI_ISteamClient_CreateLocalUser");
        return SteamEmulator.SteamClient.CreateLocalUser(out phSteamPipe, eAccountType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
    {
        Write("SteamAPI_ISteamClient_ReleaseUser");
        SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUser SteamAPI_ISteamClient_GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUser");
        return SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameServer SteamAPI_ISteamClient_GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServer");
        return SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_SetLocalIPBinding(uint unIP, ushort usPort)
    {
        Write("SteamAPI_ISteamClient_SetLocalIPBinding");
        SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamFriends SteamAPI_ISteamClient_GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamFriends");
        return SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamAPI_ISteamClient_GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUtils");
        return SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmaking SteamAPI_ISteamClient_GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmaking");
        return SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmakingServers SteamAPI_ISteamClient_GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmakingServers");
        return SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGenericInterface");
        return SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUserStats SteamAPI_ISteamClient_GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUserStats");
        return SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameServerStats SteamAPI_ISteamClient_GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServerStats");
        return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamApps SteamAPI_ISteamClient_GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamApps");
        return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworking SteamAPI_ISteamClient_GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamNetworking");
        return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamRemoteStorage SteamAPI_ISteamClient_GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamRemoteStorage");
        return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamScreenshots SteamAPI_ISteamClient_GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamScreenshots");
        return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamClient_GetIPCCallCount()
    {
        Write("SteamAPI_ISteamClient_GetIPCCallCount");
        return SteamEmulator.SteamClient.GetIPCCallCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed()
    {
        Write("SteamAPI_ISteamClient_BShutdownIfAllPipesClosed");
        return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTTP SteamAPI_ISteamClient_GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTTP");
        return SteamEmulator.SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamController SteamAPI_ISteamClient_GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamController");
        return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUGC SteamAPI_ISteamClient_GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUGC");
        return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamAppList SteamAPI_ISteamClient_GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamAppList");
        return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusic SteamAPI_ISteamClient_GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusic");
        return SteamEmulator.SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusicRemote SteamAPI_ISteamClient_GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusicRemote");
        return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTMLSurface SteamAPI_ISteamClient_GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTMLSurface");
        return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInventory SteamAPI_ISteamClient_GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamInventory");
        return SteamEmulator.SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamVideo SteamAPI_ISteamClient_GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamVideo");
        return SteamEmulator.SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamParentalSettings SteamAPI_ISteamClient_GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamParentalSettings");
        return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamuser, hSteamPipe, pchVersion);
    }

}

