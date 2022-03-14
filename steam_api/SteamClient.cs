using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Callback;
using SKYNET.GUI;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;

public class SteamClient
{
    //Instance
    public static SteamClient Instance;

    // Callbacks
    public static CallbackManager Client_Callback;
    public static CallbackManager Server_Callback;

    // Local Data
    public static HSteamUser LocalUser;
    public static HSteamPipe LocalPipe;
    public static ulong SteamId;
    public static ulong SteamId_GameServer;
    public static uint AppId;

    private static Dictionary<HSteamPipe, Steam_Pipe> steam_pipes;

    //Client
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

    static SteamClient()
    {
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
        Client_Callback = new CallbackManager();
        Server_Callback = new CallbackManager();
    }
    public static void Initialize()
    {
        string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

        if (!File.Exists(_file))
        {
            new frmLogin().ShowDialog();
        }

        modCommon.LoadSettings();

        Write($"{"Initializing SteamClient"}");

        if (Client_Callback == null) Client_Callback = new CallbackManager();
        if (Server_Callback == null) Server_Callback = new CallbackManager();
        
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

        SteamUser = new SteamUser();
        SteamFriends = new SteamFriends();
        SteamUtils = new SteamUtils();
        SteamMatchmaking = new SteamMatchmaking();
        SteamMatchmakingServers = new SteamMatchmakingServers();
        SteamUserStats = new SteamUserStats();
        SteamApps = new SteamApps();
        SteamNetworking = new SteamNetworking();
        SteamRemoteStorage = new SteamRemoteStorage();
        SteamScreenshots = new SteamScreenshots();
        SteamHTTP = new SteamHTTP();
        SteamController = new SteamController();
        SteamUGC = new SteamUGC();
        SteamAppList = new SteamAppList();
        SteamMusic = new SteamMusic();
        SteamMusicRemote = new SteamMusicRemote();
        SteamHTMLSurface = new SteamHTMLSurface();
        SteamInventory = new SteamInventory();
        SteamVideo = new SteamVideo();
        SteamParentalSettings = new SteamParentalSettings();
        SteamNetworkingSockets = new SteamNetworkingSockets();
        SteamNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();
        SteamNetworkingMessages = new SteamNetworkingMessages();
        SteamGameCoordinator = new SteamGameCoordinator();
        SteamNetworkingUtils = new SteamNetworkingUtils();
        SteamGameSearch = new SteamGameSearch();
        SteamParties = new SteamParties();
        SteamRemotePlay = new SteamRemotePlay();
        SteamTV = new SteamTV();
        SteamInput = new SteamInput();

        SteamGameServer = new SteamGameServer();
        SteamGameServerUtils = new SteamUtils();
        SteamGameServerStats = new SteamGameServerStats();
        SteamGameServerNetworking = new SteamNetworking();
        SteamGameServerHttp = new SteamHTTP();
        SteamGameServerInventory = new SteamInventory();
        SteamGameServerUgc = new SteamUGC();
        SteamGameServerApps = new SteamApps();
        SteamGameServerNetworkingSockets = new SteamNetworkingSockets();
        SteamGameServerNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();
        SteamGameServerNetworkingMessages = new SteamNetworkingMessages();
        SteamGameServerGamecoordinator = new SteamGameCoordinator();
        SteamMasterServerUpdater = new SteamMasterServerUpdater();

        Initialized = true;
    }

    public static void Write(object v)
    {
        Log.Write(v);
    }

    public static T InstallInterface<T>(SteamInterface Interface) where T : SteamInterface, new()
    {
        Interface = new T();
        return (T)Interface;
    }

    public static void SetAppId(uint appId)
    {
        AppId = appId;
    }

    public static HSteamPipe CreateSteamPipe()
    {
        DEBUG("CreateSteamPipe");
        if (LocalPipe == null)
        {
            LocalPipe = (HSteamPipe)1;
            DEBUG($"Creating pipe {LocalPipe}");
            steam_pipes[LocalPipe] = Steam_Pipe.NO_USER;
        }
        return LocalPipe;
    }

    public static bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
    {
        DEBUG("BReleaseSteamPipe");
        return false;
    }

    public static HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
    {
        DEBUG("ConnectToGlobalUser");
        if (steam_pipes.ContainsKey(hSteamPipe))
        {
            return (HSteamUser)0;
        }

        steam_pipes[hSteamPipe] = Steam_Pipe.CLIENT;

        return (HSteamUser)1;
    }

    enum Steam_Pipe
    {
        NO_USER,
        CLIENT,
        SERVER
    };

    public static HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
    {
        DEBUG("CreateLocalUser");
        LocalUser = (HSteamUser)1;
        phSteamPipe = (HSteamPipe)1;
        return LocalUser;
    }

    public static void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
    {
        DEBUG("ReleaseUser");
    }
    
    public static ISteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUser");
        return SteamUser;
    }
    
    public static ISteamGameServer GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamGameServer");
        return SteamGameServer;
    }
    
    public static void SetLocalIPBinding(uint unIP, ushort usPort)
    {
        DEBUG("SetLocalIPBinding");
    }
    
    public static ISteamFriends GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamFriends");
        return SteamFriends;
    }
    
    public static ISteamGameSearch GetISteamGameSearch(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamFriends");
        return SteamGameSearch;
    }
    
    public static ISteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUtils");
        return default;
    }
    
    public static ISteamMatchmaking GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMatchmaking");
        return SteamMatchmaking;
    }
    
    public static ISteamMatchmakingServers GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMatchmakingServers");
        return SteamMatchmakingServers;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static object GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamGenericInterface");
        return SteamVideo;
    }
    
    public static ISteamUserStats GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUserStats");
        return SteamUserStats;
    }

    //public static ISteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    //{
    //    DEBUG("GetISteamGameServerStats");
    //    return steam_GameServerStats;
    //}

    public static ISteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamApps");
        return SteamApps;
    }

    public static ISteamNetworking GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamNetworking");
        return SteamNetworking;
    }

    public static ISteamRemoteStorage GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamRemoteStorage");
        return SteamRemoteStorage;
    }
    
    public static ISteamScreenshots GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamScreenshots");
        return SteamScreenshots;
    }
    
    public static uint GetIPCCallCount()
    {
        DEBUG("GetIPCCallCount");
        return 0;
    }

    //
    //public static void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
    //{

    //}

    public static bool BShutdownIfAllPipesClosed()
    {
        DEBUG("BShutdownIfAllPipesClosed");
        return false;
    }
    
    public static ISteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamHTTP");
        return SteamHTTP;
    }
    
    public static ISteamController GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamController");
        return SteamController;
    }
    
    public static ISteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUGC");
        return SteamUGC;
    }

    public static ISteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamAppList");
        return SteamAppList;
    }

    public static ISteamMusic GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMusic");
        return SteamMusic;
    }
    
    public static ISteamMusicRemote GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMusicRemote");
        return SteamMusicRemote;
    }

    public static ISteamInput GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamInput");
        return SteamInput;
    }
    
    public static ISteamHTMLSurface GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamHTMLSurface");
        return SteamHTMLSurface;
    }

    public static ISteamParties GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParties");
        return SteamParties;
    }
    
    public static ISteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamInventory");
        return SteamInventory;
    }
    
    public static ISteamRemotePlay GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamRemotePlay");
        return SteamRemotePlay;
    }
    
    public static ISteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamVideo");
        return SteamVideo;
    }
    
    public static ISteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParentalSettings");
        return SteamParentalSettings;
    }
    
    public static void SetPersonaName(string pchPersonaName)
    {
        // Save to registry
    }

    private static void DEBUG(object v)
    {
        Log.Write(v);
    }
}
