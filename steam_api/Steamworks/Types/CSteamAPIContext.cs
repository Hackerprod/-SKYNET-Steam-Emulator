using SKYNET;
using SKYNET.Helper;
using System;
using System.Runtime.InteropServices;


// CSteamAPIContext encapsulates the Steamworks API global accessors into
// a single object.
//
// DEPRECATED: Used the global interface accessors instead!
//
// This will be removed in a future iteration of the SDK

///[StructLayout(LayoutKind.Sequential)]
public class CSteamApiContext : SteamInterface
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

    public IntPtr SteamClient(IntPtr _) => m_pSteamClient;
    public IntPtr SteamUser(IntPtr _) => m_pSteamUser;
    public IntPtr SteamFriends(IntPtr _) => m_pSteamFriends;
    public IntPtr SteamUtils(IntPtr _) => m_pSteamUtils;
    public IntPtr SteamMatchmaking(IntPtr _) => m_pSteamMatchmaking;
    public IntPtr SteamGameSearch(IntPtr _) => m_pSteamGameSearch;
    public IntPtr SteamUserStats(IntPtr _) => m_pSteamUserStats;
    public IntPtr SteamApps(IntPtr _) => m_pSteamApps;
    public IntPtr SteamMatchmakingServers(IntPtr _) => m_pSteamMatchmakingServers;
    public IntPtr SteamNetworking(IntPtr _) => m_pSteamNetworking;
    public IntPtr SteamRemoteStorage(IntPtr _) => m_pSteamRemoteStorage;
    public IntPtr SteamScreenshots(IntPtr _) => m_pSteamScreenshots;
    public IntPtr SteamHTTP(IntPtr _) => m_pSteamHTTP;
    public IntPtr SteamController(IntPtr _) => m_pSteamController;
    public IntPtr SteamUGC(IntPtr _) => m_pSteamUGC;
    public IntPtr SteamAppList(IntPtr _) => m_pSteamAppList;
    public IntPtr SteamMusic(IntPtr _) => m_pSteamMusic;
    public IntPtr SteamMusicRemote(IntPtr _) => m_pSteamMusicRemote;
    public IntPtr SteamHTMLSurface(IntPtr _) => m_pSteamHTMLSurface;
    public IntPtr SteamInventory(IntPtr _) => m_pSteamInventory;
    public IntPtr SteamVideo(IntPtr _) => m_pSteamVideo;
    public IntPtr SteamTV(IntPtr _) => m_pSteamTV;
    public IntPtr SteamParentalSettings(IntPtr _) => m_pSteamParentalSettings;
    public IntPtr SteamInput(IntPtr _) => m_pSteamInput;

    public void Clear(IntPtr _)
    {
        SteamEmulator.Write($"Cleaning CSteamApiContext");

        m_pSteamClient = IntPtr.Zero;
        m_pSteamUser = IntPtr.Zero;
        m_pSteamFriends = IntPtr.Zero;
        m_pSteamUtils = IntPtr.Zero;
        m_pSteamMatchmaking = IntPtr.Zero;
        m_pSteamUserStats = IntPtr.Zero;
        m_pSteamApps = IntPtr.Zero;
        m_pSteamMatchmakingServers = IntPtr.Zero;
        m_pSteamNetworking = IntPtr.Zero;
        m_pSteamRemoteStorage = IntPtr.Zero;
        m_pSteamScreenshots = IntPtr.Zero;
        m_pSteamHTTP = IntPtr.Zero;
        m_pSteamController = IntPtr.Zero;
        m_pSteamUGC = IntPtr.Zero;
        m_pSteamAppList = IntPtr.Zero;
        m_pSteamMusic = IntPtr.Zero;
        m_pSteamMusicRemote = IntPtr.Zero;
        m_pSteamHTMLSurface = IntPtr.Zero;
        m_pSteamInventory = IntPtr.Zero;
        m_pSteamVideo = IntPtr.Zero;

        SteamEmulator.Write($"CSteamApiContext cleaned");
    }

    public bool Init(IntPtr _)
    {
        SteamEmulator.Write($"Initializing CSteamApiContext");

        var a_steamUser = SteamEmulator.HSteamUser;
        var a_steamPipe = SteamEmulator.HSteamPipe;

        if ((int)a_steamPipe == 0)
        {
            return false;
        }

        m_pSteamClient = SteamEmulator.SteamClient.BaseAddress;
        if (m_pSteamClient == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamUser = SteamEmulator.SteamUser.BaseAddress;
        if (m_pSteamUser == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamFriends = SteamEmulator.SteamFriends.BaseAddress;
        if (m_pSteamFriends == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamUtils = SteamEmulator.SteamUtils.BaseAddress;
        if (m_pSteamUtils == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamMatchmaking = SteamEmulator.SteamMatchmaking.BaseAddress;
        if (m_pSteamMatchmaking == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamMatchmakingServers = SteamEmulator.SteamMatchMakingServers.BaseAddress;
        if (m_pSteamMatchmakingServers == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamUserStats = SteamEmulator.SteamUserStats.BaseAddress;
        if (m_pSteamUserStats == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamApps = SteamEmulator.SteamApps.BaseAddress;
        if (m_pSteamApps == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamNetworking = SteamEmulator.SteamNetworking.BaseAddress;
        if (m_pSteamNetworking == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamRemoteStorage = SteamEmulator.SteamMusicRemote.BaseAddress;
        if (m_pSteamRemoteStorage == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamScreenshots = SteamEmulator.SteamScreenshots.BaseAddress;
        if (m_pSteamScreenshots == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamHTTP = SteamEmulator.SteamHTTP.BaseAddress;
        if (m_pSteamHTTP == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamController = SteamEmulator.SteamController.BaseAddress;
        if (m_pSteamController == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamUGC = SteamEmulator.SteamUGC.BaseAddress;
        if (m_pSteamUGC == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamAppList = SteamEmulator.SteamAppList.BaseAddress;
        if (m_pSteamAppList == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamMusic = SteamEmulator.SteamMusic.BaseAddress;
        if (m_pSteamMusic == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamMusicRemote = SteamEmulator.SteamMusicRemote.BaseAddress;
        if (m_pSteamMusicRemote == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamHTMLSurface = SteamEmulator.SteamHTMLSurface.BaseAddress;
        if (m_pSteamHTMLSurface == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamInventory = SteamEmulator.SteamInventory.BaseAddress;
        if (m_pSteamInventory == IntPtr.Zero)
        {
            return false;
        }

        m_pSteamVideo = SteamEmulator.SteamVideo.BaseAddress;
        if (m_pSteamVideo == IntPtr.Zero)
        {
            return false;
        }

        return true;
    }
}

[Delegate(Name = "CSteamApiContext")]
public class CSteamApiContext_Delegates
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamClient(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamUser(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamFriends(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamUtils(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamMatchmaking(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamGameSearch(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamUserStats(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamApps(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamMatchmakingServers(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamNetworking(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamRemoteStorage(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamScreenshots(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamHTTP(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamController(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamUGC(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamAppList(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamMusic(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamMusicRemote(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamHTMLSurface(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamInventory(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamVideo(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamTV(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamParentalSettings(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SteamInput(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate bool Init(IntPtr _);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void Clear(IntPtr _);
}
