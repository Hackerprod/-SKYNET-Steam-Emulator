using SKYNET.Callback;
using SKYNET.GUI;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;
using Steamworks.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class Test
    {
        public string test;
    }
    public class SteamEmulator
    {
        Test test = new Test();
        AddressHelper.GetAddress(test);
        //Instance
        public static SteamEmulator Instance;

        // Callbacks
        public static CallbackManager Client_Callback = new CallbackManager();
        public static CallbackManager Server_Callback = new CallbackManager();

        // Local Data
        public static HSteamUser HSteamUser;
        public static HSteamPipe HSteamPipe;

        public static HSteamUser HSteamUser_GS;
        public static HSteamPipe HSteamPipe_GS;

        public static ulong SteamId;
        public static ulong SteamId_GS;

        public static uint AppId;
        public static Dictionary<HSteamPipe, Steam_Pipe> steam_pipes;

        public static CSteamApiContext Context;

        //Client
        public static SteamClient SteamClient;
        public static SteamUser SteamUser;
        public static SteamFriends SteamFriends;
        public static SteamUtils SteamUtils;
        public static SteamMatchmaking SteamMatchmaking;
        public static SteamMatchmakingServers SteamMatchmakingServers;
        public static SteamUserStats SteamUserStats;
        public static SteamApps SteamApps;
        public static SteamNetworking SteamNetworking;
        public static SteamRemoteStorage SteamRemoteStorage;
        public static SteamScreenshots SteamScreenshots;
        public static SteamHTTP SteamHTTP;
        public static SteamController SteamController;
        public static SteamUGC SteamUGC;
        public static SteamAppList SteamAppList;
        public static SteamMusic SteamMusic;
        public static SteamMusicRemote SteamMusicRemote;
        public static SteamHTMLSurface SteamHTMLSurface;
        public static SteamInventory SteamInventory;
        public static SteamVideo SteamVideo;
        public static SteamParentalSettings SteamParentalSettings;
        public static SteamNetworkingSockets SteamNetworkingSockets;
        public static SteamNetworkingSocketsSerialized SteamNetworkingSocketsSerialized;
        public static SteamNetworkingMessages SteamNetworkingMessages;
        public static SteamGameCoordinator SteamGameCoordinator;
        public static SteamNetworkingUtils SteamNetworkingUtils;
        public static SteamGameSearch SteamGameSearch;
        public static SteamInput SteamInput;
        public static SteamParties SteamParties;
        public static SteamRemotePlay SteamRemotePlay;
        public static SteamTV SteamTV;

        //GameServer
        public static SteamGameServer SteamGameServer;
        public static SteamUtils SteamGameServerUtils;
        public static SteamGameServerStats SteamGameServerStats;
        public static SteamNetworking SteamGameServerNetworking;
        public static SteamHTTP SteamGameServerHttp;
        public static SteamInventory SteamGameServerInventory;
        public static SteamUGC SteamGameServerUgc;
        public static SteamApps SteamGameServerApps;
        public static SteamNetworkingSockets SteamGameServerNetworkingSockets;
        public static SteamNetworkingSocketsSerialized SteamGameServerNetworkingSocketsSerialized;
        public static SteamNetworkingMessages SteamGameServerNetworkingMessages;
        public static SteamGameCoordinator SteamGameServerGamecoordinator;
        public static SteamMasterServerUpdater SteamMasterServerUpdater;

        public static string Language { get; set; }
        public static string PersonaName { get; set; }
        public static bool Initialized { get; set; }

        public SteamEmulator()
        {
            steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
            Instance = this;
        }

        internal void Initialize()
        {
            string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

            if (!File.Exists(_file))
            {
                new frmLogin().ShowDialog();
            }

            modCommon.LoadSettings();

            InterfaceManager.Initialize();

            if (Client_Callback == null) Client_Callback = new CallbackManager();
            if (Server_Callback == null) Server_Callback = new CallbackManager();

            steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

            // CLIENT

            SteamClient = new SteamClient();
            AddInterface(SteamClient);

            SteamUser = new SteamUser();
            AddInterface(SteamUser);

            SteamFriends = new SteamFriends();
            AddInterface(SteamFriends);

            SteamUtils = new SteamUtils();
            AddInterface(SteamUtils);

            SteamMatchmaking = new SteamMatchmaking();
            AddInterface(SteamMatchmaking);

            SteamMatchmakingServers = new SteamMatchmakingServers();
            AddInterface(SteamMatchmakingServers);

            SteamUserStats = new SteamUserStats();
            AddInterface(SteamUserStats);

            SteamApps = new SteamApps();
            AddInterface(SteamApps);

            SteamNetworking = new SteamNetworking();
            AddInterface(SteamNetworking);

            SteamRemoteStorage = new SteamRemoteStorage();
            AddInterface(SteamRemoteStorage);

            SteamScreenshots = new SteamScreenshots();
            AddInterface(SteamScreenshots);

            SteamHTTP = new SteamHTTP();
            AddInterface(SteamHTTP);

            SteamController = new SteamController();
            AddInterface(SteamController);

            SteamUGC = new SteamUGC();
            AddInterface(SteamUGC);

            SteamAppList = new SteamAppList();
            AddInterface(SteamAppList);

            SteamMusic = new SteamMusic();
            AddInterface(SteamMusic);

            SteamMusicRemote = new SteamMusicRemote();
            AddInterface(SteamMusicRemote);

            SteamHTMLSurface = new SteamHTMLSurface();
            AddInterface(SteamHTMLSurface);

            SteamInventory = new SteamInventory();
            AddInterface(SteamInventory);

            SteamVideo = new SteamVideo();
            AddInterface(SteamVideo);

            SteamParentalSettings = new SteamParentalSettings();
            AddInterface(SteamParentalSettings);

            SteamNetworkingSockets = new SteamNetworkingSockets();
            AddInterface(SteamNetworkingSockets);

            SteamNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();
            AddInterface(SteamNetworkingSocketsSerialized);

            SteamNetworkingMessages = new SteamNetworkingMessages();
            AddInterface(SteamNetworkingMessages);

            SteamGameCoordinator = new SteamGameCoordinator();
            AddInterface(SteamGameCoordinator);

            SteamNetworkingUtils = new SteamNetworkingUtils();
            AddInterface(SteamNetworkingUtils);

            SteamGameSearch = new SteamGameSearch();
            AddInterface(SteamGameSearch);

            SteamParties = new SteamParties();
            AddInterface(SteamParties);

            SteamRemotePlay = new SteamRemotePlay();
            AddInterface(SteamRemotePlay);

            SteamTV = new SteamTV();
            AddInterface(SteamTV);

            SteamInput = new SteamInput();
            AddInterface(SteamInput);

            // GAMESERVER

            SteamGameServer = new SteamGameServer();
            AddInterface(SteamGameServer);

            SteamGameServerUtils = new SteamUtils();
            AddInterface(SteamGameServerUtils);

            SteamGameServerStats = new SteamGameServerStats();
            AddInterface(SteamGameServerStats);

            SteamGameServerNetworking = new SteamNetworking();
            AddInterface(SteamGameServerNetworking);

            SteamGameServerHttp = new SteamHTTP();
            AddInterface(SteamGameServerHttp);

            SteamGameServerInventory = new SteamInventory();
            AddInterface(SteamGameServerInventory);

            SteamGameServerUgc = new SteamUGC();
            AddInterface(SteamGameServerUgc);

            SteamGameServerApps = new SteamApps();
            AddInterface(SteamGameServerApps);

            SteamGameServerNetworkingSockets = new SteamNetworkingSockets();
            AddInterface(SteamGameServerNetworkingSockets);

            SteamGameServerNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();
            AddInterface(SteamGameServerNetworkingSocketsSerialized);

            SteamGameServerNetworkingMessages = new SteamNetworkingMessages();
            AddInterface(SteamGameServerNetworkingMessages);

            SteamGameServerGamecoordinator = new SteamGameCoordinator();
            AddInterface(SteamGameServerGamecoordinator);

            SteamMasterServerUpdater = new SteamMasterServerUpdater();
            AddInterface(SteamMasterServerUpdater);

            HSteamUser = (HSteamUser)1;
            HSteamPipe = (HSteamPipe)1;

            HSteamUser_GS = (HSteamUser)1;
            HSteamPipe_GS = (HSteamPipe)1;

            SteamClient.ConnectToGlobalUser(HSteamPipe);

            Context = new CSteamApiContext();
            var success = Context.Init();

            if (success)
            {
                Write("SteamApi Context created successfully");
            }
            else
            {
                Write("Error creating SteamApi Context");
            }

            Initialized = true;

            InterfaceManager.Initialize();

        }

        private void AddInterface(IBaseInterface steamInterface)
        {
            InterfaceManager.Interfaces.Add(steamInterface);
        }

        public static HSteamUser CreateSteamUser()
        {
            if (HSteamUser == null)
            {
                HSteamUser = (HSteamUser)1;
                Write($"Creating user {HSteamUser}");
            }
            return HSteamUser;
        }

        public static HSteamPipe CreateSteamPipe()
        {
            if (HSteamPipe == null)
            {
                HSteamPipe = (HSteamPipe)1;
                Write($"Creating pipe {HSteamPipe}");
                steam_pipes[HSteamPipe] = Steam_Pipe.NO_USER;
            }
            return HSteamPipe;
        }

        public static void Write(object v)
        {
            Log.Write(v);
        }
    }
    public enum Steam_Pipe : int
    {
        NO_USER,
        CLIENT,
        SERVER
    };
}
