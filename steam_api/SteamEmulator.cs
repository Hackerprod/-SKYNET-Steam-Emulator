#define LOG
using SKYNET;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Plugin;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AppID = System.UInt32;
using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

public class SteamEmulator
{
    public static SteamEmulator Instance;

    public static event EventHandler<GameMessage> OnMessage;

    public static bool Hooked;

    public static IGameCoordinatorPlugin GameCoordinatorPlugin;

    #region Client Info

    public static string Language;
    public static string PersonaName;
    public static string SteamApiPath;
    public static string EmulatorPath;

    public static CSteamID SteamId;
    public static CSteamID SteamId_GS;
    public static ulong GameID;
    public static uint AppId;
    public static bool Initialized;
    public static bool Initializing;
    public static bool SendLog;

    public static HSteamUser HSteamUser;
    public static HSteamPipe HSteamPipe;

    public static HSteamUser HSteamUser_GS;
    public static HSteamPipe HSteamPipe_GS;

    #endregion

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
    public static SteamGameServerStats SteamGameServerStats;
    public static SteamMasterServerUpdater SteamMasterServerUpdater;

    #endregion

    public SteamEmulator()
    {
        Instance = this;
    }

    public static void Initialize(bool hooked = false)
    {
        if (Initialized) return;
        Initializing = true;
        Hooked = hooked;
        try
        {
            if (!Hooked)
            {
                Settings.Load();

                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.log");
                if (File.Exists(fileName))
                {
                    File.WriteAllLines(fileName, new List<string>());
                }
            }
        }
        catch
        {
            Write("Settings", "Error loading settings");
        }

        SteamId_GS = new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);

        InterfaceManager.Initialize();
        NetworkManager.Initialize();

        #region Interface Initialization

        // Client Interfaces

        SteamClient = new SteamClient();

        SteamUser = new SteamUser();

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

        InitializePlugins();

        Initialized = true;
        Initializing = false;

    }

    private static void InitializePlugins()
    {
        string PluginsDirectory = modCommon.GetPath();
        if (Directory.Exists(PluginsDirectory))
        {
            foreach (var file in Directory.GetFiles(PluginsDirectory, "*.dll"))
            {
                if (Path.GetFileNameWithoutExtension(file).StartsWith("SKYNET."))
                {
                    try
                    {
                        var plugin = Assembly.LoadFile(file);
                        Type type = plugin.GetType("SKYNET.GameCoordinator");
                        if (type != null)
                        {
                            IGameCoordinatorPlugin iPlugin = (IGameCoordinatorPlugin)Activator.CreateInstance(type);
                            if (iPlugin == null)
                            {
                                Write("PLUGINS", $"Failed to load plugin {Path.GetFileNameWithoutExtension(file)}");
                            }
                            else
                            {
                                AppID appID = iPlugin.Initialize();
                                if (appID == AppId)
                                {
                                    GameCoordinatorPlugin = iPlugin;
                                    GameCoordinatorPlugin.IsMessageAvailable = IsMessageAvailable;
                                    Write("PLUGINS", $"Loaded GameCoordinator plugin {Path.GetFileNameWithoutExtension(file)} for AppID {appID}");
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Write("PLUGINS", $"Failed to load plugin {Path.GetFileNameWithoutExtension(file)} {"\n"}");
                    }
                }
            }
        }
    }

    private static void IsMessageAvailable(object sender, Dictionary<uint, byte[]> gcMessages)
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

    private static void Write(string msg)
    {
        Write("Steam Emulator", msg);
    }

    private static string lastMsg = "";

#if LOG

    public static void Write(string sender, object msg)
    {
        if (string.IsNullOrEmpty(sender)) { sender = "NULL"; }
        if (msg == null) { msg = "NULL"; }

        if (SendLog)
        {
            if (Hooked)
            {
                OnMessage?.Invoke(Instance, new GameMessage(AppId, sender, msg));
                lastMsg = msg.ToString();
            }

            //if (lastMsg != msg.ToString())
            //{
            //    if (sender.ToUpper() == "DEBUG") Console.ForegroundColor = ConsoleColor.Red;
            //    else Console.ResetColor();

            //    Console.WriteLine($" {sender}: {msg}");
            //    Log.AppEnd(sender + ": " + msg);
            //    lastMsg = msg.ToString();
            //}
        }

        if (lastMsg != msg.ToString())
        {
            if (sender.ToUpper() == "DEBUG") Console.ForegroundColor = ConsoleColor.Red;
            else Console.ResetColor();

            Console.WriteLine($" {sender}: {msg}");
            Log.AppEnd(sender + ": " + msg);
            lastMsg = msg.ToString();
        }
    }

#else

    public static void Write(string sender, object msg)
    {
        // TODO
    }

#endif

    public static void Debug(string v)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" DEBUG: {v}");
    }

}



