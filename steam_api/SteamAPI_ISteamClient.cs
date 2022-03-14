using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI_ISteamClient : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe SteamAPI_ISteamClient_CreateSteamPipe()
    {
        Write("SteamAPI_ISteamClient_CreateSteamPipe");
        return SteamClient.CreateSteamPipe();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BReleaseSteamPipe(HSteamPipe hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_BReleaseSteamPipe");
        return SteamClient.BReleaseSteamPipe(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_ISteamClient_ConnectToGlobalUser(HSteamPipe hSteamPipe)
    {
        Write("SteamAPI_ISteamClient_ConnectToGlobalUser");
        return SteamClient.ConnectToGlobalUser(hSteamPipe);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_ISteamClient_CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
    {
        Write("SteamAPI_ISteamClient_CreateLocalUser");
        return SteamClient.CreateLocalUser(out phSteamPipe, eAccountType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
    {
        Write("SteamAPI_ISteamClient_ReleaseUser");
        SteamClient.ReleaseUser(hSteamPipe, hUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUser");
        return IntPtr.Zero; // SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServer");
        return IntPtr.Zero; // SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamClient_SetLocalIPBinding(uint unIP, ushort usPort)
    {
        Write("SteamAPI_ISteamClient_SetLocalIPBinding");
        SteamClient.SetLocalIPBinding(unIP, usPort);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamFriends");
        return IntPtr.Zero; // SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUtils");
        return IntPtr.Zero; // SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmaking");
        return IntPtr.Zero; // SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMatchmakingServers");
        return IntPtr.Zero; // SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGenericInterface");
        return IntPtr.Zero; // SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUserStats");
        return IntPtr.Zero; // SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamGameServerStats");
        return IntPtr.Zero; // SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamApps");
        return IntPtr.Zero; // SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamNetworking");
        return IntPtr.Zero; // SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamRemoteStorage");
        return IntPtr.Zero; // SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamScreenshots");
        return IntPtr.Zero; // SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamClient_GetIPCCallCount()
    {
        Write("SteamAPI_ISteamClient_GetIPCCallCount");
        return SteamClient.GetIPCCallCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed()
    {
        Write("SteamAPI_ISteamClient_BShutdownIfAllPipesClosed");
        return SteamClient.BShutdownIfAllPipesClosed();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTTP");
        return IntPtr.Zero; // SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamController");
        return IntPtr.Zero; // SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamUGC");
        return IntPtr.Zero; // SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamAppList");
        return IntPtr.Zero; // SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusic");
        return IntPtr.Zero; // SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamMusicRemote");
        return IntPtr.Zero; // SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamHTMLSurface");
        return IntPtr.Zero; // SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamInventory");
        return IntPtr.Zero; // SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamVideo");
        return IntPtr.Zero; // SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamClient_GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        Write("SteamAPI_ISteamClient_GetISteamParentalSettings");
        return IntPtr.Zero; // SteamClient.GetISteamParentalSettings(hSteamuser, hSteamPipe, pchVersion);
    }

}

