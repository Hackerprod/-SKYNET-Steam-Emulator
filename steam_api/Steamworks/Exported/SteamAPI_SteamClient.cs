using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET;
using SKYNET.Managers;
using Steamworks;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamClient
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamClient_CreateSteamPipe(IntPtr _)
        {
            Write("SteamAPI_ISteamClient_CreateSteamPipe");
            return (int)SteamEmulator.CreateSteamPipe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamClient_BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe)
        {
            Write("SteamAPI_ISteamClient_BReleaseSteamPipe");
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamClient_ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe)
        {
            Write("SteamAPI_ISteamClient_ConnectToGlobalUser");
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_ISteamClient_CreateLocalUser(IntPtr _, HSteamPipe phSteamPipe, int eAccountType)
        {
            Write("SteamAPI_ISteamClient_CreateLocalUser");
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, eAccountType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamClient_ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            Write("SteamAPI_ISteamClient_ReleaseUser");
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamUser");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamGameServer");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamClient_SetLocalIPBinding(IntPtr _, UInt32 unIP, ushort usPort)
        {
            Write("SteamAPI_ISteamClient_SetLocalIPBinding");
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamFriends");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamUtils");
            return InterfaceManager.FindOrCreateInterface(1, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamMatchmaking");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamMatchmakingServers");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamGenericInterface");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamUserStats");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamGameServerStats");
            return InterfaceManager.FindOrCreateInterface((int)1, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamApps");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamNetworking");
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamRemoteStorage");
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamScreenshots(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamScreenshots");
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamClient_GetIPCCallCount(IntPtr _)
        {
            Write("SteamAPI_ISteamClient_GetIPCCallCount");
            return SteamEmulator.SteamClient.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed(IntPtr _)
        {
            Write("SteamAPI_ISteamClient_BShutdownIfAllPipesClosed");
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamHTTP(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamHTTP");
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamController");
            return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamUGC");
            return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamAppList");
            return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMusic(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamMusic");
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMusicRemote(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamMusicRemote");
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamHTMLSurface");
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamInventory(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamInventory");
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamVideo(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamVideo");
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamParentalSettings(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("SteamAPI_ISteamClient_GetISteamParentalSettings");
            return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, pchVersion);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("SteamAPI_SteamClient", msg);
        }
    }
}
