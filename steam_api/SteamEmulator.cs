/*

                                ░██████╗██╗░░██╗██╗░░░██╗███╗░░██╗███████╗████████╗
                                ██╔════╝██║░██╔╝╚██╗░██╔╝████╗░██║██╔════╝╚══██╔══╝
                                ╚█████╗░█████═╝░░╚████╔╝░██╔██╗██║█████╗░░░░░██║░░░
                                ░╚═══██╗██╔═██╗░░░╚██╔╝░░██║╚████║██╔══╝░░░░░██║░░░
                                ██████╔╝██║░╚██╗░░░██║░░░██║░╚███║███████╗░░░██║░░░
                                ╚═════╝░╚═╝░░╚═╝░░░╚═╝░░░╚═╝░░╚══╝╚══════╝░░░╚═╝░░░   
*/

#define FORCELOG 
using SKYNET;
using SKYNET.Helper;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AppID = System.UInt32;
using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using System.Windows.Forms;

public class SteamEmulator
{
    public static SteamEmulator Instance;

    #region Client Info

    public static string Language;
    public static string PersonaName;
    public static string EmulatorPath;

    public static CSteamID SteamID;
    public static CSteamID SteamID_GS;
    public static uint AppID;
    public static List<DLC> DLCs;

    public static HSteamUser HSteamUser;
    public static HSteamPipe HSteamPipe;

    public static HSteamUser HSteamUser_GS;
    public static HSteamPipe HSteamPipe_GS;

    public static bool GameOverlay;
    public static bool SendLog;
    public static bool ConsoleLog;
    public static bool LogToFile;
    public static bool LogToConsole;

    public static bool Initialized;
    public static bool Initializing;

    // Debug options
    public static bool RunCallbacks;
    public static bool ISteamHTTP;

    public static int BroadCastPort = 28032;

    #endregion

    #region Interfaces 

    //Client
    public static SteamClient SteamClient;
    public static SKYNET.Steamworks.Implementation.SteamUser SteamUser;
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
    public static SteamGameServerStats SteamGameServerStats;
    public static SteamMasterServerUpdater SteamMasterServerUpdater;

    #endregion

    public SteamEmulator()
    {
        Instance = this;
    }

    static SteamEmulator()
    {
        // Load Default data
        Language = "english";
        PersonaName = "";
        EmulatorPath = "";
        SteamID = CSteamID.Invalid;
        SteamID_GS = CSteamID.CreateOne(); 
        AppID = 0;
        DLCs = new List<DLC>();
        LogToFile = true;
        LogToConsole = true;
    }

    public static void Initialize()
    {
        try
        {
            if (Initialized) return;
            Initializing = true;

            SteamID = CSteamID.Invalid;
            LoadAppID();

            Settings.Load();

            Log.Initialize();

            Write("Initializing Steam emulator");

            UserManager.Initialize();
            NetworkManager.Initialize();
            InterfaceManager.Initialize();

            #region Interface Initialization

            // Client Interfaces

            SteamClient = new SteamClient(); 

            SteamUser = new SKYNET.Steamworks.Implementation.SteamUser(); 

            SteamFriends = new SteamFriends(); 

            SteamUtils = new SteamUtils(); 

            SteamMatchmaking = new SteamMatchmaking(); 

            SteamMatchMakingServers = new SteamMatchMakingServers();

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

            // Server Interfaces

            SteamGameServer = new SteamGameServer();

            SteamGameServerStats = new SteamGameServerStats();

            SteamMasterServerUpdater = new SteamMasterServerUpdater();

            #endregion
            
            HSteamUser = 1;
            HSteamPipe = 1;

            HSteamUser_GS = 2;
            HSteamPipe_GS = 2;

            Initialized = true;
            Initializing = false;
        }
        catch (Exception ex)
        {
            Write(ex);
        }
    }

    private static void LoadAppID()
    {
        try
        {
            string appid_Path = Path.Combine(Common.GetPath(), "steam_appid.txt");
            if (File.Exists(appid_Path))
            {
                string content = File.ReadAllText(appid_Path);
                uint.TryParse(content, out AppID);
            }
        }
        catch 
        {
        }
    }

    private static void IsGCMessageAvailable(object sender, Dictionary<uint, byte[]> gcMessages)
    {
        foreach (var msg in gcMessages)
        {
            SteamGameCoordinator.PushMessage(msg.Key, msg.Value);
        }
    }

    public static HSteamUser CreateSteamUser()
    {
        if (HSteamUser == 0)
        {
            HSteamUser = 1;
            Write($"Creating user {HSteamUser}");
        }
        return HSteamUser;
    }

    public static uint CreateSteamPipe()
    {
        if (HSteamPipe == 0)
        {
            HSteamPipe = 1;
            Write($"Creating pipe {HSteamPipe}");
        }
        return HSteamPipe;
    }

    private static void Write(object msg)
    {
        Write("Steam Emulator", msg);
    }

#if FORCELOG

    public static void Write(string sender, object msg)
    {
        string message = " ";
        message += string.IsNullOrEmpty(sender) ? "" : $"{sender}: ";
        message += msg == null ? "NULL" : msg;

            if (true)
            {
                Log.AppEnd(message);
                //lastMsg = msg.ToString();
            }

            if (true)
            {
                if (sender.ToUpper() == "DEBUG")
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ResetColor();
                Console.WriteLine(message);
            }

    }

#else

    public static void Write(string sender, object msg)
    {
        MutexHelper.Wait("LOG", delegate
        {
            string message = " ";
            message += string.IsNullOrEmpty(sender) ? "" : $"{sender}: ";
            message += msg == null ? "NULL" : msg;

            if (LogToFile)
            {
                MessageBox.Show("Aqui");
                Log.AppEnd(message);
            }

            if (LogToConsole)
            {
                ConsoleHelper.WriteLine(message);
            }
        });
    }

#endif

    public static void Debug(object msg, ConsoleColor color = ConsoleColor.Red)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($" DEBUG: {msg}");

        MessageBox.Show("DEBUG");
    }
}



