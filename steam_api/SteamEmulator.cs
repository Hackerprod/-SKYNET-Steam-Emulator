using Core.Interface;
using SKYNET.Callback;
using SKYNET.GUI;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;
using Steamworks.Core;
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

    public static string Language { get; set; }
    public static string PersonaName { get; set; }
    public static bool Initialized { get; set; }

    public SteamEmulator()
    {
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
        Instance = this;
    }

    public void Initialize()
    {
        string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

        if (!File.Exists(_file))
        {
            new frmLogin().ShowDialog();
        }

        Loader.Load();

        modCommon.LoadSettings();

        InterfaceManager.Initialize();

        if (Client_Callback == null) Client_Callback = new CallbackManager();
        if (Server_Callback == null) Server_Callback = new CallbackManager();

        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

        // CLIENT

        SteamClient = CreateInterface<SteamClient>();

        SteamUser = CreateInterface<SteamUser>();

        SteamFriends = CreateInterface<SteamFriends>();

        //SteamUtils = CreateInterface<SteamUtils>();

        //SteamMatchmaking = CreateInterface<SteamMatchmaking>();   

        //SteamMatchMakingServers = CreateInterface<SteamMatchMakingServers>();    

        //SteamUserStats = CreateInterface<SteamUserStats>();  

        //SteamApps = CreateInterface<SteamApps>();   

        //SteamNetworking = CreateInterface<SteamNetworking>();   

        //SteamRemoteStorage = CreateInterface<SteamRemoteStorage>();  

        //SteamScreenshots = CreateInterface<SteamScreenshots>();   

        //SteamHTTP = CreateInterface<SteamHTTP>();   

        //SteamController = CreateInterface<SteamController>();   

        //SteamUGC = CreateInterface<SteamUGC>();  

        //SteamAppList = CreateInterface<SteamAppList>();  

        //SteamMusic = CreateInterface<SteamMusic>();  

        //SteamMusicRemote = CreateInterface<SteamMusicRemote>();    

        //SteamHTMLSurface = CreateInterface<SteamHTMLSurface>();   

        //SteamInventory = CreateInterface<SteamInventory>();   

        //SteamVideo = CreateInterface<SteamVideo>();    

        //SteamParentalSettings = CreateInterface<SteamParentalSettings>();  

        //SteamNetworkingSockets = CreateInterface<SteamNetworkingSockets>();   

        //SteamNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>();    

        //SteamNetworkingMessages = CreateInterface<SteamNetworkingMessages>();   

        //SteamGameCoordinator = CreateInterface<SteamGameCoordinator>();   

        //SteamNetworkingUtils = CreateInterface<SteamNetworkingUtils>();    

        //SteamGameSearch = CreateInterface<SteamGameSearch>();   

        //SteamParties = CreateInterface<SteamParties>();    

        //SteamRemotePlay = CreateInterface<SteamRemotePlay>();   

        //SteamTV = CreateInterface<SteamTV>();   

        //SteamInput = CreateInterface<SteamInput>();   

        

        //SteamGameServer = CreateInterface<SteamGameServer>();   

        //SteamGameServerUtils = CreateInterface<SteamUtils>();   

        //SteamGameServerStats = CreateInterface<SteamGameServerStats>();   

        //SteamGameServerNetworking = CreateInterface<SteamNetworking>(); 

        //SteamHTTP = CreateInterface<SteamHTTP>(); 

        //SteamGameServerInventory = CreateInterface<SteamInventory>(); 

        //SteamGameServerUgc = CreateInterface<SteamUGC>();  

        //SteamGameServerApps = CreateInterface<SteamApps>(); 

        //SteamGameServerNetworkingSockets = CreateInterface<SteamNetworkingSockets>();  

        //SteamGameServerNetworkingSocketsSerialized = CreateInterface<SteamNetworkingSocketsSerialized>(); 

        //SteamGameServerNetworkingMessages = CreateInterface<SteamNetworkingMessages>();  

        //SteamGameServerGamecoordinator = CreateInterface<SteamGameCoordinator>();   

        //SteamMasterServerUpdater = CreateInterface<SteamMasterServerUpdater>(); 

        HSteamUser = (HSteamUser)1;
        HSteamPipe = (HSteamPipe)1;

        HSteamUser_GS = (HSteamUser)1;
        HSteamPipe_GS = (HSteamPipe)1;

        SteamClient.ConnectToGlobalUser((int)HSteamPipe);

        Context = new CSteamApiContext();
        //var success = Context.Init();

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

    private T CreateInterface<T>()  where T : IBaseInterface
    {
        var (context, iface) = Core.Interface.Context.CreateInterface(typeof(T));
        T baseClass = (T)iface;
        baseClass.BaseAddress = context;
        return (T)baseClass;
    }

    private void AddInterface(object steamInterface)
    {
        //
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
