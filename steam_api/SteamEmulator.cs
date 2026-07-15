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
using System.Diagnostics;
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
    public static bool UnlockAllDLC;

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
    public static bool UseServerApi;
    // Enables SKYNET-issued SDR certificates and the native CA patch. Keep this
    // disabled for unauthenticated LAN transport unless secure SDR is required.
    public static bool SecureNetworking;
    // Controls the SteamGameServer VAC-secure policy separately from SDR certs.
    // SecureNetworking can be true while the game server is advertised as
    // non-VAC-secure, which is required when Valve VAC is not present.
    public static bool VacSecureGameServer;
    public static string ServerUrl;
    public static string AccessToken;
    public static string RefreshToken;
    public static string ClientInstanceId;
    public static bool UseActiveWebUser;
    public static int PollIntervalMs;
    public static int HttpTimeoutMs;
    public static int DiscoveryPort;

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
    public static SteamTimeline SteamTimeline;
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
        SteamID_GS = CSteamID.CreateOne(true); 
        AppID = 0;
        DLCs = new List<DLC>();
        UnlockAllDLC = true;
        LogToFile = true;
        LogToConsole = true;
        ISteamHTTP = true;
        UseServerApi = true;
        SecureNetworking = false;
        VacSecureGameServer = false;
        ServerUrl = "http://127.0.0.1:27080/";
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        ClientInstanceId = string.Empty;
        UseActiveWebUser = true;
        PollIntervalMs = 50;
        HttpTimeoutMs = 8000;
        DiscoveryPort = 27081;
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
            Write($"Process PID={Process.GetCurrentProcess().Id} Role={Environment.GetEnvironmentVariable("SKYNET_PROCESS_ROLE") ?? "client"} CommandLine={Environment.CommandLine}");
            Write($"Networking security mode: {(SecureNetworking ? "secure SDR certificate" : "insecure LAN (no SDR certificate)")}");

            if (SecureNetworking)
            {
                bool diskPatched = SdrCertPatcher.EnsureDiskPatched();
                Write($"SDR disk CA patch ready: {diskPatched}");
            }

            //UserManager.Initialize();
            //NetworkManager.Initialize();
            Write("Initializing InterfaceManager");
            InterfaceManager.Initialize();
            Write("InterfaceManager initialized");

            Write("Initializing SkyNetApiClient");
            SkyNetApiClient.Initialize();
            Write("SkyNetApiClient initialized");

            // Memory patching is disabled. Disk patching is checked before the
            // networking interfaces ask for SDR config or a certificate.

            #region Interface Initialization

            // Client Interfaces

            Write("Creating client interfaces");
            SteamClient = CreateInterface("SteamClient", () => new SteamClient());

            SteamUser = CreateInterface("SteamUser", () => new SKYNET.Steamworks.Implementation.SteamUser());

            SteamFriends = CreateInterface("SteamFriends", () => new SteamFriends());

            SteamUtils = CreateInterface("SteamUtils", () => new SteamUtils());

            SteamMatchmaking = CreateInterface("SteamMatchmaking", () => new SteamMatchmaking());

            SteamMatchMakingServers = CreateInterface("SteamMatchMakingServers", () => new SteamMatchMakingServers());

            SteamUserStats = CreateInterface("SteamUserStats", () => new SteamUserStats());

            SteamApps = CreateInterface("SteamApps", () => new SteamApps());

            SteamNetworking = CreateInterface("SteamNetworking", () => new SteamNetworking());

            SteamRemoteStorage = CreateInterface("SteamRemoteStorage", () => new SteamRemoteStorage());

            SteamScreenshots = CreateInterface("SteamScreenshots", () => new SteamScreenshots());

            SteamHTTP = CreateInterface("SteamHTTP", () => new SteamHTTP());

            SteamController = CreateInterface("SteamController", () => new SteamController());

            SteamUGC = CreateInterface("SteamUGC", () => new SteamUGC());

            SteamAppList = CreateInterface("SteamAppList", () => new SteamAppList());

            SteamMusic = CreateInterface("SteamMusic", () => new SteamMusic());

            SteamMusicRemote = CreateInterface("SteamMusicRemote", () => new SteamMusicRemote());

            SteamHTMLSurface = CreateInterface("SteamHTMLSurface", () => new SteamHTMLSurface());

            SteamInventory = CreateInterface("SteamInventory", () => new SteamInventory());

            SteamVideo = CreateInterface("SteamVideo", () => new SteamVideo());

            SteamParentalSettings = CreateInterface("SteamParentalSettings", () => new SteamParentalSettings());

            SteamNetworkingSockets = CreateInterface("SteamNetworkingSockets", () => new SteamNetworkingSockets());

            SteamNetworkingSocketsSerialized = CreateInterface("SteamNetworkingSocketsSerialized", () => new SteamNetworkingSocketsSerialized());

            SteamNetworkingMessages = CreateInterface("SteamNetworkingMessages", () => new SteamNetworkingMessages());

            SteamGameCoordinator = CreateInterface("SteamGameCoordinator", () => new SteamGameCoordinator());

            SteamNetworkingUtils = CreateInterface("SteamNetworkingUtils", () => new SteamNetworkingUtils());

            SteamGameSearch = CreateInterface("SteamGameSearch", () => new SteamGameSearch());

            SteamParties = CreateInterface("SteamParties", () => new SteamParties());

            SteamRemotePlay = CreateInterface("SteamRemotePlay", () => new SteamRemotePlay());

            SteamTimeline = CreateInterface("SteamTimeline", () => new SteamTimeline());

            SteamTV = CreateInterface("SteamTV", () => new SteamTV());

            SteamInput = CreateInterface("SteamInput", () => new SteamInput());

            // Server Interfaces

            Write("Creating server interfaces");
            SteamGameServer = CreateInterface("SteamGameServer", () => new SteamGameServer());

            SteamGameServerStats = CreateInterface("SteamGameServerStats", () => new SteamGameServerStats());

            SteamMasterServerUpdater = CreateInterface("SteamMasterServerUpdater", () => new SteamMasterServerUpdater());

            #endregion
            
            HSteamUser = 1;
            HSteamPipe = 1;

            HSteamUser_GS = 2;
            HSteamPipe_GS = 2;

            Initialized = true;
            Initializing = false;
            Write("Steam emulator initialized");
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

    private static T CreateInterface<T>(string name, Func<T> factory)
    {
        Write($"Creating {name}");
        try
        {
            var instance = factory();
            Write($"{name} created");
            return instance;
        }
        catch (Exception ex)
        {
            Write($"{name} creation failed: {ex}");
            throw;
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
        if (ShouldSuppressHotLog(sender, msg))
        {
            return;
        }

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
        if (ShouldSuppressHotLog(sender, msg))
        {
            return;
        }

        MutexHelper.Wait("LOG", delegate
        {
            string message = " ";
            message += string.IsNullOrEmpty(sender) ? "" : $"{sender}: ";
            message += msg == null ? "NULL" : msg;

            if (LogToFile)
            {
                Log.AppEnd(message);
            }

            if (LogToConsole)
            {
                ConsoleHelper.WriteLine(message);
            }
        });
    }

#endif

    private static bool ShouldSuppressHotLog(string sender, object msg)
    {
        if (msg == null)
        {
            return false;
        }

        string text = msg.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        switch (text)
        {
            case "SteamAPI_RunCallbacks":
            case "SteamGameServer_RunCallbacks":
            case "RunFrame":
            case "GetServerRealTime":
            case "IsMessageAvailable = False":
                return true;
        }

        if (sender == "CallbackManager")
        {
            if (text.StartsWith("Added Callback 304 ", StringComparison.Ordinal) ||
                text.StartsWith("Registered callback 304 ", StringComparison.Ordinal) ||
                text.StartsWith("Added Callback 3406 ", StringComparison.Ordinal) ||
                text.StartsWith("Registered callback 3406 ", StringComparison.Ordinal))
            {
                return true;
            }
        }

        if (sender == "SteamAPI")
        {
            if ((text.Contains("  3406 ") && text.Contains("DownloadItemResult")) ||
                text == "SteamAPI_UnregisterCallback DownloadItemResult OK" ||
                text == "SteamAPI_UnregisterCallback PersonaStateChange OK")
            {
                return true;
            }
        }

        return text.Contains("GetPersonaState  = k_EPersonaStateOnline");
    }

    public static void Debug(object msg, ConsoleColor color = ConsoleColor.Red)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($" DEBUG: {msg}");

        MessageBox.Show("DEBUG");
    }
}



