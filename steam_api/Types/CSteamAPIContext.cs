using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class CSteamAPIContext // TypeDefIndex: 4364
    {
        //public ISteamClient m_pSteamClient;
        public ISteamUser m_pSteamUser;
        public ISteamFriends m_pSteamFriends;
        public ISteamUtils m_pSteamUtils;
        public ISteamMatchmaking m_pSteamMatchmaking;
        public ISteamUserStats m_pSteamUserStats;
        public ISteamApps m_pSteamApps;
        public ISteamMatchmakingServers m_pSteamMatchmakingServers;
        public ISteamNetworking m_pSteamNetworking;
        public ISteamRemoteStorage m_pSteamRemoteStorage;
        public ISteamScreenshots m_pSteamScreenshots;
        public ISteamHTTP m_pSteamHTTP;
        public ISteamController m_pController;
        public ISteamUGC m_pSteamUGC;
        public ISteamAppList m_pSteamAppList;
        public ISteamMusic m_pSteamMusic;
        public ISteamMusicRemote m_pSteamMusicRemote;
        public ISteamHTMLSurface m_pSteamHTMLSurface;
        public ISteamInventory m_pSteamInventory;
        public ISteamVideo m_pSteamVideo;
        public ISteamParentalSettings m_pSteamParentalSettings;

        private const string SteamclientInterfaceVersion = "SteamClient017";
        private const string SteamuserInterfaceVersion = "SteamUser019";
        private const string SteamfriendsInterfaceVersion = "SteamFriends015";
        private const string SteamutilsInterfaceVersion = "SteamUtils009";
        private const string SteammatchmakingInterfaceVersion = "SteamMatchMaking009";
        private const string SteammatchmakingserversInterfaceVersion = "SteamMatchMakingServers002";
        private const string SteamuserstatsInterfaceVersion = "STEAMUSERSTATS_INTERFACE_VERSION011";
        private const string SteamappsInterfaceVersion = "STEAMAPPS_INTERFACE_VERSION008";
        private const string SteamnetworkingInterfaceVersion = "SteamNetworking005";
        private const string SteamremotestorageInterfaceVersion = "STEAMREMOTESTORAGE_INTERFACE_VERSION014";
        private const string SteamscreenshotsInterfaceVersion = "STEAMSCREENSHOTS_INTERFACE_VERSION003";
        private const string SteamhttpInterfaceVersion = "STEAMHTTP_INTERFACE_VERSION002";
        private const string SteamcontrollerInterfaceVersion = "SteamController006";
        private const string SteamugcInterfaceVersion = "STEAMUGC_INTERFACE_VERSION010";
        private const string SteamapplistInterfaceVersion = "STEAMAPPLIST_INTERFACE_VERSION001";
        private const string SteammusicInterfaceVersion = "STEAMMUSIC_INTERFACE_VERSION001";
        private const string SteammusicremoteInterfaceVersion = "STEAMMUSICREMOTE_INTERFACE_VERSION001";
        private const string SteamhtmlsurfaceInterfaceVersion = "STEAMHTMLSURFACE_INTERFACE_VERSION_004";
        private const string SteaminventoryInterfaceVersion = "STEAMINVENTORY_INTERFACE_V002";
        private const string SteamvideoInterfaceVersion = "STEAMVIDEO_INTERFACE_V002";
        private const string SteamparentalsettingsInterfaceVersion = "STEAMPARENTALSETTINGS_INTERFACE_VERSION001";

        public bool Initialize()
        {
            var hSteamUser = SteamAPI.SteamAPI_GetHSteamUser();

            var hSteamPipe = SteamAPI.SteamAPI_GetHSteamPipe();

            SteamAPI.Write($"HSteamPipe: {hSteamPipe}, returning");

            if (hSteamPipe.m_HSteamPipe == 0)
                return false;

            //m_pSteamClient = SteamClient.Instance;
            //if (m_pSteamClient == null)
            //    return false;

            m_pSteamUser = SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, SteamuserInterfaceVersion);
            if (m_pSteamUser == null)
                return false;

            m_pSteamFriends = SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, SteamfriendsInterfaceVersion);
            if (m_pSteamFriends == null)
                return false;

            m_pSteamUtils = SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, SteamutilsInterfaceVersion);
            if (m_pSteamUtils == null)
                return false;

            m_pSteamMatchmaking = SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, SteammatchmakingInterfaceVersion);
            if (m_pSteamMatchmaking == null)
                return false;

            m_pSteamMatchmakingServers = SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, SteammatchmakingserversInterfaceVersion);
            if (m_pSteamMatchmakingServers == null)
                return false;

            m_pSteamUserStats = SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, SteamuserstatsInterfaceVersion);
            if (m_pSteamUserStats == null)
                return false;

            m_pSteamApps = SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, SteamappsInterfaceVersion);
            if (m_pSteamApps == null)
                return false;

            m_pSteamNetworking = SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, SteamnetworkingInterfaceVersion);
            if (m_pSteamNetworking == null)
                return false;

            m_pSteamRemoteStorage = SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, SteamremotestorageInterfaceVersion);
            if (m_pSteamRemoteStorage == null)
                return false;

            m_pSteamScreenshots = SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, SteamscreenshotsInterfaceVersion);
            if (m_pSteamScreenshots == null)
                return false;

            m_pSteamHTTP = SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, SteamhttpInterfaceVersion);
            if (m_pSteamHTTP == null)
                return false;

            m_pController = SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, SteamcontrollerInterfaceVersion);
            if (m_pController == null)
                return false;

            m_pSteamUGC = SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, SteamugcInterfaceVersion);
            if (m_pSteamUGC == null)
                return false;

            m_pSteamAppList = SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, SteamapplistInterfaceVersion);
            if (m_pSteamAppList == null)
                return false;

            m_pSteamMusic = SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, SteammusicInterfaceVersion);
            if (m_pSteamMusic == null)
                return false;

            m_pSteamMusicRemote = SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, SteammusicremoteInterfaceVersion);
            if (m_pSteamMusicRemote == null)
                return false;

            m_pSteamHTMLSurface = SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, SteamhtmlsurfaceInterfaceVersion);
            if (m_pSteamHTMLSurface == null)
                return false;

            m_pSteamInventory = SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, SteaminventoryInterfaceVersion);
            if (m_pSteamInventory == null)
                return false;

            m_pSteamVideo = SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, SteamvideoInterfaceVersion);
            if (m_pSteamVideo == null)
                return false;

            m_pSteamParentalSettings = SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, SteamparentalsettingsInterfaceVersion);
            if (m_pSteamParentalSettings == null)
                return false;

            return true;
        }

        public void Clear()
        {
            SteamEmulator.Write("CSteamAPIContext.Clear()");
        }
        public bool Init()
        {
            SteamEmulator.Write("CSteamAPIContext.Init()");
            return true;
        }
        //public ISteamClient GetSteamClient() { return m_pSteamClient; }
        public ISteamUser GetSteamUser() { return m_pSteamUser; }
        public ISteamFriends GetSteamFriends() { return m_pSteamFriends; }
        public ISteamUtils GetSteamUtils() { return m_pSteamUtils; }
        public ISteamMatchmaking GetSteamMatchmaking() { return m_pSteamMatchmaking; }
        public ISteamUserStats GetSteamUserStats() { return m_pSteamUserStats; }
        public ISteamApps GetSteamApps() { return m_pSteamApps; }
        public ISteamMatchmakingServers GetSteamMatchmakingServers() { return m_pSteamMatchmakingServers; }
        public ISteamNetworking GetSteamNetworking() { return m_pSteamNetworking; }
        public ISteamRemoteStorage GetSteamRemoteStorage() { return m_pSteamRemoteStorage; }
        public ISteamScreenshots GetSteamScreenshots() { return m_pSteamScreenshots; }
        public ISteamHTTP GetSteamHTTP() { return m_pSteamHTTP; }
        public ISteamController GetSteamController() { return m_pController; }
        public ISteamUGC GetSteamUGC() { return m_pSteamUGC; }
        public ISteamAppList GetSteamAppList() { return m_pSteamAppList; }
        public ISteamMusic GetSteamMusic() { return m_pSteamMusic; }
        public ISteamMusicRemote GetSteamMusicRemote() { return m_pSteamMusicRemote; }
        public ISteamHTMLSurface GetSteamHTMLSurface() { return m_pSteamHTMLSurface; }
        public ISteamInventory GetSteamInventory() { return m_pSteamInventory; }
        public ISteamVideo GetSteamVideo() { return m_pSteamVideo; }
        public ISteamParentalSettings GetSteamParentalSettings() { return m_pSteamParentalSettings; }
    }

}
