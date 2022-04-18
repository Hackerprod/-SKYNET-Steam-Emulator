using SKYNET.Interface;
using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamClient020")]
    public class SteamClient020 : ISteamInterface
    {
        public int CreateSteamPipe(IntPtr _)
        {
            return SteamEmulator.SteamClient.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(IntPtr _, int hSteamPipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        public int ConnectToGlobalUser(IntPtr _, int hSteamPipe)
        {
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        public int CreateLocalUser(IntPtr _, int phSteamPipe, int eAccountType)
        {
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, eAccountType);
        }

        public void ReleaseUser(IntPtr _, int hSteamPipe, int hUser)
        {
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        public IntPtr GetISteamUser(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServer(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        }

        public void SetLocalIPBinding(IntPtr _, uint unIP, uint usPort)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        public IntPtr GetISteamFriends(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUtils(IntPtr _, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmaking(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmakingServers(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        }

        public void GetISteamGenericInterface(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUserStats(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServerStats(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameSearch(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameSearch(hSteamuser, hSteamPipe, pchVersion);
        }

        #region STEAM_PRIVATE_API( virtual void RunFrame() = 0; )

        private void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamClient.RunFrame();
        }

        #endregion

        public uint GetIPCCallCount(IntPtr _)
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

        public IntPtr GetISteamHTTP(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
        }

        #region STEAM_PRIVATE_API( virtual void *DEPRECATED_GetISteamUnifiedMessages( HSteamUser hSteamuser, HSteamPipe hSteamPipe, const char *pchVersion ) = 0 ; )

        private void DEPRECATED_GetISteamUnifiedMessages(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamuser, hSteamPipe, pchVersion);
        }

        #endregion

        public IntPtr GetISteamController(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUGC(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamAppList(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusic(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
        }

        #region STEAM_PRIVATE_API( virtual void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        private void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr Arg)
        {
            SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(Arg);
        }

        #endregion

        #region STEAM_PRIVATE_API( virtual void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        private void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr Arg)
        {
            SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(Arg);
        }

        #endregion

        #region STEAM_PRIVATE_API( virtual void Set_SteamAPI_CCheckCallbackRegisteredInProcess( SteamAPI_CheckCallbackRegistered_t func ) = 0; )

        private void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr Arg)
        {
            SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(Arg);
        }

        #endregion

        public IntPtr GetISteamInventory(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamInput(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInput(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParties(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParties(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemotePlay(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, pchVersion);
        }

        #region STEAM_PRIVATE_API( virtual void DestroyAllInterfaces() = 0; )

        private void DestroyAllInterfaces(IntPtr _)
        {
            SteamEmulator.SteamClient.DestroyAllInterfaces();
        }

        #endregion

    }
}
