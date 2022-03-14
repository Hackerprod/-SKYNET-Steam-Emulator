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

            m_pSteamUser = SteamClient.GetISteamUser(hSteamUser, hSteamPipe, SteamuserInterfaceVersion);
            if (m_pSteamUser == null)
                return false;

            m_pSteamFriends = SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, SteamfriendsInterfaceVersion);
            if (m_pSteamFriends == null)
                return false;

            m_pSteamUtils = SteamClient.GetISteamUtils(hSteamPipe, SteamutilsInterfaceVersion);
            if (m_pSteamUtils == null)
                return false;

            m_pSteamMatchmaking = SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, SteammatchmakingInterfaceVersion);
            if (m_pSteamMatchmaking == null)
                return false;

            m_pSteamMatchmakingServers = SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, SteammatchmakingserversInterfaceVersion);
            if (m_pSteamMatchmakingServers == null)
                return false;

            m_pSteamUserStats = SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, SteamuserstatsInterfaceVersion);
            if (m_pSteamUserStats == null)
                return false;

            m_pSteamApps = SteamClient.GetISteamApps(hSteamUser, hSteamPipe, SteamappsInterfaceVersion);
            if (m_pSteamApps == null)
                return false;

            m_pSteamNetworking = SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, SteamnetworkingInterfaceVersion);
            if (m_pSteamNetworking == null)
                return false;

            m_pSteamRemoteStorage = SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, SteamremotestorageInterfaceVersion);
            if (m_pSteamRemoteStorage == null)
                return false;

            m_pSteamScreenshots = SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, SteamscreenshotsInterfaceVersion);
            if (m_pSteamScreenshots == null)
                return false;

            m_pSteamHTTP = SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, SteamhttpInterfaceVersion);
            if (m_pSteamHTTP == null)
                return false;

            m_pController = SteamClient.GetISteamController(hSteamUser, hSteamPipe, SteamcontrollerInterfaceVersion);
            if (m_pController == null)
                return false;

            m_pSteamUGC = SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, SteamugcInterfaceVersion);
            if (m_pSteamUGC == null)
                return false;

            m_pSteamAppList = SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, SteamapplistInterfaceVersion);
            if (m_pSteamAppList == null)
                return false;

            m_pSteamMusic = SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, SteammusicInterfaceVersion);
            if (m_pSteamMusic == null)
                return false;

            m_pSteamMusicRemote = SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, SteammusicremoteInterfaceVersion);
            if (m_pSteamMusicRemote == null)
                return false;

            m_pSteamHTMLSurface = SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, SteamhtmlsurfaceInterfaceVersion);
            if (m_pSteamHTMLSurface == null)
                return false;

            m_pSteamInventory = SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, SteaminventoryInterfaceVersion);
            if (m_pSteamInventory == null)
                return false;

            m_pSteamVideo = SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, SteamvideoInterfaceVersion);
            if (m_pSteamVideo == null)
                return false;

            m_pSteamParentalSettings = SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, SteamparentalsettingsInterfaceVersion);
            if (m_pSteamParentalSettings == null)
                return false;

            return true;
        }

        internal void Clear()
        {
            SteamClient.Write("CSteamAPIContext.Clear()");
        }
        internal bool Init()
        {
            SteamClient.Write("CSteamAPIContext.Init()");
            return true;
        }
        //internal ISteamClient GetSteamClient() { return m_pSteamClient; }
        internal ISteamUser GetSteamUser() { return m_pSteamUser; }
        internal ISteamFriends GetSteamFriends() { return m_pSteamFriends; }
        internal ISteamUtils GetSteamUtils() { return m_pSteamUtils; }
        internal ISteamMatchmaking GetSteamMatchmaking() { return m_pSteamMatchmaking; }
        internal ISteamUserStats GetSteamUserStats() { return m_pSteamUserStats; }
        internal ISteamApps GetSteamApps() { return m_pSteamApps; }
        internal ISteamMatchmakingServers GetSteamMatchmakingServers() { return m_pSteamMatchmakingServers; }
        internal ISteamNetworking GetSteamNetworking() { return m_pSteamNetworking; }
        internal ISteamRemoteStorage GetSteamRemoteStorage() { return m_pSteamRemoteStorage; }
        internal ISteamScreenshots GetSteamScreenshots() { return m_pSteamScreenshots; }
        internal ISteamHTTP GetSteamHTTP() { return m_pSteamHTTP; }
        internal ISteamController GetSteamController() { return m_pController; }
        internal ISteamUGC GetSteamUGC() { return m_pSteamUGC; }
        internal ISteamAppList GetSteamAppList() { return m_pSteamAppList; }
        internal ISteamMusic GetSteamMusic() { return m_pSteamMusic; }
        internal ISteamMusicRemote GetSteamMusicRemote() { return m_pSteamMusicRemote; }
        internal ISteamHTMLSurface GetSteamHTMLSurface() { return m_pSteamHTMLSurface; }
        internal ISteamInventory GetSteamInventory() { return m_pSteamInventory; }
        internal ISteamVideo GetSteamVideo() { return m_pSteamVideo; }
        internal ISteamParentalSettings GetSteamParentalSettings() { return m_pSteamParentalSettings; }
    }

}
