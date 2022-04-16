using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Helpers;
using SKYNET.Steamworks.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamClient : ISteamInterface
    {
        public SteamClient()
        {
            InterfaceVersion = "SteamClient";
        }

        public Int32 CreateSteamPipe()
        {
            Write("CreateSteamPipe");
            return (int)SteamEmulator.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(int hSteamPipe)
        {
            Write($"BReleaseSteamPipe {hSteamPipe}");
            return true;
        }

        public int ConnectToGlobalUser(int hSteamPipe)
        {
            Write("ConnectToGlobalUser");
            return 1;
        }

        public int CreateLocalUser(int phSteamPipe, int eAccountType)
        {
            Write("CreateLocalUser");
            return (int)SteamEmulator.CreateSteamUser();
        }

        public void ReleaseUser(int hSteamPipe, int hSteamUser)
        {
            Write("ReleaseUser");
        }

        public IntPtr GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUser {pchVersion}");
            if (!pchVersion.StartsWith("SteamUser")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion); 
        }

        public IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServer {pchVersion}");
            if (!pchVersion.StartsWith("SteamGameServer")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void SetLocalIPBinding(uint unIP, uint usPort)
        {
            Write("SetLocalIPBinding");
        }

        public IntPtr GetISteamFriends(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamFriends {pchVersion}");
            if (!pchVersion.StartsWith("SteamFriends")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUtils(int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUtils {pchVersion}");
            if (!pchVersion.StartsWith("SteamUtils")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmaking {pchVersion}");
            if (!pchVersion.StartsWith("SteamMatchmaking")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmakingServers {pchVersion}");
            if (!pchVersion.StartsWith("SteamMatchmakingServers")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion)
        {
            Write($"GetISteamGenericInterface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUserStats {pchVersion}");
            if (!pchVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServerStats {pchVersion}");
            if (!pchVersion.StartsWith("SteamGameServer")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamApps {pchVersion}");
            if (!pchVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamNetworking {pchVersion}");
            if (!pchVersion.StartsWith("SteamNetworking")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamRemoteStorage {pchVersion}");
            if (!pchVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamScreenshots {pchVersion}");
            if (!pchVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void RunFrame()
        {
            Write($"RunFrame");
        }

        public uint GetIPCCallCount()
        {
            Write("GetIPCCallCount");
            return 0;
        }

        public void SetWarningMessageHook(IntPtr pFunctionPtr)
        {
            try
            {
                SteamAPIWarningMessageHook_t pFunction = Marshal.PtrToStructure<SteamAPIWarningMessageHook_t>(pFunctionPtr);
                Write($"SetWarningMessageHook Name: {pFunction.Method.Name}, ReturnType: {pFunction.Method.ReturnType}");
            }
            catch (Exception)
            {
                Write($"SetWarningMessageHook");
            }
        }

        public bool BShutdownIfAllPipesClosed()
        {
            Write("BShutdownIfAllPipesClosed");
            return false;
        }

        public IntPtr GetISteamHTTP(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTTP {pchVersion}");
            if (!pchVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr DEPRECATED_GetISteamUnifiedMessages(int hSteamuser, int hSteamPipe, string pchVersion)
        {
            Write($"DEPRECATED_GetISteamUnifiedMessages {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUnifiedMessages(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion )
        {
            Write($"GetISteamUnifiedMessages {pchVersion}");
            return IntPtr.Zero;
        }

        public IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamController {pchVersion}");
            if (!pchVersion.StartsWith("SteamController")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUGC {pchVersion}");
            if (!pchVersion.StartsWith("STEAMUGC_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamAppList {pchVersion}");
            if (!pchVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusic {pchVersion}");
            if (!pchVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusicRemote {pchVersion}");
            if (!pchVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTMLSurface {pchVersion}");
            if (!pchVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess( IntPtr arg0)
        {
            Write($"DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess");
        }

        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr arg0)
        {
            Write($"DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess");
        }

        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr arg0)
        {
            Write($"Set_SteamAPI_CCheckCallbackRegisteredInProcess");
        }

        public void Set_SteamAPI_CPostAPIResultInProcess(IntPtr arg0)
        {
            Write($"Set_SteamAPI_CPostAPIResultInProcess");
        }

        public void Remove_SteamAPI_CPostAPIResultInProcess(IntPtr arg0)
        {
            Write($"Remove_SteamAPI_CPostAPIResultInProcess");
        }


        public IntPtr GetISteamInventory(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamInventory {pchVersion}");
            if (!pchVersion.StartsWith("STEAMINVENTORY_INTERFACE_V")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamVideo {pchVersion}");
            if (!pchVersion.StartsWith("STEAMVIDEO_INTERFACE_V")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParentalSettings {pchVersion}");
            if (!pchVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMasterServerUpdater(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMasterServerUpdater {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamContentServer(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamContentServer {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamFriends {pchVersion}");
            if (!pchVersion.StartsWith("SteamFriends")) return IntPtr.Zero;
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamInput {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParties {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamRemotePlay {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void DestroyAllInterfaces()
        {
            Write($"DestroyAllInterfaces");
        }
    }
}
