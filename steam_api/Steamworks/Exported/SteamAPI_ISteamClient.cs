using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET;
using SKYNET.Managers;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamClient
    {
        static SteamAPI_ISteamClient()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe SteamAPI_ISteamClient_CreateSteamPipe(IntPtr _)
        {
            Write($"SteamAPI_ISteamClient_CreateSteamPipe");
            return SteamEmulator.CreateSteamPipe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamClient_BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe)
        {
            Write($"SteamAPI_ISteamClient_BReleaseSteamPipe");
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_ISteamClient_ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe)
        {
            Write($"SteamAPI_ISteamClient_ConnectToGlobalUser");
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_ISteamClient_CreateLocalUser(IntPtr _, HSteamPipe phSteamPipe, int eAccountType)
        {
            Write($"SteamAPI_ISteamClient_CreateLocalUser");
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, eAccountType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamClient_ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            Write($"SteamAPI_ISteamClient_ReleaseUser");
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamUser {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamGameServer {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamClient_SetLocalIPBinding(IntPtr _, UInt32 unIP, ushort usPort)
        {
            Write($"SteamAPI_ISteamClient_SetLocalIPBinding");
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamFriends {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamUtils {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(1, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamMatchmaking {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamMatchmakingServers {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamGenericInterface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGameSearch(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamGameSearch {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamUserStats {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamParties(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamParties {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamRemotePlay(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamRemotePlay {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamGameServerStats {pchVersion}");
            return InterfaceManager.FindOrCreateInterface((int)1, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamApps {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamNetworking {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamRemoteStorage {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamScreenshots(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamScreenshots {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamClient_GetIPCCallCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamClient_GetIPCCallCount");
            return SteamEmulator.SteamClient.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamClient_SetWarningMessageHook(IntPtr _, IntPtr pFunctionPtr)
        {
            Write($"SteamAPI_ISteamClient_SetWarningMessageHook");
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunctionPtr);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamClient_BShutdownIfAllPipesClosed(IntPtr _)
        {
            Write($"SteamAPI_ISteamClient_BShutdownIfAllPipesClosed");
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamHTTP(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamHTTP {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamController {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamUGC {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamAppList {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMusic(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamMusic {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamMusicRemote(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamMusicRemote {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamHTMLSurface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamInventory(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamInventory {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamVideo(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamVideo {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamInput(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamInput {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }
       

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamClient_GetISteamParentalSettings(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"SteamAPI_ISteamClient_GetISteamParentalSettings {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write($"", msg);
        }
    }
}
