using System;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using uint16 = System.UInt16;
using uint32 = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamClient017")]
    public class SteamClient017 : ISteamInterface
    {
        public HSteamPipe CreateSteamPipe(IntPtr _)
        {
            return SteamEmulator.SteamClient.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        public HSteamUser ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        public HSteamUser CreateLocalUser(IntPtr _, HSteamPipe phSteamPipe, int eAccountType)
        {
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, eAccountType);
        }

        public void ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        public IntPtr GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        }

        public void SetLocalIPBinding(IntPtr _, uint32 unIP, uint16 usPort)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        public IntPtr GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        }

        public void GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
        }

        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamClient.RunFrame();
        }

        public uint32 GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamClient.GetIPCCallCount();
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        }

        public bool BShutdownIfAllPipesClosed(IntPtr _)
        {
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        }

        public IntPtr GetISteamHTTP(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
        }

        public void DEPRECATED_GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusic(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
        }

        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr msg )
        {
            SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(msg);
        }

        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr msg)
        {
            SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(msg);
        }

        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr func)
        {
            SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(func);
        }

        public IntPtr GetISteamInventory(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamuser, hSteamPipe, pchVersion);
        }
    }
}
