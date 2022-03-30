using SKYNET;
using SKYNET.Helper;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CSteamApiContext
    {
        public IntPtr m_pSteamClient;              //ISteamClient* 
        public IntPtr m_pSteamUser;                //ISteamUser* 
        public IntPtr m_pSteamFriends;             //ISteamFriends* 
        public IntPtr m_pSteamUtils;               //ISteamUtils* 
        public IntPtr m_pSteamMatchmaking;         //ISteamMatchmaking* 
        public IntPtr m_pSteamGameSearch;          //ISteamGameSearch* 
        public IntPtr m_pSteamUserStats;           //ISteamUserStats* 
        public IntPtr m_pSteamApps;                //ISteamApps* 
        public IntPtr m_pSteamMatchmakingServers;  //ISteamMatchmakingServers* 
        public IntPtr m_pSteamNetworking;          //ISteamNetworking* 
        public IntPtr m_pSteamRemoteStorage;       //ISteamRemoteStorage* 
        public IntPtr m_pSteamScreenshots;         //ISteamScreenshots* 
        public IntPtr m_pSteamHTTP;                //ISteamHTTP* 
        public IntPtr m_pSteamController;          //ISteamController* 
        public IntPtr m_pSteamUGC;                 //ISteamUGC* 
        public IntPtr m_pSteamAppList;             //ISteamAppList* 
        public IntPtr m_pSteamMusic;               //ISteamMusic* 
        public IntPtr m_pSteamMusicRemote;         //ISteamMusicRemote* 
        public IntPtr m_pSteamHTMLSurface;         //ISteamHTMLSurface* 
        public IntPtr m_pSteamInventory;           //ISteamInventory* 
        public IntPtr m_pSteamVideo;               //ISteamVideo* 
        public IntPtr m_pSteamTV;                  //ISteamTV* 
        public IntPtr m_pSteamParentalSettings;    //ISteamParentalSettings* 
        public IntPtr m_pSteamInput;               //ISteamInput* 

        public IntPtr SteamClient() => m_pSteamClient;
        public IntPtr SteamUser() => m_pSteamUser;
        public IntPtr SteamFriends() => m_pSteamFriends;
        public IntPtr SteamUtils() => m_pSteamUtils;
        public IntPtr SteamMatchmaking() => m_pSteamMatchmaking;
        public IntPtr SteamGameSearch() => m_pSteamGameSearch;
        public IntPtr SteamUserStats() => m_pSteamUserStats;
        public IntPtr SteamApps() => m_pSteamApps;
        public IntPtr SteamMatchmakingServers() => m_pSteamMatchmakingServers;
        public IntPtr SteamNetworking() => m_pSteamNetworking;
        public IntPtr SteamRemoteStorage() => m_pSteamRemoteStorage;
        public IntPtr SteamScreenshots() => m_pSteamScreenshots;
        public IntPtr SteamHTTP() => m_pSteamHTTP;
        public IntPtr SteamController() => m_pSteamController;
        public IntPtr SteamUGC() => m_pSteamUGC;
        public IntPtr SteamAppList() => m_pSteamAppList;
        public IntPtr SteamMusic() => m_pSteamMusic;
        public IntPtr SteamMusicRemote() => m_pSteamMusicRemote;
        public IntPtr SteamHTMLSurface() => m_pSteamHTMLSurface;
        public IntPtr SteamInventory() => m_pSteamInventory;
        public IntPtr SteamVideo() => m_pSteamVideo;
        public IntPtr SteamTV() => m_pSteamTV;
        public IntPtr SteamParentalSettings() => m_pSteamParentalSettings;
        public IntPtr SteamInput() => m_pSteamInput;

        public void Clear()
        {
            SteamEmulator.Write($"Cleaning CSteamApiContext");

            m_pSteamClient = IntPtr.Zero;
            m_pSteamUser = IntPtr.Zero;
            m_pSteamFriends = IntPtr.Zero;
            m_pSteamUtils = IntPtr.Zero;
            m_pSteamMatchmaking = IntPtr.Zero;
            m_pSteamUserStats = IntPtr.Zero;
            m_pSteamApps = IntPtr.Zero;
            m_pSteamMatchmakingServers = IntPtr.Zero;
            m_pSteamNetworking = IntPtr.Zero;
            m_pSteamRemoteStorage = IntPtr.Zero;
            m_pSteamScreenshots = IntPtr.Zero;
            m_pSteamHTTP = IntPtr.Zero;
            m_pSteamController = IntPtr.Zero;
            m_pSteamUGC = IntPtr.Zero;
            m_pSteamAppList = IntPtr.Zero;
            m_pSteamMusic = IntPtr.Zero;
            m_pSteamMusicRemote = IntPtr.Zero;
            m_pSteamHTMLSurface = IntPtr.Zero;
            m_pSteamInventory = IntPtr.Zero;
            m_pSteamVideo = IntPtr.Zero;

            SteamEmulator.Write($"CSteamApiContext cleaned");
        }

        public bool Init()
        {

            m_pSteamClient = SteamEmulator.SteamClient.BaseAddress;
            if (m_pSteamClient == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUser = SteamEmulator.SteamUser.BaseAddress;
            if (m_pSteamUser == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamFriends = SteamEmulator.SteamFriends.BaseAddress;
            if (m_pSteamFriends == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUtils = SteamEmulator.SteamUtils.BaseAddress;
            if (m_pSteamUtils == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmaking = SteamEmulator.SteamMatchmaking.BaseAddress;
            if (m_pSteamMatchmaking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmakingServers = SteamEmulator.SteamMatchMakingServers.BaseAddress;
            if (m_pSteamMatchmakingServers == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUserStats = SteamEmulator.SteamUserStats.BaseAddress;
            if (m_pSteamUserStats == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamApps = SteamEmulator.SteamApps.BaseAddress;
            if (m_pSteamApps == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamNetworking = SteamEmulator.SteamNetworking.BaseAddress;
            if (m_pSteamNetworking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamRemoteStorage = SteamEmulator.SteamMusicRemote.BaseAddress;
            if (m_pSteamRemoteStorage == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamScreenshots = SteamEmulator.SteamScreenshots.BaseAddress;
            if (m_pSteamScreenshots == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTTP = SteamEmulator.SteamHTTP.BaseAddress;
            if (m_pSteamHTTP == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamController = SteamEmulator.SteamController.BaseAddress;
            if (m_pSteamController == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUGC = SteamEmulator.SteamUGC.BaseAddress;
            if (m_pSteamUGC == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamAppList = SteamEmulator.SteamAppList.BaseAddress;
            if (m_pSteamAppList == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusic = SteamEmulator.SteamMusic.BaseAddress;
            if (m_pSteamMusic == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusicRemote = SteamEmulator.SteamMusicRemote.BaseAddress;
            if (m_pSteamMusicRemote == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTMLSurface = SteamEmulator.SteamHTMLSurface.BaseAddress;
            if (m_pSteamHTMLSurface == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamInventory = SteamEmulator.SteamInventory.BaseAddress;
            if (m_pSteamInventory == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamVideo = SteamEmulator.SteamVideo.BaseAddress;
            if (m_pSteamVideo == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }
    }
    //public unsafe class CSteamAPIContext
    //{
    //    public void Clear()
    //    {
         
    //    }
    //    public bool Init()
    //    {
    //        m_pSteamClient = &SteamEmulator.SteamClient;
    //        return true;
    //    }
    //    public unsafe SteamClient* SteamClient() { return m_pSteamClient; }
    //    public SteamUser* SteamUser() { return m_pSteamUser; }
    //    public SteamFriends* SteamFriends() { return m_pSteamFriends; }
    //    public SteamUtils* SteamUtils() { return m_pSteamUtils; }
    //    public SteamMatchmaking* SteamMatchmaking() { return m_pSteamMatchmaking; }
    //    public SteamGameSearch* SteamGameSearch() { return m_pSteamGameSearch; }
    //    public SteamUserStats* SteamUserStats() { return m_pSteamUserStats; }
    //    public SteamApps* SteamApps() { return m_pSteamApps; }
    //    public SteamMatchMakingServers* SteamMatchmakingServers() { return m_pSteamMatchmakingServers; }
    //    public SteamNetworking* SteamNetworking() { return m_pSteamNetworking; }
    //    public SteamRemoteStorage* SteamRemoteStorage() { return m_pSteamRemoteStorage; }
    //    public SteamScreenshots* SteamScreenshots() { return m_pSteamScreenshots; }
    //    public SteamHTTP* SteamHTTP() { return m_pSteamHTTP; }
    //    public SteamController* SteamController() { return m_pController; }
    //    public SteamUGC* SteamUGC() { return m_pSteamUGC; }
    //    public SteamAppList* SteamAppList() { return m_pSteamAppList; }
    //    public SteamMusic* SteamMusic() { return m_pSteamMusic; }
    //    public SteamMusicRemote* SteamMusicRemote() { return m_pSteamMusicRemote; }
    //    public SteamHTMLSurface* SteamHTMLSurface() { return m_pSteamHTMLSurface; }
    //    public SteamInventory* SteamInventory() { return m_pSteamInventory; }
    //    public SteamVideo* SteamVideo() { return m_pSteamVideo; }
    //    public SteamTV* SteamTV() { return m_pSteamTV; }
    //    public SteamParentalSettings* SteamParentalSettings() { return m_pSteamParentalSettings; }
    //    public SteamInput* SteamInput() { return m_pSteamInput; }

    //    private SteamClient* m_pSteamClient;
    //    private SteamUser* m_pSteamUser;
    //    private SteamFriends* m_pSteamFriends;
    //    private SteamUtils* m_pSteamUtils;
    //    private SteamMatchmaking* m_pSteamMatchmaking;
    //    private SteamGameSearch* m_pSteamGameSearch;
    //    private SteamUserStats* m_pSteamUserStats;
    //    private SteamApps* m_pSteamApps;
    //    private SteamMatchMakingServers* m_pSteamMatchmakingServers;
    //    private SteamNetworking* m_pSteamNetworking;
    //    private SteamRemoteStorage* m_pSteamRemoteStorage;
    //    private SteamScreenshots* m_pSteamScreenshots;
    //    private SteamHTTP* m_pSteamHTTP;
    //    private SteamController* m_pController;
    //    private SteamUGC* m_pSteamUGC;
    //    private SteamAppList* m_pSteamAppList;
    //    private SteamMusic* m_pSteamMusic;
    //    private SteamMusicRemote* m_pSteamMusicRemote;
    //    private SteamHTMLSurface* m_pSteamHTMLSurface;
    //    private SteamInventory* m_pSteamInventory;
    //    private SteamVideo* m_pSteamVideo;
    //    private SteamTV* m_pSteamTV;
    //    private SteamParentalSettings* m_pSteamParentalSettings;
    //    private SteamInput* m_pSteamInput;
    //}
}
