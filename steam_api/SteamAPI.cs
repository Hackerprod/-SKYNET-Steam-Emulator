using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.GUI;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;

Implementar SteamAPI_SteamNetworkingIPAddr
public class SteamAPI : SteamInterface
{
    private static HSteamPipe user_steam_pipe;
    private static HSteamUser flat_gs_hsteamuser() => SteamClient.SteamGameServer.GetHSteamUser();
    private static HSteamPipe flat_gs_hsteampipe() => SteamClient.SteamGameServer.GetHSteamPipe();


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_Init()
    {
        if (SteamClient.Initialized)
        {
            Write($"{"SteamAPI_Init [TRUE]"}");
            return true;
        }

        Write($"{"SteamAPI_Init [FALSE]"}");
        SteamClient.Initialize();

        user_steam_pipe = SteamClient.CreateSteamPipe();
        SteamClient.CreateLocalUser(out _, EAccountType.k_EAccountTypeIndividual);
        SteamClient.ConnectToGlobalUser(user_steam_pipe);

        SteamInternal.global_counter++;
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_RunCallbacks()
    {
        Write("SteamAPI_RunCallbacks");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_Shutdown(IntPtr pContextInitData)
    {
        Write("SteamAPI_Shutdown");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public unsafe static void SteamAPI_RegisterCallback(IntPtr pCallback, int iCallback)
    {
        string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

        if (File.Exists(_file))
        {
            modCommon.LoadSettings();
        }
        
        CCallbackBase Base = Marshal.PtrToStructure<CCallbackBase>(pCallback);
        string callMessage = $"SteamAPI_RegisterCallback: ";

        int base_callback = (iCallback / 100) * 100;
        int callback_id = iCallback % 100;

        bool GameServer = (Base.m_nCallbackFlags & CCallbackBase.k_ECallbackFlagsGameServer) != 0;
        string isGameServer = GameServer ? "[ GAMESERVER ]" : "[   CLIENT   ]";
        callMessage += $"{isGameServer} ";

        callMessage += $"{(CallbackType)base_callback} {callback_id} ";

        if (GameServer)
        {
            //SteamClient.Server_Callback.RegisterCallback(pCallback, iCallback);
        }
        else
        {
            //SteamClient.Client_Callback.RegisterCallback(pCallback, iCallback);
        }
        
        Write(callMessage);
    }



    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_InitSafe()
    {
        Write("SteamAPI_InitSafe");
        SteamAPI_Init();
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_InitAnonymousUser()
    {
        Write("SteamAPI_InitAnonymousUser");
        SteamAPI_Init();
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_IsSteamRunning()
    {
        Write($"{"SteamAPI_IsSteamRunning"}");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_RestartAppIfNecessary(uint appId)
    {
        Write($"SteamAPI_RestartAppIfNecessary called {appId}");
        SteamClient.SetAppId(appId);
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_GetSteamInstallPath()
    {
        Write($"{"SteamAPI_GetSteamInstallPath"}");

        string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_GetHSteamUser(int p = 0)
    {
        Write("SteamAPI_GetHSteamUser");
        if (SteamClient.LocalUser == null)
        {
            SteamClient.CreateLocalUser(out _, EAccountType.k_EAccountTypeIndividual);
        }
        return SteamClient.LocalUser;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe SteamAPI_GetHSteamPipe()
    {
        Write("SteamAPI_GetHSteamPipe");
        if (user_steam_pipe == null)
        {
            user_steam_pipe = new HSteamPipe(1);
        }
        return user_steam_pipe;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe GetHSteamPipe()
    {
        Write("GetHSteamPipe");
        return SteamAPI_GetHSteamPipe();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser GetHSteamUser()
    {
        Write("GetHSteamUser");
        return SteamAPI_GetHSteamUser();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_CreateInterface(IntPtr ver)
    {
        Write($"SteamInternal_CreateInterface {ver}");

        IntPtr returnInterface = IntPtr.Zero;

        return returnInterface;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetTryCatchCallbacks(bool bTryCatchCallbacks)
    {
        Write($"SteamAPI_SetTryCatchCallbacks");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetBreakpadAppID(UInt32 unAppID)
    {
        Write($"SteamAPI_SetBreakpadAppID {unAppID}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_UseBreakpadCrashHandler([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback)
    {
        Write($"SteamAPI_UseBreakpadCrashHandler | Date: {pchDate} | Time: {pchTime} | Version : {pchVersion}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ManualDispatch_RunFrame(IntPtr hSteamPipe)
    {
        Write($"SteamAPI_ManualDispatch_RunFrame");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe hSteamPipe, IntPtr pCallbackMsg)
    {
        Write($"SteamAPI_ManualDispatch_GetNextCallback");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ManualDispatch_FreeLastCallback(HSteamPipe hSteamPipe)
    {
        Write($"SteamAPI_ManualDispatch_FreeLastCallback");
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ManualDispatch_GetAPICallResult(HSteamPipe hSteamPipe, IntPtr hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        Write($"SteamAPI_ManualDispatch_GetAPICallResult");
        return true;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_RestartApp(UInt32 appid)
    {
        Write($"SteamAPI_RestartApp");
        return SteamAPI_RestartAppIfNecessary(appid);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] string pchMsg )
    {
        string Msg = "SteamAPI_SetMiniDumpComment" + Environment.NewLine;
        Msg += "////////////////////////////// Mini Dump Content \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
        Msg += $"{pchMsg}" + Environment.NewLine;
        Msg += "//////////////////////////////   End Mini Dump   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
        Write(Msg);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_WriteMiniDump(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID)
    {
        Write($"SteamAPI_WriteMiniDump");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ReleaseCurrentThreadMemory()
    {
        Write($"SteamAPI_ReleaseCurrentThreadMemory");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_UnregisterCallback( CCallbackBase pCallback )
    {
        Write($"SteamAPI_UnregisterCallback");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_gameserveritem_t_Construct(IntPtr self)
    {
        Write($"SteamAPI_gameserveritem_t_Construct");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_gameserveritem_t_GetName(IntPtr self)
    {
        Write($"SteamAPI_gameserveritem_t_GetName");
        return "";
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_gameserveritem_t_SetName(IntPtr self, IntPtr pName)
    {
        Write($"SteamAPI_gameserveritem_t_SetName");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr g_pSteamClientGameServer()
    {
        Write($"g_pSteamClientGameServer");
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void Steam_RegisterInterfaceFuncs(IntPtr hModule)
    {
        Write($"Steam_RegisterInterfaceFuncs");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void Steam_RunCallbacks(IntPtr hSteamPipe, bool bGameServerCallbacks)
    {
        Write("Steam_RunCallbacks\n");

        SteamAPI_RunCallbacks();

        if (bGameServerCallbacks)
           SteamClient.SteamGameServer.RunCallbacks();
    }




    #region Interfaces

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamAppList SteamAPI_SteamAppList_v001()
    {
        Write($"SteamAPI_SteamAppList_v001");
        return SteamClient.GetISteamAppList(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMAPPLIST_INTERFACE_VERSION001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamApps SteamAPI_SteamApps_v008()
    {
        Write($"SteamAPI_SteamApps_v008");
        return SteamClient.GetISteamApps(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMAPPS_INTERFACE_VERSION008");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamApps SteamAPI_SteamGameServerApps_v008()
    {
        Write($"SteamAPI_SteamGameServerApps_v008");
        return SteamClient.GetISteamApps(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMAPPS_INTERFACE_VERSION008");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamController SteamAPI_SteamController_v007()
    {
        Write($"SteamAPI_SteamController_v007");
        return SteamClient.GetISteamController(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamController007");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamController SteamAPI_SteamController_v008()
    {
        Write($"SteamAPI_SteamController_v008");
        return SteamClient.GetISteamController(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamController008");
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamFriends SteamAPI_SteamFriends_v017()
    {
        Write($"SteamAPI_SteamFriends_v017");
        return SteamClient.GetISteamFriends(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamFriends017");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamAPI_SteamUtils_v010()
    {
        Write($"SteamAPI_SteamUtils_v010");
        return SteamClient.GetISteamUtils(SteamAPI_GetHSteamPipe(), "SteamUtils010");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamAPI_SteamGameServerUtils_v010()
    {
        Write($"SteamAPI_SteamGameServerUtils_v010");
        return SteamClient.GetISteamUtils(flat_gs_hsteampipe(), "SteamUtils010");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamAPI_SteamUtils_v009()
    {
        Write($"SteamAPI_SteamUtils_v009");
        return SteamClient.GetISteamUtils(SteamAPI_GetHSteamPipe(), "SteamUtils009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamAPI_SteamGameServerUtils_v009()
    {
        Write($"SteamAPI_SteamGameServerUtils_v009");
        return SteamClient.GetISteamUtils(flat_gs_hsteampipe(), "SteamUtils009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmaking SteamAPI_SteamMatchmaking_v009()
    {
        Write($"SteamAPI_SteamMatchmaking_v009");
        return SteamClient.GetISteamMatchmaking(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamMatchMaking009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmakingServers SteamAPI_SteamMatchmakingServers_v002()
    {
        Write($"SteamAPI_SteamMatchmakingServers_v002");
        return SteamClient.GetISteamMatchmakingServers(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamMatchMakingServers002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameSearch SteamAPI_SteamGameSearch_v001()
    {
        Write($"SteamAPI_SteamGameSearch_v001");
        return SteamClient.GetISteamGameSearch(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamMatchGameSearch001");
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamParties SteamAPI_SteamParties_v002()
    {
        Write($"SteamAPI_SteamParties_v002");
        return SteamClient.GetISteamParties(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamParties002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworking SteamAPI_SteamNetworking_v006()
    {
        Write($"SteamAPI_SteamNetworking_v006");
        return SteamClient.GetISteamNetworking(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworking006");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworking SteamAPI_SteamGameServerNetworking_v006()
    {
        Write($"SteamAPI_SteamGameServerNetworking_v006");
        return SteamClient.GetISteamNetworking(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworking006");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamScreenshots SteamAPI_SteamScreenshots_v003()
    {
        Write($"SteamAPI_SteamScreenshots_v003");
        return SteamClient.GetISteamScreenshots(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMSCREENSHOTS_INTERFACE_VERSION003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusic SteamAPI_SteamMusic_v001()
    {
        Write($"SteamAPI_SteamMusic_v001");
        return SteamClient.GetISteamMusic(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMMUSIC_INTERFACE_VERSION001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusicRemote SteamAPI_SteamMusicRemote_v001()
    {
        Write($"SteamAPI_SteamMusicRemote_v001");
        return SteamClient.GetISteamMusicRemote(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMMUSICREMOTE_INTERFACE_VERSION001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTTP SteamAPI_SteamHTTP_v003()
    {
        Write($"SteamAPI_SteamHTTP_v003");
        return SteamClient.GetISteamHTTP(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMHTTP_INTERFACE_VERSION003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTTP SteamAPI_SteamGameServerHTTP_v003()
    {
        Write($"SteamAPI_SteamGameServerHTTP_v003");
        return SteamClient.GetISteamHTTP(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMHTTP_INTERFACE_VERSION003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInput SteamAPI_SteamInput_v001()
    {
        Write($"SteamAPI_SteamInput_v001");
        return SteamClient.GetISteamInput(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamInput001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInput SteamAPI_SteamInput_v002()
    {
        Write($"SteamAPI_SteamInput_v002");
        return SteamClient.GetISteamInput(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamInput002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTMLSurface SteamAPI_SteamHTMLSurface_v005()
    {
        Write($"SteamAPI_SteamHTMLSurface_v005");
        return SteamClient.GetISteamHTMLSurface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMHTMLSURFACE_INTERFACE_VERSION_005");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInventory SteamAPI_SteamInventory_v003()
    {
        Write($"SteamAPI_SteamInventory_v003");
        return SteamClient.GetISteamInventory(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMINVENTORY_INTERFACE_V003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInventory SteamAPI_SteamGameServerInventory_v003()
    {
        Write($"SteamAPI_SteamGameServerInventory_v003");
        return SteamClient.GetISteamInventory(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMINVENTORY_INTERFACE_V003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamVideo SteamAPI_SteamVideo_v002()
    {
        Write($"SteamAPI_SteamVideo_v002");
        return SteamClient.GetISteamVideo(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMVIDEO_INTERFACE_V002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamTV SteamAPI_SteamTV_v001()
    {
        Write($"SteamAPI_SteamTV_v001");
        return (ISteamTV)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMTV_INTERFACE_V001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamParentalSettings SteamAPI_SteamParentalSettings_v001()
    {
        Write($"SteamAPI_SteamParentalSettings_v001");
        return SteamClient.GetISteamParentalSettings(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMPARENTALSETTINGS_INTERFACE_VERSION001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamRemotePlay SteamAPI_SteamRemotePlay_v001()
    {
        Write($"SteamAPI_SteamRemotePlay_v001");
        return SteamClient.GetISteamRemotePlay(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMREMOTEPLAY_INTERFACE_VERSION001");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingMessages SteamAPI_SteamNetworkingMessages_v002()
    {
        Write($"SteamAPI_SteamNetworkingMessages_v002");
        return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingMessages002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingMessages SteamAPI_SteamNetworkingMessages_SteamAPI_v002()
    {
        Write($"SteamAPI_SteamNetworkingMessages_SteamAPI_v002");
        return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingMessages002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingMessages SteamAPI_SteamGameServerNetworkingMessages_v002()
    {
        Write($"SteamAPI_SteamGameServerNetworkingMessages_v002");
        return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingMessages002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingMessages SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002()
    {
        Write($"SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002");
        return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingMessages002");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_SteamAPI_v009()
    {
        Write($"SteamAPI_SteamNetworkingSockets_SteamAPI_v009");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009()
    {
        Write($"SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_v009()
    {
        Write($"SteamAPI_SteamNetworkingSockets_v009");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_v009()
    {
        Write($"SteamAPI_SteamGameServerNetworkingSockets_v009");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets009");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_v008()
    {
        Write($"SteamAPI_SteamNetworkingSockets_v008");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets008");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_v008()
    {
        Write($"SteamAPI_SteamGameServerNetworkingSockets_v008");
        return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets008");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingUtils SteamAPI_SteamNetworkingUtils_SteamAPI_v003()
    {
        Write($"SteamAPI_SteamNetworkingUtils_SteamAPI_v003");
        return (ISteamNetworkingUtils)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingUtils003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworkingUtils SteamAPI_SteamNetworkingUtils_v003()
    {
        Write($"SteamAPI_SteamNetworkingUtils_v003");
        return (ISteamNetworkingUtils)SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingUtils003");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameServerStats SteamAPI_SteamGameServerStats_v001()
    {
        Write($"SteamAPI_SteamGameServerStats_v001");
        return default;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamAppList SteamAppList()
    {
        Write($"SteamAppList");
        return SteamClient.SteamAppList;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamApps SteamApps()
    {
        Write($"ISteamApps");
        return SteamClient.SteamApps;
    }

    [DllExport("SteamClient", CallingConvention = CallingConvention.Cdecl)]
    public static SteamClient steamClient()
    {
        Write($"SteamClient");
        return SteamClient.Instance;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamController SteamController()
    {
        Write($"SteamController");
        return SteamClient.SteamController;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamFriends SteamFriends()
    {
        Write($"SteamFriends");
        return SteamClient.SteamFriends;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameServer SteamGameServer()
    {
        Write($"SteamGameServer");
        return SteamClient.SteamGameServer;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamApps SteamGameServerApps()
    {
        Write($"SteamGameServerApps");
        return SteamClient.SteamGameServerApps;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTTP SteamGameServerHTTP()
    {
        Write($"SteamGameServerHTTP");
        return SteamClient.SteamHTTP;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInventory SteamGameServerInventory()
    {
        Write($"SteamGameServerInventory");
        return SteamClient.SteamGameServerInventory;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworking SteamGameServerNetworking()
    {
        Write($"SteamGameServerNetworking");
        return SteamClient.SteamGameServerNetworking;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamGameServerStats SteamGameServerStats()
    {
        Write($"SteamGameServerStats");
        return SteamClient.SteamGameServerStats;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUGC SteamGameServerUGC()
    {
        Write($"SteamGameServerUGC");
        return SteamClient.SteamUGC;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamGameServerUtils()
    {
        Write($"SteamGameServerUtils");
        return SteamClient.SteamGameServerUtils;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTTP SteamHTTP()
    {
        Write($"SteamHTTP");
        return SteamClient.SteamHTTP;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamHTMLSurface SteamHTMLSurface()
    {
        Write($"SteamHTMLSurface");
        return SteamClient.SteamHTMLSurface;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamInventory SteamInventory()
    {
        Write($"SteamInventory");
        return SteamClient.SteamInventory;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMasterServerUpdater SteamMasterServerUpdater()
    {
        Write($"SteamMasterServerUpdater");
        return SteamClient.SteamMasterServerUpdater;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmaking SteamMatchmaking()
    {
        Write($"SteamMatchmaking");
        return SteamClient.SteamMatchmaking;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMatchmakingServers SteamMatchmakingServers()
    {
        Write($"SteamMatchmakingServers");
        return SteamClient.SteamMatchmakingServers;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusic SteamMusic()
    {
        Write($"SteamMusic");
        return SteamClient.SteamMusic;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamMusicRemote SteamMusicRemote()
    {
        Write($"SteamMusicRemote");
        return SteamClient.SteamMusicRemote;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamNetworking SteamNetworking()
    {
        Write($"SteamNetworking");
        return SteamClient.SteamNetworking;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamParentalSettings SteamParentalSettings()
    {
        Write($"SteamParentalSettings");
        return SteamClient.SteamParentalSettings;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamRemoteStorage SteamRemoteStorage()
    {
        Write($"SteamRemoteStorage");
        return SteamClient.SteamRemoteStorage;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamScreenshots SteamScreenshots()
    {
        Write($"SteamScreenshots");
        return SteamClient.SteamScreenshots;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUGC SteamUGC()
    {
        Write($"SteamUGC");
        return SteamClient.SteamUGC;
    }

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamUnifiedMessages SteamUnifiedMessages()
    //{
    //    Write($"SteamUnifiedMessages");
    //    return SteamClient.SteamUnifiedMessages;
    //}

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUser SteamUser()
    {
        Write($"SteamUser");
        return SteamClient.SteamUser;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUserStats SteamUserStats()
    {
        Write($"SteamUserStats");
        return SteamClient.SteamUserStats;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamUtils SteamUtils()
    {
        Write($"SteamUtils");
        return SteamClient.SteamUtils;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ISteamVideo SteamVideo()
    {
        Write($"SteamVideo");
        return SteamClient.SteamVideo;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void VR_GetGenericInterface(string pchInterfaceVersion, int peError )
    {
        Write($"VR_GetGenericInterface version {pchInterfaceVersion}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string VR_GetStringForHmdError(int error)
    {
        Write($"VR_GetStringForHmdError");
        return "";
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void VR_Init(int error, int type)
    {
        Write($"VR_Init");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool VR_IsHmdPresent()
    {
        Write($"VR_IsHmdPresent");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void VR_Shutdown()
    {
        Write($"VR_Shutdown");
    }


    #endregion


}

