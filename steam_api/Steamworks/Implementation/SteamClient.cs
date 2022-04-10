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
    [StructLayout(LayoutKind.Sequential)]
    public class SteamClient : ISteamInterface
    {
        public Int32 CreateSteamPipe(IntPtr _)
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
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion); 
        }

        public IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServer {pchVersion}");
            return SteamEmulator.SteamGameServer.MemoryAddress;
        }

        public void SetLocalIPBinding(uint unIP, uint usPort)
        {
            Write("SetLocalIPBinding");
        }

        public IntPtr GetISteamFriends(int user, int pipe, string pchVersion)
        {
            Write($"GetISteamFriends {pchVersion}");
            return SteamEmulator.SteamFriends.MemoryAddress;
        }

        public IntPtr GetISteamUtils(int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUtils {pchVersion}");
            return SteamEmulator.SteamUtils.MemoryAddress;
        }

        public IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmaking {pchVersion}");
            return SteamEmulator.SteamMatchmaking.MemoryAddress;
        }

        public IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMatchmakingServers {pchVersion}");
            return SteamEmulator.SteamMatchMakingServers.MemoryAddress;
        }

        public IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion)
        {
            Write($"GetISteamGenericInterface {pchVersion}");
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUserStats {pchVersion}");
            return SteamEmulator.SteamUserStats.MemoryAddress;
        }

        public IntPtr GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamGameServerStats {pchVersion}");
            return SteamEmulator.SteamGameServerStats.MemoryAddress;
        }

        public IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamApps {pchVersion}");
            return SteamEmulator.SteamApps.MemoryAddress;
        }

        public IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamNetworking {pchVersion}");
            return SteamEmulator.SteamNetworking.MemoryAddress;
        }

        public IntPtr GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamRemoteStorage {pchVersion}");
            return SteamEmulator.SteamRemoteStorage.MemoryAddress;
        }

        public IntPtr GetISteamScreenshots(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamScreenshots {pchVersion}");
            return SteamEmulator.SteamScreenshots.MemoryAddress;
        }

        //public IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion)
        //{
        //    Write($"GetISteamFriends {pchVersion}");
        //    return SteamEmulator.SteamGameSearch.MemoryAddress;
        //}

        public void RunFrame(IntPtr _)
        {
            Write($"RunFrame");
        }

        public uint GetIPCCallCount(IntPtr _)
        {
            Write("GetIPCCallCount");
            return 0;
        }

        public void SetWarningMessageHook(IntPtr pFunctionPtr)
        {
            SteamAPIWarningMessageHook_t pFunction = Marshal.PtrToStructure<SteamAPIWarningMessageHook_t>(pFunctionPtr);
            Write($"SetWarningMessageHook Name: {pFunction.Method.Name}, ReturnType: {pFunction.Method.ReturnType}");
        }

        public bool BShutdownIfAllPipesClosed(IntPtr _)
        {
            Write("BShutdownIfAllPipesClosed");
            return false;
        }

        public IntPtr GetISteamHTTP(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTTP {pchVersion}");

            return SteamEmulator.SteamHTTP.MemoryAddress;
        }

        public IntPtr DEPRECATED_GetISteamUnifiedMessages(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write($"DEPRECATED_GetISteamUnifiedMessages {pchVersion}");
            return IntPtr.Zero;
        }

        public IntPtr GetISteamUnifiedMessages(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion )
        {
            Write($"GetISteamUnifiedMessages {pchVersion}");
            return IntPtr.Zero;
        }

        public IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamController {pchVersion}");
            return SteamEmulator.SteamController.MemoryAddress;
        }

        public IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamUGC {pchVersion}");
            return SteamEmulator.SteamUGC.MemoryAddress;
        }

        public IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamAppList {pchVersion}");
            return SteamEmulator.SteamAppList.MemoryAddress;
        }

        public IntPtr GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusic {pchVersion}");
            return SteamEmulator.SteamMusic.MemoryAddress;
        }

        public IntPtr GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamMusicRemote {pchVersion}");
            return SteamEmulator.SteamMusicRemote.MemoryAddress;
        }

        public IntPtr GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamHTMLSurface {pchVersion}");
            return SteamEmulator.SteamHTMLSurface.MemoryAddress;
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
            return SteamEmulator.SteamInventory.MemoryAddress;
        }

        public IntPtr GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamVideo {pchVersion}");
            return SteamEmulator.SteamVideo.MemoryAddress;
        }

        public IntPtr GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParentalSettings {pchVersion}");
            return SteamEmulator.SteamParentalSettings.MemoryAddress;
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
            return SteamEmulator.SteamGameSearch.MemoryAddress;
        }

        public IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamInput {pchVersion}");
            return SteamEmulator.SteamInput.MemoryAddress;
        }

        public IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamParties {pchVersion}");
            return SteamEmulator.SteamParties.MemoryAddress;
        }

        public IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion)
        {
            Write($"GetISteamRemotePlay {pchVersion}");
            return SteamEmulator.SteamRemotePlay.MemoryAddress;
        }

        public void DestroyAllInterfaces(IntPtr _)
        {
            Write($"DestroyAllInterfaces");
        }


        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamClient()
        {
            InterfaceVersion = "SteamClient";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }

    }
}
