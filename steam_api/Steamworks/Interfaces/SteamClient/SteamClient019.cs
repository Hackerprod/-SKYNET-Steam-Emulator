using System;
using System.Runtime.InteropServices;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamClient019")]
    public class SteamClient019 : ISteamInterface
    {
        public HSteamPipe CreateSteamPipe(IntPtr _) => SteamEmulator.SteamClient.CreateSteamPipe();
        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe) => SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        public HSteamUser ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe) => SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        public HSteamUser CreateLocalUser(IntPtr _, IntPtr phSteamPipe, int eAccountType) => CreateLocalUserAndWritePipe(phSteamPipe, eAccountType);
        public void ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser) => SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        public IntPtr GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        public void SetLocalIPBinding(IntPtr _, uint unIP, ushort usPort) => SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        public IntPtr GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        public IntPtr GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamScreenshots(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameSearch(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameSearch(hSteamUser, hSteamPipe, pchVersion);
        public void RunFrame(IntPtr _) => SteamEmulator.SteamClient.RunFrame();
        public uint GetIPCCallCount(IntPtr _) => SteamEmulator.SteamClient.GetIPCCallCount();
        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction) => SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        public bool BShutdownIfAllPipesClosed(IntPtr _) => SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        public IntPtr GetISteamHTTP(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr DEPRECATED_GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMusic(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMusicRemote(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, pchVersion);
        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(arg0);
        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(arg0);
        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(arg0);
        public IntPtr GetISteamInventory(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamVideo(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamParentalSettings(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamInput(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamInput(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamParties(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamParties(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamRemotePlay(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, pchVersion);

        private static HSteamUser CreateLocalUserAndWritePipe(IntPtr phSteamPipe, int eAccountType)
        {
            HSteamPipe pipe = SteamEmulator.CreateSteamPipe();
            if (phSteamPipe != IntPtr.Zero)
            {
                Marshal.WriteInt32(phSteamPipe, unchecked((int)pipe));
            }
            return SteamEmulator.SteamClient.CreateLocalUser(pipe, eAccountType);
        }
    }
}
