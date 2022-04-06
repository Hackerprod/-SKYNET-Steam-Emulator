using SKYNET;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SteamEmulator
{
    public static SteamEmulator Instance;

    // Callbacks
    public static CallbackManager Client_Callback = new CallbackManager();
    public static CallbackManager Server_Callback = new CallbackManager();

    public event EventHandler<object> OnMessage;

    #region Client Info

    public static string Language { get; set; }
    public static string PersonaName { get; set; }
    public static ulong SteamId { get; set; }
    public static ulong SteamId_GS { get; set; }
    public static bool AsClient { get; set; }
    public static uint AppId { get; set; }

    public static HSteamUser HSteamUser;
    public static HSteamPipe HSteamPipe;

    public static HSteamUser HSteamUser_GS{ get; set; }
    public static HSteamPipe HSteamPipe_GS;

    #endregion

    public static bool Initialized { get; set; }
    public static string SteamApiPath { get; set; }

    public static Dictionary<HSteamPipe, Steam_Pipe> steam_pipes;

    #region Interfaces 

    //Client
    public static SteamClient SteamClient;
    public static SteamUser SteamUser;
    public static SteamFriends SteamFriends;
    public static SteamUtils SteamUtils;
    public static SteamMatchmaking SteamMatchmaking;
    public static SteamMatchMakingServers SteamMatchMakingServers;
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

    #endregion

    public SteamEmulator(bool asClient)
    {
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
        Instance = this;
        AsClient = asClient;
    }

    public void Initialize()
    {
        string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

        modCommon.LoadSettings();

        InterfaceManager.Initialize();

        if (Client_Callback == null) Client_Callback = new CallbackManager();
        if (Server_Callback == null) Server_Callback = new CallbackManager();

        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

        #region Interface Initialization

        // Client Interfaces

        SteamClient = CreateInterface<SteamClient>();

        SteamUser = CreateInterface<SteamUser>();

        SteamFriends = CreateInterface<SteamFriends>();

        SteamUtils = CreateInterface<SteamUtils>();

        SteamMatchmaking = CreateInterface<SteamMatchmaking>();   

        SteamMatchMakingServers = CreateInterface<SteamMatchMakingServers>();    

        SteamUserStats = CreateInterface<SteamUserStats>();  

        SteamApps = CreateInterface<SteamApps>();   

        SteamNetworking = CreateInterface<SteamNetworking>();   

        SteamRemoteStorage = CreateInterface<SteamRemoteStorage>();  

        SteamScreenshots = CreateInterface<SteamScreenshots>();   

        SteamHTTP = CreateInterface<SteamHTTP>();

        SteamController = CreateInterface<SteamController>();

        SteamUGC = CreateInterface<SteamUGC>();

        SteamAppList = CreateInterface<SteamAppList>();

        SteamMusic = CreateInterface<SteamMusic>();

        SteamMusicRemote = CreateInterface<SteamMusicRemote>();

        SteamHTMLSurface = CreateInterface<SteamHTMLSurface>();

        SteamInventory = CreateInterface<SteamInventory>();

        SteamVideo = CreateInterface<SteamVideo>();

        SteamParentalSettings = CreateInterface<SteamParentalSettings>();

        SteamNetworkingSockets = CreateInterface<SteamNetworkingSockets>();

        SteamNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>();

        SteamNetworkingMessages = CreateInterface<SteamNetworkingMessages>();

        SteamGameCoordinator = CreateInterface<SteamGameCoordinator>();

        SteamNetworkingUtils = CreateInterface<SteamNetworkingUtils>();

        SteamGameSearch = CreateInterface<SteamGameSearch>();

        SteamParties = CreateInterface<SteamParties>();

        SteamRemotePlay = CreateInterface<SteamRemotePlay>();

        SteamTV = CreateInterface<SteamTV>();

        SteamInput = CreateInterface<SteamInput>();


        // Server Interfaces

        SteamGameServer = CreateInterface<SteamGameServer>();

        SteamGameServerUtils = CreateInterface<SteamUtils>();

        SteamGameServerStats = CreateInterface<SteamGameServerStats>();

        SteamGameServerNetworking = CreateInterface<SteamNetworking>();

        SteamHTTP = CreateInterface<SteamHTTP>();

        SteamGameServerInventory = CreateInterface<SteamInventory>();

        SteamGameServerUgc = CreateInterface<SteamUGC>();

        SteamGameServerApps = CreateInterface<SteamApps>();

        SteamGameServerNetworkingSockets = CreateInterface<SteamNetworkingSockets>();

        SteamGameServerNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>();

        SteamGameServerNetworkingMessages = CreateInterface<SteamNetworkingMessages>();

        SteamGameServerGamecoordinator = CreateInterface<SteamGameCoordinator>();

        SteamMasterServerUpdater = CreateInterface<SteamMasterServerUpdater>();

        #endregion

        HSteamUser = (HSteamUser)1;
        HSteamPipe = (HSteamPipe)1;

        HSteamUser_GS = (HSteamUser)1;
        HSteamPipe_GS = (HSteamPipe)1;

        SteamClient.ConnectToGlobalUser((int)HSteamPipe);

        //if (success)
        //{
        //    Write("SteamApi Context created successfully");
        //}
        //else
        //{
        //    Write("Error creating SteamApi Context");
        //}

        Initialized = true;

        InterfaceManager.Initialize();
    }

    private T CreateInterface<T>()  where T : ISteamInterface
    {
        T baseClass = InterfaceManager.CreateInterface<T>(out IntPtr BaseAddress);
        baseClass.MemoryAddress = BaseAddress;
        return (T)baseClass;
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

    public static void Write(object sender, object msg)
    {
        Write(sender + ": " + msg);
    }

//#if Debug

    public static void Write(object msg)
    {
        if (AsClient)
        {
            Instance.OnMessage?.Invoke(Instance, msg);
        }
        Log.Write(msg);
    }
//#else

//    public static void Write(object msg)
//    {
//        // TODO
//    }

//#endif

}

public enum Steam_Pipe : int
{
    NO_USER,
    CLIENT,
    SERVER
};
