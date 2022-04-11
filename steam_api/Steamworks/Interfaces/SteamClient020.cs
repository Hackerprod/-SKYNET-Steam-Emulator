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
            return SteamEmulator.SteamClient.CreateSteamPipe(_);
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
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServer(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public void SetLocalIPBinding(IntPtr _, uint unIP, uint usPort)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        public IntPtr GetISteamFriends(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUtils(IntPtr _, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(pchVersion);
        }

        public IntPtr GetISteamMatchmaking(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMatchmakingServers(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGenericInterface(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUserStats(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameServerStats(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamApps(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamNetworking(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemoteStorage(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamScreenshots(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamGameSearch(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        // 	STEAM_PRIVATE_API( virtual void RunFrame() = 0; )

        public uint GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamClient.GetIPCCallCount(_);
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        }

        public bool BShutdownIfAllPipesClosed(IntPtr _)
        {
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed(_);
        }

        public IntPtr GetISteamHTTP(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        // 	STEAM_PRIVATE_API( virtual void *DEPRECATED_GetISteamUnifiedMessages( HSteamUser hSteamuser, HSteamPipe hSteamPipe, const char *pchVersion ) = 0 ; )

        public IntPtr GetISteamController(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamUGC(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamAppList(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusic(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamMusicRemote(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamHTMLSurface(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        // 	STEAM_PRIVATE_API( virtual void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        // 	STEAM_PRIVATE_API( virtual void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess( void (*)() ) = 0; )

        // 	STEAM_PRIVATE_API( virtual void Set_SteamAPI_CCheckCallbackRegisteredInProcess( SteamAPI_CheckCallbackRegistered_t func ) = 0; )

        public IntPtr GetISteamInventory(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamVideo(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParentalSettings(IntPtr _, int hSteamuser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamuser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamInput(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamParties(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public IntPtr GetISteamRemotePlay(IntPtr _, int hSteamUser, int hSteamPipe, string pchVersion)
        {
            return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
        }


    }
}
