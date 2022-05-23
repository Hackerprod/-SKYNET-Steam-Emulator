using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Managers;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamClient : ISteamInterface
    {
        public static SteamClient Instance;

        public SteamClient()
        {
            Instance = this;
            InterfaceName = "SteamClient";
            InterfaceVersion = "SteamClient017";
        }

        public HSteamPipe CreateSteamPipe()
        {
            Write("CreateSteamPipe");
            return SteamEmulator.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
        {
            Write($"BReleaseSteamPipe {hSteamPipe}");
            return true;
        }

        public HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
        {
            Write("ConnectToGlobalUser");
            return 1;
        }

        public HSteamUser CreateLocalUser(HSteamPipe phSteamPipe, int eAccountType)
        {
            Write("CreateLocalUser");
            return SteamEmulator.CreateSteamUser();
        }

        public void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hSteamUser)
        {
            Write("ReleaseUser");
        }

        public IntPtr GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUser {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServer {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void SetLocalIPBinding(UInt32 unIP, uint usPort)
        {
            Write("SetLocalIPBinding");
        }

        public IntPtr GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamFriends {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUtils {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmaking {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmakingServers {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion)
        {
            Write($"GetISteamGenericInterface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUserStats {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServerStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServerStats {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamApps {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamNetworking {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamRemoteStorage {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamScreenshots {pchVersion}");
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
                //SteamAPIWarningMessageHook_t pFunction = Marshal.PtrToStructure<SteamAPIWarningMessageHook_t>(pFunctionPtr);
                Write($"SetWarningMessageHook");
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

        public IntPtr GetISteamHTTP(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTTP {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr DEPRECATED_GetISteamUnifiedMessages(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"DEPRECATED_GetISteamUnifiedMessages {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUnifiedMessages(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUnifiedMessages {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamController {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUGC {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamAppList {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusic(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusic {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusicRemote {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTMLSurface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr arg0)
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


        public IntPtr GetISteamInventory(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamInventory {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamVideo {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParentalSettings {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMasterServerUpdater(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMasterServerUpdater {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamContentServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamContentServer {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamGameSearch(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamFriends {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamInput {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParties {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
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
