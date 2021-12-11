using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI : BaseCalls
{
    private static HSteamPipe user_steam_pipe;
    private static HSteamUser flat_hsteamuser() => SteamAPI_GetHSteamUser();
    private static HSteamPipe flat_hsteampipe() => SteamAPI_GetHSteamPipe();
    private static HSteamUser flat_gs_hsteamuser() => Steam_GameServer.SteamGameServer_GetHSteamUser();
    private static HSteamPipe flat_gs_hsteampipe() => Steam_GameServer.SteamGameServer_GetHSteamPipe();




    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_Init()
    {
        DEBUG($"{"SteamAPI_Init"}");

        SteamClient.Initialize();
        //if (user_steam_pipe != null) return true;

        user_steam_pipe = SteamClient.CreateSteamPipe();
        SteamClient.ConnectToGlobalUser(user_steam_pipe);

        SteamInternal.global_counter++;
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_RunCallbacks()
    {
        DEBUG("SteamAPI_RunCallbacks");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_Shutdown(IntPtr pContextInitData)
    {
        DEBUG("SteamAPI_Shutdown");

    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public unsafe static void SteamAPI_RegisterCallback(/*[MarshalAs(UnmanagedType.FunctionPtr)]*/ IntPtr pCallback, int iCallback)
    {
        string callMessage = "SteamAPI_RegisterCallback: ";

        int base_callback = (iCallback / 100) * 100;
        int callback_id = iCallback % 100;

        bool isGameServer = false;

        callMessage += $"{(CallbackType)base_callback} {callback_id}";

        DEBUG(callMessage);
    }



    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_InitSafe()
    {
        DEBUG("SteamAPI_InitSafe");
        SteamAPI_Init();
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_InitAnonymousUser()
    {
        DEBUG("SteamAPI_InitAnonymousUser");
        SteamAPI_Init();
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_IsSteamRunning()
    {
        DEBUG($"{"SteamAPI_IsSteamRunning"}");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_RestartAppIfNecessary(uint appId)
    {
        DEBUG($"SteamAPI_RestartAppIfNecessary called {appId}");
        SteamClient.SetAppId(appId);
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_GetSteamInstallPath()
    {
        DEBUG($"{"SteamAPI_GetSteamInstallPath"}");

        string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        return path;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe SteamAPI_GetHSteamPipe()
    {
        //SteamAPIInterop.SteamAPI_ISteamClient_CreateSteamPipe();

        DEBUG("SteamAPI_GetHSteamPipe");
        if (user_steam_pipe == null)
        {
            user_steam_pipe = new HSteamPipe(1);
        }
        return user_steam_pipe;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_GetHSteamUser()
    {
        DEBUG("SteamAPI_GetHSteamUser");
        //if (!get_steam_client()->user_logged_in) return 0;
        return (HSteamUser)1;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamPipe GetHSteamPipe()
    {
        DEBUG("GetHSteamPipe");
        return SteamAPI_GetHSteamPipe();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser GetHSteamUser()
    {
        DEBUG("GetHSteamUser");
        return SteamAPI_GetHSteamUser();
    }

    [DllExport("SteamClient", CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr _SteamClient()
    {
        DEBUG("SteamClient");
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string ver)
    {
        DEBUG($"SteamInternal_CreateInterface {ver}");

        IntPtr returnInterface = IntPtr.Zero;

        return returnInterface;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetTryCatchCallbacks(bool bTryCatchCallbacks)
    {
        DEBUG($"SteamAPI_SetTryCatchCallbacks");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetBreakpadAppID(UInt32 unAppID)
    {
        DEBUG($"SteamAPI_SetBreakpadAppID {unAppID}");
    }
    //
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_UseBreakpadCrashHandler([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback)
    {
        DEBUG($"SteamAPI_UseBreakpadCrashHandler {pchDate} {pchTime}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ManualDispatch_RunFrame(IntPtr hSteamPipe)
    {
        DEBUG($"SteamAPI_ManualDispatch_RunFrame");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe hSteamPipe, CallbackMsg_t pCallbackMsg)
    {
        DEBUG($"SteamAPI_ManualDispatch_GetNextCallback");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ManualDispatch_FreeLastCallback(HSteamPipe hSteamPipe)
    {
        DEBUG($"SteamAPI_ManualDispatch_FreeLastCallback");
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ManualDispatch_GetAPICallResult(HSteamPipe hSteamPipe, IntPtr hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        DEBUG($"SteamAPI_ManualDispatch_GetAPICallResult");
        return true;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_RestartApp(UInt32 appid)
    {
        DEBUG($"SteamAPI_RestartApp");
        return SteamAPI_RestartAppIfNecessary(appid);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] string pchMsg )
    {

        DEBUG($"SteamAPI_SetMiniDumpComment: {pchMsg}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_WriteMiniDump(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID)
    {
        DEBUG($"SteamAPI_WriteMiniDump");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ReleaseCurrentThreadMemory()
    {
        DEBUG($"SteamAPI_ReleaseCurrentThreadMemory");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_UnregisterCallback( CCallbackBase pCallback )
    {
        DEBUG($"SteamAPI_UnregisterCallback");
    }

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static void SteamAPI_gameserveritem_t_Construct(gameserveritem_t self)
    //{
    //    PRINT_DEBUG($"SteamAPI_gameserveritem_t_Construct");
    //    new gameserveritem_t();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static string SteamAPI_gameserveritem_t_GetName(gameserveritem_t self)
    //{
    //    PRINT_DEBUG($"SteamAPI_gameserveritem_t_GetName");
    //    return self.GetServerName();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static void SteamAPI_gameserveritem_t_SetName(gameserveritem_t self, [MarshalAs(UnmanagedType.LPStr)] string pName )
    //{
    //    PRINT_DEBUG($"SteamAPI_gameserveritem_t_SetName");
    //    self.SetServerName(pName);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static IntPtr g_pSteamClientGameServer()
    //{
    //    PRINT_DEBUG($"g_pSteamClientGameServer");
    //    return IntPtr.Zero;
    //}

    //#region Interfaces

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static IntPtr SteamAPI_SteamAppList_v001()
    //{
    //    PRINT_DEBUG($"SteamAPI_SteamAppList_v001");
    //    return SteamClient.GetISteamAppList(flat_hsteamuser(), flat_hsteampipe(), "STEAMAPPLIST_INTERFACE_VERSION001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamApps SteamAPI_SteamApps_v008()
    //{
    //    return SteamClient.GetISteamApps(flat_hsteamuser(), flat_hsteampipe(), "STEAMAPPS_INTERFACE_VERSION008");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamApps SteamAPI_SteamGameServerApps_v008()
    //{
    //    return SteamClient.GetISteamApps(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMAPPS_INTERFACE_VERSION008");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static IntPtr SteamAPI_SteamController_v007()
    //{
    //    return SteamClient.GetISteamController(flat_hsteamuser(), flat_hsteampipe(), "SteamController007");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static IntPtr SteamAPI_SteamController_v008()
    //{
    //    return SteamClient.GetISteamController(flat_hsteamuser(), flat_hsteampipe(), "SteamController008");
    //}


    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static IntPtr SteamAPI_SteamFriends_v017()
    //{
    //    return SteamClient.GetISteamFriends(flat_hsteamuser(), flat_hsteampipe(), "SteamFriends017");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamUtils SteamAPI_SteamUtils_v010()
    //{
    //    return SteamClient.GetISteamUtils(flat_hsteampipe(), "SteamUtils010");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamUtils SteamAPI_SteamGameServerUtils_v010()
    //{
    //    return SteamClient.GetISteamUtils(flat_gs_hsteampipe(), "SteamUtils010");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamUtils SteamAPI_SteamUtils_v009()
    //{
    //    return SteamClient.GetISteamUtils(flat_hsteampipe(), "SteamUtils009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamUtils SteamAPI_SteamGameServerUtils_v009()
    //{
    //    return SteamClient.GetISteamUtils(flat_gs_hsteampipe(), "SteamUtils009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamMatchmaking SteamAPI_SteamMatchmaking_v009()
    //{
    //    return SteamClient.GetISteamMatchmaking(flat_hsteamuser(), flat_hsteampipe(), "SteamMatchMaking009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamMatchmakingServers SteamAPI_SteamMatchmakingServers_v002()
    //{
    //    return SteamClient.GetISteamMatchmakingServers(flat_hsteamuser(), flat_hsteampipe(), "SteamMatchMakingServers002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamGameSearch SteamAPI_SteamGameSearch_v001()
    //{
    //    return SteamClient.GetISteamGameSearch(flat_hsteamuser(), flat_hsteampipe(), "SteamMatchGameSearch001");
    //}


    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamParties SteamAPI_SteamParties_v002()
    //{
    //    return SteamClient.GetISteamParties(flat_hsteamuser(), flat_hsteampipe(), "SteamParties002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworking SteamAPI_SteamNetworking_v006()
    //{
    //    return SteamClient.GetISteamNetworking(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworking006");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworking SteamAPI_SteamGameServerNetworking_v006()
    //{
    //    return SteamClient.GetISteamNetworking(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworking006");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamScreenshots SteamAPI_SteamScreenshots_v003()
    //{
    //    return SteamClient.GetISteamScreenshots(flat_hsteamuser(), flat_hsteampipe(), "STEAMSCREENSHOTS_INTERFACE_VERSION003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamMusic SteamAPI_SteamMusic_v001()
    //{
    //    return SteamClient.GetISteamMusic(flat_hsteamuser(), flat_hsteampipe(), "STEAMMUSIC_INTERFACE_VERSION001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamMusicRemote SteamAPI_SteamMusicRemote_v001()
    //{
    //    return SteamClient.GetISteamMusicRemote(flat_hsteamuser(), flat_hsteampipe(), "STEAMMUSICREMOTE_INTERFACE_VERSION001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamHTTP SteamAPI_SteamHTTP_v003()
    //{
    //    return SteamClient.GetISteamHTTP(flat_hsteamuser(), flat_hsteampipe(), "STEAMHTTP_INTERFACE_VERSION003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamHTTP SteamAPI_SteamGameServerHTTP_v003()
    //{
    //    return SteamClient.GetISteamHTTP(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMHTTP_INTERFACE_VERSION003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamInput SteamAPI_SteamInput_v001()
    //{
    //    return SteamClient.GetISteamInput(flat_hsteamuser(), flat_hsteampipe(), "SteamInput001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamInput SteamAPI_SteamInput_v002()
    //{
    //    return SteamClient.GetISteamInput(flat_hsteamuser(), flat_hsteampipe(), "SteamInput002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamHTMLSurface SteamAPI_SteamHTMLSurface_v005()
    //{
    //    return SteamClient.GetISteamHTMLSurface(flat_hsteamuser(), flat_hsteampipe(), "STEAMHTMLSURFACE_INTERFACE_VERSION_005");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamInventory SteamAPI_SteamInventory_v003()
    //{
    //    return SteamClient.GetISteamInventory(flat_hsteamuser(), flat_hsteampipe(), "STEAMINVENTORY_INTERFACE_V003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamInventory SteamAPI_SteamGameServerInventory_v003()
    //{
    //    return SteamClient.GetISteamInventory(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "STEAMINVENTORY_INTERFACE_V003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamVideo SteamAPI_SteamVideo_v002()
    //{
    //    return SteamClient.GetISteamVideo(flat_hsteamuser(), flat_hsteampipe(), "STEAMVIDEO_INTERFACE_V002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamTV SteamAPI_SteamTV_v001()
    //{
    //    return (ISteamTV)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "STEAMTV_INTERFACE_V001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamParentalSettings SteamAPI_SteamParentalSettings_v001()
    //{
    //    return SteamClient.GetISteamParentalSettings(flat_hsteamuser(), flat_hsteampipe(), "STEAMPARENTALSETTINGS_INTERFACE_VERSION001");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamRemotePlay SteamAPI_SteamRemotePlay_v001()
    //{
    //    return SteamClient.GetISteamRemotePlay(flat_hsteamuser(), flat_hsteampipe(), "STEAMREMOTEPLAY_INTERFACE_VERSION001");
    //}


    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingMessages SteamAPI_SteamNetworkingMessages_v002()
    //{
    //    return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingMessages002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingMessages SteamAPI_SteamNetworkingMessages_SteamAPI_v002()
    //{
    //    return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingMessages002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingMessages SteamAPI_SteamGameServerNetworkingMessages_v002()
    //{
    //    return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingMessages002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingMessages SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002()
    //{
    //    return (ISteamNetworkingMessages)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingMessages002");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_SteamAPI_v009()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingSockets009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_v009()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingSockets009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_v009()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets009");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamNetworkingSockets_v008()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingSockets008");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingSockets SteamAPI_SteamGameServerNetworkingSockets_v008()
    //{
    //    return (ISteamNetworkingSockets)SteamClient.GetISteamGenericInterface(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamNetworkingSockets008");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingUtils SteamAPI_SteamNetworkingUtils_SteamAPI_v003()
    //{
    //    return (ISteamNetworkingUtils)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingUtils003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamNetworkingUtils SteamAPI_SteamNetworkingUtils_v003()
    //{
    //    return (ISteamNetworkingUtils)SteamClient.GetISteamGenericInterface(flat_hsteamuser(), flat_hsteampipe(), "SteamNetworkingUtils003");
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static ISteamGameServerStats SteamAPI_SteamGameServerStats_v001()
    //{
    //    return SteamClient.GetISteamGameServerStats(flat_gs_hsteamuser(), flat_gs_hsteampipe(), "SteamGameServerStats001");
    //}














    //#endregion



}

