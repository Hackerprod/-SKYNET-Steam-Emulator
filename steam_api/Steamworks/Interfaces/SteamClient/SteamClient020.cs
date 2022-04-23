using SKYNET.Interface;
using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Interface
{
    [Interface("SteamClient020")]
    public class SteamClient020 : ISteamInterface
    {
        public int CreateSteamPipe(IntPtr _)
        {
            return SteamEmulator.SteamClient.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        public int ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe)
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

        public void SetLocalIPBinding(IntPtr _, uint unIP, uint usPort)
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

        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameSearch(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameSearch(hSteamUser, hSteamPipe, pchVersion);
        }

        //#region STEAM_PRIVATE_API( virtual void RunFrame() = 0; )

        //private void RunFrame(IntPtr _)
        //{
        //    SteamEmulator.SteamClient.RunFrame();
        //}

        //#endregion

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

        public IntPtr GetISteamHTTP(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, pchVersion);
        }

        //#region STEAM_PRIVATE_API( virtual void *DEPRECATED_GetISteamUnifiedMessages( HSteamUser hSteamUser, HSteamPipe hSteamPipe, const char *pchVersion ) = 0 ; )

        //private void DEPRECATED_GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        //{
        //    SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamUser, hSteamPipe, pchVersion);
        //}

        //#endregion

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

        public IntPtr GetISteamMusic(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, pchVersion);
        }

        //#region STEAM_PRIVATE_API( virtual void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        //private void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr Arg)
        //{
        //    SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(Arg);
        //}

        //#endregion

        //#region STEAM_PRIVATE_API( virtual void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        //private void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr Arg)
        //{
        //    SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(Arg);
        //}

        //#endregion

        //#region STEAM_PRIVATE_API( virtual void Set_SteamAPI_CCheckCallbackRegisteredInProcess( SteamAPI_CheckCallbackRegistered_t func ) = 0; )

        //private void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr Arg)
        //{
        //    SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(Arg);
        //}

        //#endregion

        public IntPtr GetISteamInventory(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamInput(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInput(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParties(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParties(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemotePlay(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, pchVersion);
        }

        //#region STEAM_PRIVATE_API( virtual void DestroyAllInterfaces() = 0; )

        //private void DestroyAllInterfaces(IntPtr _)
        //{
        //    SteamEmulator.SteamClient.DestroyAllInterfaces();
        //}

        //#endregion

    }
}
