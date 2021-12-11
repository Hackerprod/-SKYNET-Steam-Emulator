using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;

public class SteamClient 
{
    public static SteamClient Instance;
    public static uint AppId;
    public static ulong SteamId;
    public static ulong SteamId_GameServer;


    private static Dictionary<HSteamPipe, Steam_Pipe> steam_pipes;

    public static bool user_LoggedIn = false;
    private static bool server_init = false;
    private static bool steamclient_server_inited = false;
    private static int steam_pipe_counter = 1;
        private static List<CCallbackBase> callbacks;

    //Client
    static Steam_User steam_User;
    static Steam_Friends steam_Friends;
    static Steam_Utils steam_Utils;
    static Steam_Matchmaking steam_Matchmaking;
    static Steam_Matchmaking_Servers steam_MatchmakingServers;
    static Steam_User_Stats steam_UserStats;
    static Steam_Apps steam_Apps;
    static Steam_Networking steam_Networking;
    static Steam_RemoteStorage steam_RemoteStorage;
    static Steam_Screenshots steam_Screenshots;
    static Steam_HTTP steam_Http;
    static Steam_Controller steam_Controller;
    static Steam_UGC steam_Ugc;
    static Steam_AppList steam_AppList;
    static Steam_Music steam_Music;
    static Steam_MusicRemote steam_MusicRemote;
    static Steam_HTMLSurface steam_HTMLsurface;
    static Steam_Inventory steam_Inventory;
    static Steam_Video steam_Video;
    static Steam_ParentalSettings steam_Parental;
    static Steam_NetworkingSockets steam_NetworkingSockets;
    static Steam_Networking_Sockets_Serialized steam_NetworkingSocketsSerialized;
    static Steam_NetworkingMessages steam_NetworkingMessages;
    static Steam_GameCoordinator steam_Gamecoordinator;
    static Steam_NetworkingUtils steam_NetworkingUtils;
    static Steam_Unified_Messages steam_UnifiedMessages;
    static Steam_GameSearch steam_GameSearch;
    static Steam_Input steam_Input;


    static Steam_Parties steam_Parties;
    static Steam_RemotePlay steam_RemotePlay;
    static Steam_TV steam_Tv;

    //GameServer
    static Steam_GameServer steam_GameServer;

    static Steam_Utils steam_GameServerUtils;
    static Steam_GameServerStats steam_GameServerStats;
    static Steam_Networking steam_GameServerNetworking;
    static Steam_HTTP steam_GameServerHttp;
    static Steam_Inventory steam_GameServerInventory;
    static Steam_UGC steam_GameServerUgc;
    static Steam_Apps steam_GameServerApps;
    static Steam_NetworkingSockets steam_GameServerNetworkingSockets;
    static Steam_Networking_Sockets_Serialized steam_GameServerNetworkingSocketsSerialized;
    static Steam_NetworkingMessages steam_GameServerNetworkingMessages;
    static Steam_GameCoordinator steam_GameServerGamecoordinator;
    static Steam_Masterserver_Updater steam_MasterServerUpdater;
    static Steam_Overlay steam_Overlay;

    public static string Language { get; internal set; }
    public static string PersonaName { get; internal set; }

    public SteamClient()
    {
        Instance = this;
    }
    public static void Initialize()
    {
        try
        {
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
        catch (Exception ex)
        {
            
        }

    }

    public static void SetAppId(uint appId)
    {
        AppId = appId;
    }

    public static HSteamPipe CreateSteamPipe()
    {
        DEBUG("CreateSteamPipe");
        HSteamPipe pipe = (HSteamPipe)1;
        DEBUG($"Creating pipe {pipe}");
        steam_pipes[pipe] = Steam_Pipe.NO_USER;
        return pipe;
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
        user_LoggedIn = true;

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
        phSteamPipe = (HSteamPipe)1;
        return (HSteamUser)1;
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

    internal static ISteamGameSearch GetISteamGameSearch(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
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

    public static ISteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamGameServerStats");
        return steam_GameServerStats;
    }

    public static ISteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamApps");
        return steam_Apps;
    }

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

    public static ISteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamAppList");
        return steam_AppList;
    }

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

    internal static ISteamParties GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParties");
        return steam_Parties;
    }

    public static ISteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamInventory");
        return steam_Inventory;
    }

    internal static ISteamRemotePlay GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamRemotePlay");
        return steam_RemotePlay;
    }

    public static ISteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamVideo");
        return steam_Video;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
    {
        DEBUG("GetISteamParentalSettings");
        return steam_Parental;
    }





    static SteamClient()
    {
        steam_pipes = new Dictionary<HSteamPipe, Steam_Pipe>();
        callbacks = new List<CCallbackBase>();
    }

    internal static void AddCallback(int iCallback, CCallbackBase cb)
    {
        DEBUG("AddCallback " + iCallback);
        if (iCallback == SteamAPICallCompleted_t.k_iCallback)
        {
            return;
        }

        if (!callbacks.Contains(cb))
        {
            callbacks.Add(cb);
        }
    }

    internal static void AddCallback(int iCallback, IntPtr cb)
    {
        DEBUG("addCallBack " + iCallback);
        if (iCallback == SteamAPICallCompleted_t.k_iCallback)
        {
            return;
        }
    }

    internal static void SetPersonaName(string pchPersonaName)
    {
        // Save to registry
    }

    private static void DEBUG(object v)
    {
        Log.Write(v);
    }
}
