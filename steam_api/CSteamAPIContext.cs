//   !!  // Steamworks.Core - CSteamAPIContext.cs
// *.-". // Created: 2016-10-17 [6:45 PM]
//  | |  // Copyright 2016 // MIT License // The Fox Council 
// Modified by: Fox Diller on 2016-10-22 @ 3:02 PM

#region Usings

using SKYNET;
using SKYNET.Helper;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Steamworks.Core
{
    // CSteamAPIContext encapsulates the Steamworks API global accessors into
    // a single object.
    //
    // DEPRECATED: Used the global interface accessors instead!
    //
    // This will be removed in a future iteration of the SDK

    [StructLayout(LayoutKind.Sequential)]
    public struct CSteamApiContext
    {
        private IntPtr m_pSteamClient;              //ISteamClient* 
        private IntPtr m_pSteamUser;                //ISteamUser* 
        private IntPtr m_pSteamFriends;             //ISteamFriends* 
        private IntPtr m_pSteamUtils;               //ISteamUtils* 
        private IntPtr m_pSteamMatchmaking;         //ISteamMatchmaking* 
        private IntPtr m_pSteamGameSearch;          //ISteamGameSearch* 
        private IntPtr m_pSteamUserStats;           //ISteamUserStats* 
        private IntPtr m_pSteamApps;                //ISteamApps* 
        private IntPtr m_pSteamMatchmakingServers;  //ISteamMatchmakingServers* 
        private IntPtr m_pSteamNetworking;          //ISteamNetworking* 
        private IntPtr m_pSteamRemoteStorage;       //ISteamRemoteStorage* 
        private IntPtr m_pSteamScreenshots;         //ISteamScreenshots* 
        private IntPtr m_pSteamHTTP;                //ISteamHTTP* 
        private IntPtr m_pSteamController;          //ISteamController* 
        private IntPtr m_pSteamUGC;                 //ISteamUGC* 
        private IntPtr m_pSteamAppList;             //ISteamAppList* 
        private IntPtr m_pSteamMusic;               //ISteamMusic* 
        private IntPtr m_pSteamMusicRemote;         //ISteamMusicRemote* 
        private IntPtr m_pSteamHTMLSurface;         //ISteamHTMLSurface* 
        private IntPtr m_pSteamInventory;           //ISteamInventory* 
        private IntPtr m_pSteamVideo;               //ISteamVideo* 
        private IntPtr m_pSteamTV;                  //ISteamTV* 
        private IntPtr m_pSteamParentalSettings;    //ISteamParentalSettings* 
        private IntPtr m_pSteamInput;               //ISteamInput* 

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

            m_pSteamClient =                IntPtr.Zero;
            m_pSteamUser =                  IntPtr.Zero;
            m_pSteamFriends =               IntPtr.Zero;
            m_pSteamUtils =                 IntPtr.Zero;
            m_pSteamMatchmaking =           IntPtr.Zero;
            m_pSteamUserStats =             IntPtr.Zero;
            m_pSteamApps =                  IntPtr.Zero;
            m_pSteamMatchmakingServers =    IntPtr.Zero;
            m_pSteamNetworking =            IntPtr.Zero;
            m_pSteamRemoteStorage =         IntPtr.Zero;
            m_pSteamScreenshots =           IntPtr.Zero;
            m_pSteamHTTP =                  IntPtr.Zero;
            m_pSteamController =            IntPtr.Zero;
            m_pSteamUGC =                   IntPtr.Zero;
            m_pSteamAppList =               IntPtr.Zero;
            m_pSteamMusic =                 IntPtr.Zero;
            m_pSteamMusicRemote =           IntPtr.Zero;
            m_pSteamHTMLSurface =           IntPtr.Zero;
            m_pSteamInventory =             IntPtr.Zero;
            m_pSteamVideo =                 IntPtr.Zero;

            SteamEmulator.Write($"CSteamApiContext cleaned");
        }

        public bool Init()
        {
            SteamEmulator.Write($"Initializing CSteamApiContext");

            var a_steamUser = SteamEmulator.HSteamUser;
            var a_steamPipe = SteamEmulator.HSteamPipe;

            if ((int)a_steamPipe == 0)
            {
                return false;
            }

            m_pSteamClient = MemoryHelper.MemoryAddress(SteamEmulator.SteamClient);
            if (m_pSteamClient == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUser = MemoryHelper.MemoryAddress(SteamEmulator.SteamUser);
            if (m_pSteamUser == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamFriends = MemoryHelper.MemoryAddress(SteamEmulator.SteamFriends);
            if (m_pSteamFriends == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUtils = MemoryHelper.MemoryAddress(SteamEmulator.SteamUtils);
            if (m_pSteamUtils == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmaking = MemoryHelper.MemoryAddress(SteamEmulator.SteamMatchmaking);
            if (m_pSteamMatchmaking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmakingServers = MemoryHelper.MemoryAddress(SteamEmulator.SteamMatchmakingServers);
            if (m_pSteamMatchmakingServers == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUserStats = MemoryHelper.MemoryAddress(SteamEmulator.SteamUserStats);
            if (m_pSteamUserStats == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamApps = MemoryHelper.MemoryAddress(SteamEmulator.SteamApps);
            if (m_pSteamApps == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamNetworking = MemoryHelper.MemoryAddress(SteamEmulator.SteamNetworking);
            if (m_pSteamNetworking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamRemoteStorage = MemoryHelper.MemoryAddress(SteamEmulator.SteamRemoteStorage);
            if (m_pSteamRemoteStorage == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamScreenshots = MemoryHelper.MemoryAddress(SteamEmulator.SteamScreenshots);
            if (m_pSteamScreenshots == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTTP = MemoryHelper.MemoryAddress(SteamEmulator.SteamHTTP);
            if (m_pSteamHTTP == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamController = MemoryHelper.MemoryAddress(SteamEmulator.SteamController);
            if (m_pSteamController == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUGC = MemoryHelper.MemoryAddress(SteamEmulator.SteamUGC);
            if (m_pSteamUGC == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamAppList = MemoryHelper.MemoryAddress(SteamEmulator.SteamAppList);
            if (m_pSteamAppList == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusic = MemoryHelper.MemoryAddress(SteamEmulator.SteamMusic);
            if (m_pSteamMusic == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusicRemote = MemoryHelper.MemoryAddress(SteamEmulator.SteamMusicRemote);
            if (m_pSteamMusicRemote == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTMLSurface = MemoryHelper.MemoryAddress(SteamEmulator.SteamHTMLSurface);
            if (m_pSteamHTMLSurface == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamInventory = MemoryHelper.MemoryAddress(SteamEmulator.SteamInventory);
            if (m_pSteamInventory == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamVideo = MemoryHelper.MemoryAddress(SteamEmulator.SteamVideo);
            if (m_pSteamVideo == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }


        public const string STEAMCLIENT_INTERFACE_VERSION = "SteamClient017";
        public const string STEAMUSER_INTERFACE_VERSION = "SteamUser019";
        public const string STEAMFRIENDS_INTERFACE_VERSION = "SteamFriends015";
        public const string STEAMUTILS_INTERFACE_VERSION = "SteamUtils008";
        public const string STEAMMATCHMAKING_INTERFACE_VERSION = "SteamMatchMaking009";
        public const string STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION = "SteamMatchMakingServers002";
        public const string STEAMUSERSTATS_INTERFACE_VERSION = "STEAMUSERSTATS_INTERFACE_VERSION011";
        public const string STEAMAPPS_INTERFACE_VERSION = "STEAMAPPS_INTERFACE_VERSION008";
        public const string STEAMNETWORKING_INTERFACE_VERSION = "SteamNetworking005";
        public const string STEAMREMOTESTORAGE_INTERFACE_VERSION = "STEAMREMOTESTORAGE_INTERFACE_VERSION014";
        public const string STEAMSCREENSHOTS_INTERFACE_VERSION = "STEAMSCREENSHOTS_INTERFACE_VERSION003";
        public const string STEAMHTTP_INTERFACE_VERSION = "STEAMHTTP_INTERFACE_VERSION002";
        public const string STEAMUNIFIEDMESSAGES_INTERFACE_VERSION = "STEAMUNIFIEDMESSAGES_INTERFACE_VERSION001";
        public const string STEAMCONTROLLER_INTERFACE_VERSION = "SteamController004";
        public const string STEAMUGC_INTERFACE_VERSION = "STEAMUGC_INTERFACE_VERSION009";
        public const string STEAMAPPLIST_INTERFACE_VERSION = "STEAMAPPLIST_INTERFACE_VERSION001";
        public const string STEAMMUSIC_INTERFACE_VERSION = "STEAMMUSIC_INTERFACE_VERSION001";
        public const string STEAMMUSICREMOTE_INTERFACE_VERSION = "STEAMMUSICREMOTE_INTERFACE_VERSION001";
        public const string STEAMHTMLSURFACE_INTERFACE_VERSION = "STEAMHTMLSURFACE_INTERFACE_VERSION_003";
        public const string STEAMINVENTORY_INTERFACE_VERSION = "STEAMINVENTORY_INTERFACE_V001";
        public const string STEAMVIDEO_INTERFACE_VERSION = "STEAMVIDEO_INTERFACE_V001";
    }
}