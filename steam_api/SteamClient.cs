using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;

public class SteamClient 
{
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
    public static Steam_User steam_User;
    public static Steam_Friends steam_Friends;
    public static Steam_Utils steam_Utils;
    public static Steam_Matchmaking steam_Matchmaking;
    public static Steam_Matchmaking_Servers steam_MatchmakingServers;
    public static Steam_User_Stats steam_UserStats;
    public static Steam_Apps steam_Apps;
    public static Steam_Networking steam_Networking;
    public static Steam_RemoteStorage steam_RemoteStorage;
    public static Steam_Screenshots steam_Screenshots;
    public static Steam_HTTP steam_Http;
    public static Steam_Controller steam_Controller;
    public static Steam_UGC steam_Ugc;
    public static Steam_AppList steam_AppList;
    public static Steam_Music steam_Music;
    public static Steam_MusicRemote steam_MusicRemote;
    public static Steam_HTMLSurface steam_HTMLsurface;
    public static Steam_Inventory steam_Inventory;
    public static Steam_Video steam_Video;
    public static Steam_ParentalSettings steam_Parental;
    public static Steam_NetworkingSockets steam_NetworkingSockets;
    public static Steam_Networking_Sockets_Serialized steam_NetworkingSocketsSerialized;
    public static Steam_NetworkingMessages steam_NetworkingMessages;
    public static Steam_GameCoordinator steam_Gamecoordinator;
    public static Steam_NetworkingUtils steam_NetworkingUtils;
    public static Steam_Unified_Messages steam_UnifiedMessages;
    public static Steam_GameSearch steam_GameSearch;
    public static Steam_Input steam_Input;


    public static Steam_Parties steam_Parties;
    public static Steam_RemotePlay steam_RemotePlay;
    public static Steam_TV steam_Tv;

    //GameServer
    public static Steam_GameServer steam_GameServer;

    public static Steam_Utils steam_GameServerUtils;
    public static Steam_GameServerStats steam_GameServerStats;
    public static Steam_Networking steam_GameServerNetworking;
    public static Steam_HTTP steam_GameServerHttp;
    public static Steam_Inventory steam_GameServerInventory;
    public static Steam_UGC steam_GameServerUgc;
    public static Steam_Apps steam_GameServerApps;
    public static Steam_NetworkingSockets steam_GameServerNetworkingSockets;
    public static Steam_Networking_Sockets_Serialized steam_GameServerNetworkingSocketsSerialized;
    public static Steam_NetworkingMessages steam_GameServerNetworkingMessages;
    public static Steam_GameCoordinator steam_GameServerGamecoordinator;
    public static Steam_Masterserver_Updater steam_MasterServerUpdater;
    public static Steam_Overlay steam_Overlay;

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
        PRINT_DEBUG($"{"Initializing SteamClient"}");

        modCommon.ActiveConsoleOutput();
        Console.WriteLine("Testing");

        if (Client_Callback == null) Client_Callback = new CallbackManager();
        if (Server_Callback == null) Server_Callback = new CallbackManager();
        
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();

        steam_User = new Steam_User();
        steam_Friends = new Steam_Friends();
        steam_Utils = new Steam_Utils();
        steam_Matchmaking = new Steam_Matchmaking();
        steam_MatchmakingServers = new Steam_Matchmaking_Servers();
        steam_UserStats = new Steam_User_Stats();
        steam_Apps = new Steam_Apps();
        steam_Networking = new Steam_Networking();
        steam_RemoteStorage = new Steam_RemoteStorage();
        steam_Screenshots = new Steam_Screenshots();
        steam_Http = new Steam_HTTP();
        steam_Controller = new Steam_Controller();
        steam_Ugc = new Steam_UGC();
        steam_AppList = new Steam_AppList();
        steam_Music = new Steam_Music();
        steam_MusicRemote = new Steam_MusicRemote();
        steam_HTMLsurface = new Steam_HTMLSurface();
        steam_Inventory = new Steam_Inventory();
        steam_Video = new Steam_Video();
        steam_Parental = new Steam_ParentalSettings();
        steam_NetworkingSockets = new Steam_NetworkingSockets();
        steam_NetworkingSocketsSerialized = new Steam_Networking_Sockets_Serialized();
        steam_NetworkingMessages = new Steam_NetworkingMessages();
        steam_Gamecoordinator = new Steam_GameCoordinator();
        steam_NetworkingUtils = new Steam_NetworkingUtils();
        steam_UnifiedMessages = new Steam_Unified_Messages();
        steam_GameSearch = new Steam_GameSearch();
        steam_Parties = new Steam_Parties();
        steam_RemotePlay = new Steam_RemotePlay();
        steam_Tv = new Steam_TV();
        steam_Input = new Steam_Input();

        steam_GameServer = new Steam_GameServer();
        steam_GameServerUtils = new Steam_Utils();
        steam_GameServerStats = new Steam_GameServerStats();
        steam_GameServerNetworking = new Steam_Networking();
        steam_GameServerHttp = new Steam_HTTP();
        steam_GameServerInventory = new Steam_Inventory();
        steam_GameServerUgc = new Steam_UGC();
        steam_GameServerApps = new Steam_Apps();
        steam_GameServerNetworkingSockets = new Steam_NetworkingSockets();
        steam_GameServerNetworkingSocketsSerialized = new Steam_Networking_Sockets_Serialized();
        steam_GameServerNetworkingMessages = new Steam_NetworkingMessages();
        steam_GameServerGamecoordinator = new Steam_GameCoordinator();
        steam_MasterServerUpdater = new Steam_Masterserver_Updater();
    }



    private static void PRINT_DEBUG(object v)
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
        return steam_User;
    }
    
    public static ISteamGameServer GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamGameServer");
        return steam_GameServer;
    }

    
    public static void SetLocalIPBinding(uint unIP, ushort usPort)
    {
        DEBUG("SetLocalIPBinding");
    }

    
    public static IntPtr GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamFriends");
        return IntPtr.Zero;
    }

    
    public static ISteamGameSearch GetISteamGameSearch(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamFriends");
        return steam_GameSearch;
    }

    
    public static ISteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUtils");
        return default;
    }

    
    public static ISteamMatchmaking GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMatchmaking");
        return steam_Matchmaking;
    }

    
    public static ISteamMatchmakingServers GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMatchmakingServers");
        return steam_MatchmakingServers;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static object GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamGenericInterface");
        return steam_Video;
    }

    
    public static ISteamUserStats GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUserStats");
        return steam_UserStats;
    }
    
    //public static ISteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    //{
    //    DEBUG("GetISteamGameServerStats");
    //    return steam_GameServerStats;
    //}

    //
    //public static ISteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    //{
    //    DEBUG("GetISteamApps");
    //    return steam_Apps;
    //}

    
    public static ISteamNetworking GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamNetworking");
        return steam_Networking;
    }

    
    public static ISteamRemoteStorage GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamRemoteStorage");
        return steam_RemoteStorage;
    }

    
    public static ISteamScreenshots GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamScreenshots");
        return steam_Screenshots;
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
        return steam_Http;
    }

    
    public static ISteamController GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamController");
        return steam_Controller;
    }

    
    public static ISteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamUGC");
        return steam_Ugc;
    }

    //
    //public static ISteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    //{
    //    DEBUG("GetISteamAppList");
    //    return steam_AppList;
    //}

    
    public static ISteamMusic GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMusic");
        return steam_Music;
    }

    
    public static ISteamMusicRemote GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamMusicRemote");
        return steam_MusicRemote;
    }

    
    public static ISteamInput GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamInput");
        return steam_Input;
    }

    
    public static ISteamHTMLSurface GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamHTMLSurface");
        return steam_HTMLsurface;
    }

    
    public static ISteamParties GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParties");
        return steam_Parties;
    }

    
    public static ISteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamInventory");
        return steam_Inventory;
    }

    
    public static ISteamRemotePlay GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamRemotePlay");
        return steam_RemotePlay;
    }

    
    public static ISteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamVideo");
        return steam_Video;
    }

    
    public static ISteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParentalSettings");
        return steam_Parental;
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
