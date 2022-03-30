using EasyHook;
using SKYNET.Helper;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            // SteamApi Handles
            base.Install<SteamAPI_InitDelegate>("SteamAPI_Init", _SteamAPI_InitDelegate, new SteamAPI_InitDelegate(SteamAPI_Init));
            base.Install<SteamAPI_RunCallbacksDelegate>("SteamAPI_RunCallbacks", _SteamAPI_RunCallbacksDelegate, new SteamAPI_RunCallbacksDelegate(SteamAPI_RunCallbacks));
            base.Install<SteamAPI_RegisterCallResultDelegate>("SteamAPI_RegisterCallResult", _SteamAPI_RegisterCallResultDelegate, new SteamAPI_RegisterCallResultDelegate(SteamAPI_RegisterCallResult));
            base.Install<SteamAPI_ShutdownDelegate>("SteamAPI_Shutdown", _SteamAPI_ShutdownDelegate, new SteamAPI_ShutdownDelegate(SteamAPI_Shutdown));
            base.Install<SteamAPI_UnregisterCallbackDelegate>("SteamAPI_UnregisterCallback", _SteamAPI_UnregisterCallbackDelegate, new SteamAPI_UnregisterCallbackDelegate(SteamAPI_UnregisterCallback));
            base.Install<SteamAPI_UnregisterCallResultDelegate>("SteamAPI_UnregisterCallResult", _SteamAPI_UnregisterCallResultDelegate, new SteamAPI_UnregisterCallResultDelegate(SteamAPI_UnregisterCallResult));
            base.Install<SteamAPI_RegisterCallbackDelegate>("SteamAPI_RegisterCallback", _SteamAPI_RegisterCallbackDelegate, new SteamAPI_RegisterCallbackDelegate(SteamAPI_RegisterCallback));
            base.Install<SteamAPI_InitSafeDelegate>("SteamAPI_InitSafe", _SteamAPI_InitSafeDelegate, new SteamAPI_InitSafeDelegate(SteamAPI_InitSafe));
            base.Install<SteamAPI_InitAnonymousUserDelegate>("SteamAPI_InitAnonymousUser", _SteamAPI_InitAnonymousUserDelegate, new SteamAPI_InitAnonymousUserDelegate(SteamAPI_InitAnonymousUser));
            base.Install<SteamAPI_IsSteamRunningDelegate>("SteamAPI_IsSteamRunning", _SteamAPI_IsSteamRunningDelegate, new SteamAPI_IsSteamRunningDelegate(SteamAPI_IsSteamRunning));
            base.Install<SteamAPI_RestartAppIfNecessaryDelegate>("SteamAPI_RestartAppIfNecessary", _SteamAPI_RestartAppIfNecessaryDelegate, new SteamAPI_RestartAppIfNecessaryDelegate(SteamAPI_RestartAppIfNecessary));
            base.Install<SteamAPI_GetSteamInstallPathDelegate>("SteamAPI_GetSteamInstallPath", _SteamAPI_GetSteamInstallPathDelegate, new SteamAPI_GetSteamInstallPathDelegate(SteamAPI_GetSteamInstallPath));
            base.Install<SteamAPI_GetHSteamUserDelegate>("SteamAPI_GetHSteamUser", _SteamAPI_GetHSteamUserDelegate, new SteamAPI_GetHSteamUserDelegate(SteamAPI_GetHSteamUser));
            base.Install<SteamAPI_GetHSteamPipeDelegate>("SteamAPI_GetHSteamPipe", _SteamAPI_GetHSteamPipeDelegate, new SteamAPI_GetHSteamPipeDelegate(SteamAPI_GetHSteamPipe));
            base.Install<GetHSteamPipeDelegate>("GetHSteamPipe", _GetHSteamPipeDelegate, new GetHSteamPipeDelegate(GetHSteamPipe));
            base.Install<GetHSteamUserDelegate>("GetHSteamUser", _GetHSteamUserDelegate, new GetHSteamUserDelegate(GetHSteamUser));
            base.Install<SteamAPI_SetTryCatchCallbacksDelegate>("SteamAPI_SetTryCatchCallbacks", _SteamAPI_SetTryCatchCallbacksDelegate, new SteamAPI_SetTryCatchCallbacksDelegate(SteamAPI_SetTryCatchCallbacks));
            base.Install<SteamAPI_SetBreakpadAppIDDelegate>("SteamAPI_SetBreakpadAppID", _SteamAPI_SetBreakpadAppIDDelegate, new SteamAPI_SetBreakpadAppIDDelegate(SteamAPI_SetBreakpadAppID));
            base.Install<SteamAPI_UseBreakpadCrashHandlerDelegate>("SteamAPI_UseBreakpadCrashHandler", _SteamAPI_UseBreakpadCrashHandlerDelegate, new SteamAPI_UseBreakpadCrashHandlerDelegate(SteamAPI_UseBreakpadCrashHandler));
            base.Install<SteamAPI_ManualDispatch_RunFrameDelegate>("SteamAPI_ManualDispatch_RunFrame", _SteamAPI_ManualDispatch_RunFrameDelegate, new SteamAPI_ManualDispatch_RunFrameDelegate(SteamAPI_ManualDispatch_RunFrame));
            base.Install<SteamAPI_ManualDispatch_GetNextCallbackDelegate>("SteamAPI_ManualDispatch_GetNextCallback", _SteamAPI_ManualDispatch_GetNextCallbackDelegate, new SteamAPI_ManualDispatch_GetNextCallbackDelegate(SteamAPI_ManualDispatch_GetNextCallback));
            base.Install<SteamAPI_ManualDispatch_FreeLastCallbackDelegate>("SteamAPI_ManualDispatch_FreeLastCallback", _SteamAPI_ManualDispatch_FreeLastCallbackDelegate, new SteamAPI_ManualDispatch_FreeLastCallbackDelegate(SteamAPI_ManualDispatch_FreeLastCallback));
            base.Install<SteamAPI_ManualDispatch_GetAPICallResultDelegate>("SteamAPI_ManualDispatch_GetAPICallResult", _SteamAPI_ManualDispatch_GetAPICallResultDelegate, new SteamAPI_ManualDispatch_GetAPICallResultDelegate(SteamAPI_ManualDispatch_GetAPICallResult));
            base.Install<SteamAPI_RestartAppDelegate>("SteamAPI_RestartApp", _SteamAPI_RestartAppDelegate, new SteamAPI_RestartAppDelegate(SteamAPI_RestartApp));
            base.Install<SteamAPI_SetMiniDumpCommentDelegate>("SteamAPI_SetMiniDumpComment", _SteamAPI_SetMiniDumpCommentDelegate, new SteamAPI_SetMiniDumpCommentDelegate(SteamAPI_SetMiniDumpComment));
            base.Install<SteamAPI_WriteMiniDumpDelegate>("SteamAPI_WriteMiniDump", _SteamAPI_WriteMiniDumpDelegate, new SteamAPI_WriteMiniDumpDelegate(SteamAPI_WriteMiniDump));
            base.Install<SteamAPI_ReleaseCurrentThreadMemoryDelegate>("SteamAPI_ReleaseCurrentThreadMemory", _SteamAPI_ReleaseCurrentThreadMemoryDelegate, new SteamAPI_ReleaseCurrentThreadMemoryDelegate(SteamAPI_ReleaseCurrentThreadMemory));
            base.Install<SteamAPI_gameserveritem_t_ConstructDelegate>("SteamAPI_gameserveritem_t_Construct", _SteamAPI_gameserveritem_t_ConstructDelegate, new SteamAPI_gameserveritem_t_ConstructDelegate(SteamAPI_gameserveritem_t_Construct));
            base.Install<SteamAPI_gameserveritem_t_GetNameDelegate>("SteamAPI_gameserveritem_t_GetName", _SteamAPI_gameserveritem_t_GetNameDelegate, new SteamAPI_gameserveritem_t_GetNameDelegate(SteamAPI_gameserveritem_t_GetName));
            base.Install<SteamAPI_gameserveritem_t_SetNameDelegate>("SteamAPI_gameserveritem_t_SetName", _SteamAPI_gameserveritem_t_SetNameDelegate, new SteamAPI_gameserveritem_t_SetNameDelegate(SteamAPI_gameserveritem_t_SetName));
            base.Install<g_pSteamClientGameServerDelegate>("g_pSteamClientGameServer", _g_pSteamClientGameServerDelegate, new g_pSteamClientGameServerDelegate(g_pSteamClientGameServer));
            base.Install<Steam_RegisterInterfaceFuncsDelegate>("Steam_RegisterInterfaceFuncs", _Steam_RegisterInterfaceFuncsDelegate, new Steam_RegisterInterfaceFuncsDelegate(Steam_RegisterInterfaceFuncs));
            base.Install<Steam_RunCallbacksDelegate>("Steam_RunCallbacks", _Steam_RunCallbacksDelegate, new Steam_RunCallbacksDelegate(Steam_RunCallbacks));
            base.Install<SteamAPI_SteamAppList_v001Delegate>("SteamAPI_SteamAppList_v001", _SteamAPI_SteamAppList_v001Delegate, new SteamAPI_SteamAppList_v001Delegate(SteamAPI_SteamAppList_v001));
            base.Install<SteamAPI_SteamApps_v008Delegate>("SteamAPI_SteamApps_v008", _SteamAPI_SteamApps_v008Delegate, new SteamAPI_SteamApps_v008Delegate(SteamAPI_SteamApps_v008));
            base.Install<SteamAPI_SteamGameServerApps_v008Delegate>("SteamAPI_SteamGameServerApps_v008", _SteamAPI_SteamGameServerApps_v008Delegate, new SteamAPI_SteamGameServerApps_v008Delegate(SteamAPI_SteamGameServerApps_v008));
            base.Install<SteamAPI_SteamController_v007Delegate>("SteamAPI_SteamController_v007", _SteamAPI_SteamController_v007Delegate, new SteamAPI_SteamController_v007Delegate(SteamAPI_SteamController_v007));
            base.Install<SteamAPI_SteamController_v008Delegate>("SteamAPI_SteamController_v008", _SteamAPI_SteamController_v008Delegate, new SteamAPI_SteamController_v008Delegate(SteamAPI_SteamController_v008));
            base.Install<SteamAPI_SteamFriends_v017Delegate>("SteamAPI_SteamFriends_v017", _SteamAPI_SteamFriends_v017Delegate, new SteamAPI_SteamFriends_v017Delegate(SteamAPI_SteamFriends_v017));
            base.Install<SteamAPI_SteamUtils_v010Delegate>("SteamAPI_SteamUtils_v010", _SteamAPI_SteamUtils_v010Delegate, new SteamAPI_SteamUtils_v010Delegate(SteamAPI_SteamUtils_v010));
            base.Install<SteamAPI_SteamGameServerUtils_v010Delegate>("SteamAPI_SteamGameServerUtils_v010", _SteamAPI_SteamGameServerUtils_v010Delegate, new SteamAPI_SteamGameServerUtils_v010Delegate(SteamAPI_SteamGameServerUtils_v010));
            base.Install<SteamAPI_SteamUtils_v009Delegate>("SteamAPI_SteamUtils_v009", _SteamAPI_SteamUtils_v009Delegate, new SteamAPI_SteamUtils_v009Delegate(SteamAPI_SteamUtils_v009));
            base.Install<SteamAPI_SteamGameServerUtils_v009Delegate>("SteamAPI_SteamGameServerUtils_v009", _SteamAPI_SteamGameServerUtils_v009Delegate, new SteamAPI_SteamGameServerUtils_v009Delegate(SteamAPI_SteamGameServerUtils_v009));
            base.Install<SteamAPI_SteamMatchmaking_v009Delegate>("SteamAPI_SteamMatchmaking_v009", _SteamAPI_SteamMatchmaking_v009Delegate, new SteamAPI_SteamMatchmaking_v009Delegate(SteamAPI_SteamMatchmaking_v009));
            base.Install<SteamAPI_SteamMatchmakingServers_v002Delegate>("SteamAPI_SteamMatchmakingServers_v002", _SteamAPI_SteamMatchmakingServers_v002Delegate, new SteamAPI_SteamMatchmakingServers_v002Delegate(SteamAPI_SteamMatchmakingServers_v002));
            base.Install<SteamAPI_SteamGameSearch_v001Delegate>("SteamAPI_SteamGameSearch_v001", _SteamAPI_SteamGameSearch_v001Delegate, new SteamAPI_SteamGameSearch_v001Delegate(SteamAPI_SteamGameSearch_v001));
            base.Install<SteamAPI_SteamParties_v002Delegate>("SteamAPI_SteamParties_v002", _SteamAPI_SteamParties_v002Delegate, new SteamAPI_SteamParties_v002Delegate(SteamAPI_SteamParties_v002));
            base.Install<SteamAPI_SteamNetworking_v006Delegate>("SteamAPI_SteamNetworking_v006", _SteamAPI_SteamNetworking_v006Delegate, new SteamAPI_SteamNetworking_v006Delegate(SteamAPI_SteamNetworking_v006));
            base.Install<SteamAPI_SteamGameServerNetworking_v006Delegate>("SteamAPI_SteamGameServerNetworking_v006", _SteamAPI_SteamGameServerNetworking_v006Delegate, new SteamAPI_SteamGameServerNetworking_v006Delegate(SteamAPI_SteamGameServerNetworking_v006));
            base.Install<SteamAPI_SteamScreenshots_v003Delegate>("SteamAPI_SteamScreenshots_v003", _SteamAPI_SteamScreenshots_v003Delegate, new SteamAPI_SteamScreenshots_v003Delegate(SteamAPI_SteamScreenshots_v003));
            base.Install<SteamAPI_SteamMusic_v001Delegate>("SteamAPI_SteamMusic_v001", _SteamAPI_SteamMusic_v001Delegate, new SteamAPI_SteamMusic_v001Delegate(SteamAPI_SteamMusic_v001));
            base.Install<SteamAPI_SteamMusicRemote_v001Delegate>("SteamAPI_SteamMusicRemote_v001", _SteamAPI_SteamMusicRemote_v001Delegate, new SteamAPI_SteamMusicRemote_v001Delegate(SteamAPI_SteamMusicRemote_v001));
            base.Install<SteamAPI_SteamHTTP_v003Delegate>("SteamAPI_SteamHTTP_v003", _SteamAPI_SteamHTTP_v003Delegate, new SteamAPI_SteamHTTP_v003Delegate(SteamAPI_SteamHTTP_v003));
            base.Install<SteamAPI_SteamGameServerHTTP_v003Delegate>("SteamAPI_SteamGameServerHTTP_v003", _SteamAPI_SteamGameServerHTTP_v003Delegate, new SteamAPI_SteamGameServerHTTP_v003Delegate(SteamAPI_SteamGameServerHTTP_v003));
            base.Install<SteamAPI_SteamInput_v001Delegate>("SteamAPI_SteamInput_v001", _SteamAPI_SteamInput_v001Delegate, new SteamAPI_SteamInput_v001Delegate(SteamAPI_SteamInput_v001));
            base.Install<SteamAPI_SteamInput_v002Delegate>("SteamAPI_SteamInput_v002", _SteamAPI_SteamInput_v002Delegate, new SteamAPI_SteamInput_v002Delegate(SteamAPI_SteamInput_v002));
            base.Install<SteamAPI_SteamHTMLSurface_v005Delegate>("SteamAPI_SteamHTMLSurface_v005", _SteamAPI_SteamHTMLSurface_v005Delegate, new SteamAPI_SteamHTMLSurface_v005Delegate(SteamAPI_SteamHTMLSurface_v005));
            base.Install<SteamAPI_SteamInventory_v003Delegate>("SteamAPI_SteamInventory_v003", _SteamAPI_SteamInventory_v003Delegate, new SteamAPI_SteamInventory_v003Delegate(SteamAPI_SteamInventory_v003));
            base.Install<SteamAPI_SteamGameServerInventory_v003Delegate>("SteamAPI_SteamGameServerInventory_v003", _SteamAPI_SteamGameServerInventory_v003Delegate, new SteamAPI_SteamGameServerInventory_v003Delegate(SteamAPI_SteamGameServerInventory_v003));
            base.Install<SteamAPI_SteamVideo_v002Delegate>("SteamAPI_SteamVideo_v002", _SteamAPI_SteamVideo_v002Delegate, new SteamAPI_SteamVideo_v002Delegate(SteamAPI_SteamVideo_v002));
            base.Install<SteamAPI_SteamTV_v001Delegate>("SteamAPI_SteamTV_v001", _SteamAPI_SteamTV_v001Delegate, new SteamAPI_SteamTV_v001Delegate(SteamAPI_SteamTV_v001));
            base.Install<SteamAPI_SteamParentalSettings_v001Delegate>("SteamAPI_SteamParentalSettings_v001", _SteamAPI_SteamParentalSettings_v001Delegate, new SteamAPI_SteamParentalSettings_v001Delegate(SteamAPI_SteamParentalSettings_v001));
            base.Install<SteamAPI_SteamRemotePlay_v001Delegate>("SteamAPI_SteamRemotePlay_v001", _SteamAPI_SteamRemotePlay_v001Delegate, new SteamAPI_SteamRemotePlay_v001Delegate(SteamAPI_SteamRemotePlay_v001));
            base.Install<SteamAPI_SteamNetworkingMessages_v002Delegate>("SteamAPI_SteamNetworkingMessages_v002", _SteamAPI_SteamNetworkingMessages_v002Delegate, new SteamAPI_SteamNetworkingMessages_v002Delegate(SteamAPI_SteamNetworkingMessages_v002));
            base.Install<SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate>("SteamAPI_SteamNetworkingMessages_SteamAPI_v002", _SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate, new SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate(SteamAPI_SteamNetworkingMessages_SteamAPI_v002));
            base.Install<SteamAPI_SteamGameServerNetworkingMessages_v002Delegate>("SteamAPI_SteamGameServerNetworkingMessages_v002", _SteamAPI_SteamGameServerNetworkingMessages_v002Delegate, new SteamAPI_SteamGameServerNetworkingMessages_v002Delegate(SteamAPI_SteamGameServerNetworkingMessages_v002));
            base.Install<SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate>("SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002", _SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate, new SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate(SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002));
            base.Install<SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate>("SteamAPI_SteamNetworkingSockets_SteamAPI_v009", _SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate, new SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate(SteamAPI_SteamNetworkingSockets_SteamAPI_v009));
            base.Install<SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate>("SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009", _SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate, new SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate(SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009));
            base.Install<SteamAPI_SteamNetworkingSockets_v009Delegate>("SteamAPI_SteamNetworkingSockets_v009", _SteamAPI_SteamNetworkingSockets_v009Delegate, new SteamAPI_SteamNetworkingSockets_v009Delegate(SteamAPI_SteamNetworkingSockets_v009));
            base.Install<SteamAPI_SteamGameServerNetworkingSockets_v009Delegate>("SteamAPI_SteamGameServerNetworkingSockets_v009", _SteamAPI_SteamGameServerNetworkingSockets_v009Delegate, new SteamAPI_SteamGameServerNetworkingSockets_v009Delegate(SteamAPI_SteamGameServerNetworkingSockets_v009));
            base.Install<SteamAPI_SteamNetworkingSockets_v008Delegate>("SteamAPI_SteamNetworkingSockets_v008", _SteamAPI_SteamNetworkingSockets_v008Delegate, new SteamAPI_SteamNetworkingSockets_v008Delegate(SteamAPI_SteamNetworkingSockets_v008));
            base.Install<SteamAPI_SteamGameServerNetworkingSockets_v008Delegate>("SteamAPI_SteamGameServerNetworkingSockets_v008", _SteamAPI_SteamGameServerNetworkingSockets_v008Delegate, new SteamAPI_SteamGameServerNetworkingSockets_v008Delegate(SteamAPI_SteamGameServerNetworkingSockets_v008));
            base.Install<SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate>("SteamAPI_SteamNetworkingUtils_SteamAPI_v003", _SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate, new SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate(SteamAPI_SteamNetworkingUtils_SteamAPI_v003));
            base.Install<SteamAPI_SteamNetworkingUtils_v003Delegate>("SteamAPI_SteamNetworkingUtils_v003", _SteamAPI_SteamNetworkingUtils_v003Delegate, new SteamAPI_SteamNetworkingUtils_v003Delegate(SteamAPI_SteamNetworkingUtils_v003));
            base.Install<SteamAPI_SteamGameServerStats_v001Delegate>("SteamAPI_SteamGameServerStats_v001", _SteamAPI_SteamGameServerStats_v001Delegate, new SteamAPI_SteamGameServerStats_v001Delegate(SteamAPI_SteamGameServerStats_v001));
            base.Install<SteamAppListDelegate>("SteamAppList", _SteamAppListDelegate, new SteamAppListDelegate(SteamAppList));
            base.Install<SteamAppsDelegate>("SteamApps", _SteamAppsDelegate, new SteamAppsDelegate(SteamApps));
            base.Install<SteamClientDelegate>("SteamClient", _SteamClientDelegate, new SteamClientDelegate(SteamClient));
            base.Install<SteamControllerDelegate>("SteamController", _SteamControllerDelegate, new SteamControllerDelegate(SteamController));
            base.Install<SteamFriendsDelegate>("SteamFriends", _SteamFriendsDelegate, new SteamFriendsDelegate(SteamFriends));
            base.Install<SteamGameServerDelegate>("SteamGameServer", _SteamGameServerDelegate, new SteamGameServerDelegate(SteamGameServer));
            base.Install<SteamGameServerAppsDelegate>("SteamGameServerApps", _SteamGameServerAppsDelegate, new SteamGameServerAppsDelegate(SteamGameServerApps));
            base.Install<SteamGameServerHTTPDelegate>("SteamGameServerHTTP", _SteamGameServerHTTPDelegate, new SteamGameServerHTTPDelegate(SteamGameServerHTTP));
            base.Install<SteamGameServerInventoryDelegate>("SteamGameServerInventory", _SteamGameServerInventoryDelegate, new SteamGameServerInventoryDelegate(SteamGameServerInventory));
            base.Install<SteamGameServerNetworkingDelegate>("SteamGameServerNetworking", _SteamGameServerNetworkingDelegate, new SteamGameServerNetworkingDelegate(SteamGameServerNetworking));
            base.Install<SteamGameServerStatsDelegate>("SteamGameServerStats", _SteamGameServerStatsDelegate, new SteamGameServerStatsDelegate(SteamGameServerStats));
            base.Install<SteamGameServerUGCDelegate>("SteamGameServerUGC", _SteamGameServerUGCDelegate, new SteamGameServerUGCDelegate(SteamGameServerUGC));
            base.Install<SteamGameServerUtilsDelegate>("SteamGameServerUtils", _SteamGameServerUtilsDelegate, new SteamGameServerUtilsDelegate(SteamGameServerUtils));
            base.Install<SteamHTTPDelegate>("SteamHTTP", _SteamHTTPDelegate, new SteamHTTPDelegate(SteamHTTP));
            base.Install<SteamHTMLSurfaceDelegate>("SteamHTMLSurface", _SteamHTMLSurfaceDelegate, new SteamHTMLSurfaceDelegate(SteamHTMLSurface));
            base.Install<SteamInventoryDelegate>("SteamInventory", _SteamInventoryDelegate, new SteamInventoryDelegate(SteamInventory));
            base.Install<SteamMasterServerUpdaterDelegate>("SteamMasterServerUpdater", _SteamMasterServerUpdaterDelegate, new SteamMasterServerUpdaterDelegate(SteamMasterServerUpdater));
            base.Install<SteamMatchmakingDelegate>("SteamMatchmaking", _SteamMatchmakingDelegate, new SteamMatchmakingDelegate(SteamMatchmaking));
            base.Install<SteamMatchmakingServersDelegate>("SteamMatchmakingServers", _SteamMatchmakingServersDelegate, new SteamMatchmakingServersDelegate(SteamMatchmakingServers));
            base.Install<SteamMusicDelegate>("SteamMusic", _SteamMusicDelegate, new SteamMusicDelegate(SteamMusic));
            base.Install<SteamMusicRemoteDelegate>("SteamMusicRemote", _SteamMusicRemoteDelegate, new SteamMusicRemoteDelegate(SteamMusicRemote));
            base.Install<SteamNetworkingDelegate>("SteamNetworking", _SteamNetworkingDelegate, new SteamNetworkingDelegate(SteamNetworking));
            base.Install<SteamParentalSettingsDelegate>("SteamParentalSettings", _SteamParentalSettingsDelegate, new SteamParentalSettingsDelegate(SteamParentalSettings));
            base.Install<SteamRemoteStorageDelegate>("SteamRemoteStorage", _SteamRemoteStorageDelegate, new SteamRemoteStorageDelegate(SteamRemoteStorage));
            base.Install<SteamScreenshotsDelegate>("SteamScreenshots", _SteamScreenshotsDelegate, new SteamScreenshotsDelegate(SteamScreenshots));
            base.Install<SteamUGCDelegate>("SteamUGC", _SteamUGCDelegate, new SteamUGCDelegate(SteamUGC));
            base.Install<SteamUserDelegate>("SteamUser", _SteamUserDelegate, new SteamUserDelegate(SteamUser));
            base.Install<SteamUserStatsDelegate>("SteamUserStats", _SteamUserStatsDelegate, new SteamUserStatsDelegate(SteamUserStats));
            base.Install<SteamUtilsDelegate>("SteamUtils", _SteamUtilsDelegate, new SteamUtilsDelegate(SteamUtils));
            base.Install<SteamVideoDelegate>("SteamVideo", _SteamVideoDelegate, new SteamVideoDelegate(SteamVideo));
            base.Install<VR_GetGenericInterfaceDelegate>("VR_GetGenericInterface", _VR_GetGenericInterfaceDelegate, new VR_GetGenericInterfaceDelegate(VR_GetGenericInterface));
            base.Install<VR_GetStringForHmdErrorDelegate>("VR_GetStringForHmdError", _VR_GetStringForHmdErrorDelegate, new VR_GetStringForHmdErrorDelegate(VR_GetStringForHmdError));
            base.Install<VR_InitDelegate>("VR_Init", _VR_InitDelegate, new VR_InitDelegate(VR_Init));
            base.Install<VR_IsHmdPresentDelegate>("VR_IsHmdPresent", _VR_IsHmdPresentDelegate, new VR_IsHmdPresentDelegate(VR_IsHmdPresent));
            base.Install<VR_ShutdownDelegate>("VR_Shutdown", _VR_ShutdownDelegate, new VR_ShutdownDelegate(VR_Shutdown));
        }
        int steps = 0;
        public bool SteamAPI_Init()
        {
            if (SteamEmulator.Initialized && steps == 2)
            {
                Write($"{"SteamAPI_Init [TRUE]"}");
                return true;
            }
            if (!SteamEmulator.Initialized)
            {
                new SteamEmulator(false).Initialize();
            }
            steps++;
            Write($"{"SteamAPI_Init [FALSE]"}");

            return false;
        }

        public void SteamAPI_RunCallbacks()
        {
            Write("SteamAPI_RunCallbacks");
            SteamEmulator.Client_Callback.RunCallbacks();
        }

        
        public void SteamAPI_RegisterCallResult(CCallbackBase pCallback, SteamAPICall_t hAPICall)
        {
            Write("SteamAPI_RegisterCallResult");
            SteamEmulator.Client_Callback.RegisterCallResult(pCallback, hAPICall);
        }

        
        public void SteamAPI_Shutdown(IntPtr pContextInitData)
        {
            Write("SteamAPI_Shutdown");
        }

        
        public void SteamAPI_UnregisterCallback(IntPtr pCallback)
        {
            Write($"SteamAPI_UnregisterCallback {pCallback}");
            SteamEmulator.Client_Callback.UnregisterCallback(pCallback);
        }

        
        public void SteamAPI_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            Write("SteamAPI_UnregisterCallResult\n");
            SteamEmulator.Client_Callback.UnregisterCallResult(pCallback, hAPICall);
        }

        
        public unsafe void SteamAPI_RegisterCallback(IntPtr pCallback, int iCallback)
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
                SteamEmulator.Server_Callback.RegisterCallback(pCallback, iCallback);
            }
            else
            {
                SteamEmulator.Client_Callback.RegisterCallback(pCallback, iCallback);
            }

            Write(callMessage);
        }



        
        public bool SteamAPI_InitSafe()
        {
            Write("SteamAPI_InitSafe");
            return SteamAPI_Init();
        }

        
        public bool SteamAPI_InitAnonymousUser()
        {
            Write("SteamAPI_InitAnonymousUser");
            SteamAPI_Init();
            return true;
        }

        
        public bool SteamAPI_IsSteamRunning()
        {
            Write($"{"SteamAPI_IsSteamRunning"}");
            return true;
        }

        
        public bool SteamAPI_RestartAppIfNecessary(uint appId)
        {
            Write($"SteamAPI_RestartAppIfNecessary called {appId}");
            SteamEmulator.AppId = appId;
            return false;
        }

        
        public string SteamAPI_GetSteamInstallPath()
        {
            Write($"{"SteamAPI_GetSteamInstallPath"}");

            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            return path;
        }

        
        public int SteamAPI_GetHSteamUser()
        {
            Write("SteamAPI_GetHSteamUser");
            if (SteamEmulator.HSteamUser == null || (int)SteamEmulator.HSteamUser == 0)
            {
                SteamEmulator.CreateSteamUser();
            }
            return (int)SteamEmulator.HSteamUser;
        }

        
        public int SteamAPI_GetHSteamPipe()
        {
            Write("SteamAPI_GetHSteamPipe");
            if (SteamEmulator.HSteamPipe == null || (int)SteamEmulator.HSteamPipe == 0)
            {
                SteamEmulator.CreateSteamPipe();
            }
            return (int)SteamEmulator.HSteamPipe;
        }

        
        public int GetHSteamPipe()
        {
            Write("GetHSteamPipe");
            return SteamAPI_GetHSteamPipe();
        }

        
        public int GetHSteamUser()
        {
            Write("GetHSteamUser");
            return SteamAPI_GetHSteamUser();
        }

        
        public void SteamAPI_SetTryCatchCallbacks(bool bTryCatchCallbacks)
        {
            Write($"SteamAPI_SetTryCatchCallbacks");
        }

        
        public void SteamAPI_SetBreakpadAppID(UInt32 unAppID)
        {
            Write($"SteamAPI_SetBreakpadAppID {unAppID}");
        }

        
        public void SteamAPI_UseBreakpadCrashHandler([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback)
        {
            Write($"SteamAPI_UseBreakpadCrashHandler | Date: {pchDate} | Time: {pchTime} | Version : {pchVersion}");
        }

        
        public void SteamAPI_ManualDispatch_RunFrame(IntPtr hSteamPipe)
        {
            Write($"SteamAPI_ManualDispatch_RunFrame");
        }

        
        public bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe hSteamPipe, IntPtr pCallbackMsg)
        {
            Write($"SteamAPI_ManualDispatch_GetNextCallback");
            return true;
        }

        
        public void SteamAPI_ManualDispatch_FreeLastCallback(HSteamPipe hSteamPipe)
        {
            Write($"SteamAPI_ManualDispatch_FreeLastCallback");
        }


        
        public bool SteamAPI_ManualDispatch_GetAPICallResult(HSteamPipe hSteamPipe, IntPtr hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
        {
            Write($"SteamAPI_ManualDispatch_GetAPICallResult");
            return true;
        }


        
        public bool SteamAPI_RestartApp(UInt32 appid)
        {
            Write($"SteamAPI_RestartApp");
            return SteamAPI_RestartAppIfNecessary(appid);
        }

        
        public void SteamAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] string pchMsg)
        {
            string Msg = "SteamAPI_SetMiniDumpComment" + Environment.NewLine;
            Msg += "////////////////////////////// Mini Dump Content \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
            Msg += $"{pchMsg}" + Environment.NewLine;
            Msg += "//////////////////////////////   End Mini Dump   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
            Write(Msg);
            //Log.Write(Msg, true);
        }

        
        public void SteamAPI_WriteMiniDump(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID)
        {
            Write($"SteamAPI_WriteMiniDump");
        }

        
        public void SteamAPI_ReleaseCurrentThreadMemory()
        {
            Write($"SteamAPI_ReleaseCurrentThreadMemory");
        }

        
        public void SteamAPI_gameserveritem_t_Construct(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_Construct");
        }

        
        public string SteamAPI_gameserveritem_t_GetName(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_GetName");
            return "";
        }

        
        public void SteamAPI_gameserveritem_t_SetName(IntPtr self, IntPtr pName)
        {
            Write($"SteamAPI_gameserveritem_t_SetName");
        }

        
        public IntPtr g_pSteamClientGameServer()
        {
            Write($"g_pSteamClientGameServer");
            return SteamEmulator.SteamGameServer.BaseAddress;
        }

        
        public void Steam_RegisterInterfaceFuncs(IntPtr hModule)
        {
            Write($"Steam_RegisterInterfaceFuncs");
        }

        
        public void Steam_RunCallbacks(IntPtr hSteamPipe, bool bGameServerCallbacks)
        {
            Write("Steam_RunCallbacks\n");

            SteamAPI_RunCallbacks();

            if (bGameServerCallbacks)
                SteamEmulator.SteamGameServer.RunCallbacks(IntPtr.Zero);
        }




        #region Interfaces

        
        public IntPtr SteamAPI_SteamAppList_v001()
        {
            Write($"SteamAPI_SteamAppList_v001");
            return SteamEmulator.SteamClient.GetISteamAppList(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMAPPLIST_INTERFACE_VERSION001");
        }

        
        public IntPtr SteamAPI_SteamApps_v008()
        {
            Write($"SteamAPI_SteamApps_v008");
            return SteamEmulator.SteamClient.GetISteamApps(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMAPPS_INTERFACE_VERSION008");
        }

        
        public IntPtr SteamAPI_SteamGameServerApps_v008()
        {
            Write($"SteamAPI_SteamGameServerApps_v008");
            return SteamEmulator.SteamClient.GetISteamApps((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "STEAMAPPS_INTERFACE_VERSION008");
        }

        
        public IntPtr SteamAPI_SteamController_v007()
        {
            Write($"SteamAPI_SteamController_v007");
            return SteamEmulator.SteamClient.GetISteamController(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamController007");
        }

        
        public IntPtr SteamAPI_SteamController_v008()
        {
            Write($"SteamAPI_SteamController_v008");
            return SteamEmulator.SteamClient.GetISteamController(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamController008");
        }
        
        public IntPtr SteamAPI_SteamFriends_v017()
        {
            Write($"SteamAPI_SteamFriends_v017");
            return SteamEmulator.SteamClient.GetISteamFriends(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamFriends017");
        }
        
        public IntPtr SteamAPI_SteamUtils_v010()
        {
            Write($"SteamAPI_SteamUtils_v010");
            return SteamEmulator.SteamClient.GetISteamUtils(SteamAPI_GetHSteamPipe(), "SteamUtils010");
        }

        
        public IntPtr SteamAPI_SteamGameServerUtils_v010()
        {
            Write($"SteamAPI_SteamGameServerUtils_v010");
            return SteamEmulator.SteamClient.GetISteamUtils((int)SteamEmulator.HSteamPipe_GS, "SteamUtils010");
        }

        
        public IntPtr SteamAPI_SteamUtils_v009()
        {
            Write($"SteamAPI_SteamUtils_v009");
            return SteamEmulator.SteamClient.GetISteamUtils(SteamAPI_GetHSteamPipe(), "SteamUtils009");
        }

        
        public IntPtr SteamAPI_SteamGameServerUtils_v009()
        {
            Write($"SteamAPI_SteamGameServerUtils_v009");
            return SteamEmulator.SteamClient.GetISteamUtils((int)SteamEmulator.HSteamPipe_GS, "SteamUtils009");
        }

        
        public IntPtr SteamAPI_SteamMatchmaking_v009()
        {
            Write($"SteamAPI_SteamMatchmaking_v009");
            return SteamEmulator.SteamClient.GetISteamMatchmaking(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamMatchMaking009");
        }

        
        public IntPtr SteamAPI_SteamMatchmakingServers_v002()
        {
            Write($"SteamAPI_SteamMatchmakingServers_v002");
            return SteamEmulator.SteamClient.GetISteamMatchmakingServers(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamMatchMakingServers002");
        }

        
        public IntPtr SteamAPI_SteamGameSearch_v001()
        {
            Write($"SteamAPI_SteamGameSearch_v001");
            return InterfaceManager.FindOrCreateInterface(1, 1, "SteamAPI_SteamGameSearch_v001");
        }


        
        public IntPtr SteamAPI_SteamParties_v002()
        {
            Write($"SteamAPI_SteamParties_v002");
            return SteamEmulator.SteamClient.GetISteamParties(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamParties002");
        }

        
        public IntPtr SteamAPI_SteamNetworking_v006()
        {
            Write($"SteamAPI_SteamNetworking_v006");
            return SteamEmulator.SteamClient.GetISteamNetworking(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworking006");
        }

        
        public IntPtr SteamAPI_SteamGameServerNetworking_v006()
        {
            Write($"SteamAPI_SteamGameServerNetworking_v006");
            return SteamEmulator.SteamClient.GetISteamNetworking((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworking006");
        }

        
        public IntPtr SteamAPI_SteamScreenshots_v003()
        {
            Write($"SteamAPI_SteamScreenshots_v003");
            return SteamEmulator.SteamScreenshots.BaseAddress;
        }

        
        public IntPtr SteamAPI_SteamMusic_v001()
        {
            Write($"SteamAPI_SteamMusic_v001");
            return SteamEmulator.SteamClient.GetISteamMusic(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMMUSIC_INTERFACE_VERSION001");
        }

        
        public IntPtr SteamAPI_SteamMusicRemote_v001()
        {
            Write($"SteamAPI_SteamMusicRemote_v001");
            return SteamEmulator.SteamClient.GetISteamMusicRemote(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMMUSICREMOTE_INTERFACE_VERSION001");
        }

        
        public IntPtr SteamAPI_SteamHTTP_v003()
        {
            Write($"SteamAPI_SteamHTTP_v003");
            return SteamEmulator.SteamHTTP.BaseAddress;
        }

        
        public IntPtr SteamAPI_SteamGameServerHTTP_v003()
        {
            Write($"SteamAPI_SteamGameServerHTTP_v003");
            return SteamEmulator.SteamHTTP.BaseAddress;
        }

        
        public IntPtr SteamAPI_SteamInput_v001()
        {
            Write($"SteamAPI_SteamInput_v001");
            return SteamEmulator.SteamClient.GetISteamInput(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamInput001");
        }

        
        public IntPtr SteamAPI_SteamInput_v002()
        {
            Write($"SteamAPI_SteamInput_v002");
            return SteamEmulator.SteamClient.GetISteamInput(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamInput002");
        }

        
        public IntPtr SteamAPI_SteamHTMLSurface_v005()
        {
            Write($"SteamAPI_SteamHTMLSurface_v005");
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMHTMLSURFACE_INTERFACE_VERSION_005");
        }

        
        public IntPtr SteamAPI_SteamInventory_v003()
        {
            Write($"SteamAPI_SteamInventory_v003");
            return SteamEmulator.SteamClient.GetISteamInventory(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMINVENTORY_INTERFACE_V003");
        }

        
        public IntPtr SteamAPI_SteamGameServerInventory_v003()
        {
            Write($"SteamAPI_SteamGameServerInventory_v003");
            return SteamEmulator.SteamClient.GetISteamInventory((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "STEAMINVENTORY_INTERFACE_V003");
        }

        
        public IntPtr SteamAPI_SteamVideo_v002()
        {
            Write($"SteamAPI_SteamVideo_v002");
            return SteamEmulator.SteamClient.GetISteamVideo(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMVIDEO_INTERFACE_V002");
        }

        
        public IntPtr SteamAPI_SteamTV_v001()
        {
            Write($"SteamAPI_SteamTV_v001");
            return SteamEmulator.SteamTV.BaseAddress;
        }

        
        public IntPtr SteamAPI_SteamParentalSettings_v001()
        {
            Write($"SteamAPI_SteamParentalSettings_v001");
            return SteamEmulator.SteamClient.GetISteamParentalSettings(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMPARENTALSETTINGS_INTERFACE_VERSION001");
        }

        
        public IntPtr SteamAPI_SteamRemotePlay_v001()
        {
            Write($"SteamAPI_SteamRemotePlay_v001");
            return SteamEmulator.SteamClient.GetISteamRemotePlay(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "STEAMREMOTEPLAY_INTERFACE_VERSION001");
        }

        
        public IntPtr SteamAPI_SteamNetworkingMessages_v002()
        {
            Write($"SteamAPI_SteamNetworkingMessages_v002");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingMessages002");
        }

        
        public IntPtr SteamAPI_SteamNetworkingMessages_SteamAPI_v002()
        {
            Write($"SteamAPI_SteamNetworkingMessages_SteamAPI_v002");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingMessages002");
        }

        
        public IntPtr SteamAPI_SteamGameServerNetworkingMessages_v002()
        {
            Write($"SteamAPI_SteamGameServerNetworkingMessages_v002");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworkingMessages002");
        }

        
        public IntPtr SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002()
        {
            Write($"SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworkingMessages002");
        }

        
        public IntPtr SteamAPI_SteamNetworkingSockets_SteamAPI_v009()
        {
            Write($"SteamAPI_SteamNetworkingSockets_SteamAPI_v009");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets009");
        }

        
        public IntPtr SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworkingSockets009");
        }

        
        public IntPtr SteamAPI_SteamNetworkingSockets_v009()
        {
            Write($"SteamAPI_SteamNetworkingSockets_v009");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets009");
        }

        public IntPtr SteamAPI_SteamGameServerNetworkingSockets_v009()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_v009");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworkingSockets009");
        }
        
        public IntPtr SteamAPI_SteamNetworkingSockets_v008()
        {
            Write($"SteamAPI_SteamNetworkingSockets_v008");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingSockets008");
        }

        public IntPtr SteamAPI_SteamGameServerNetworkingSockets_v008()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_v008");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser_GS, (int)SteamEmulator.HSteamPipe_GS, "SteamNetworkingSockets008");
        }

        public IntPtr SteamAPI_SteamNetworkingUtils_SteamAPI_v003()
        {
            Write($"SteamAPI_SteamNetworkingUtils_SteamAPI_v003");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingUtils003");
        }

        public IntPtr SteamAPI_SteamNetworkingUtils_v003()
        {
            Write($"SteamAPI_SteamNetworkingUtils_v003");
            return SteamEmulator.SteamClient.GetISteamGenericInterface(SteamAPI_GetHSteamUser(), SteamAPI_GetHSteamPipe(), "SteamNetworkingUtils003");
        }

        public IntPtr SteamAPI_SteamGameServerStats_v001()
        {
            Write($"SteamAPI_SteamGameServerStats_v001");
            return default;
        }
        
        public IntPtr SteamAppList()
        {
            Write($"SteamAppList");
            return SteamEmulator.SteamAppList.BaseAddress;
        }

        public IntPtr SteamApps()
        {
            Write($"ISteamApps");
            return SteamEmulator.SteamApps.BaseAddress;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr SteamClient()
        {
            Write($"SteamClient");
            return SteamEmulator.SteamClient.BaseAddress;
        }

        public IntPtr SteamController()
        {
            Write($"SteamController");
            return SteamEmulator.SteamController.BaseAddress;
        }

        
        public IntPtr SteamFriends()
        {
            Write($"SteamFriends");
            return SteamEmulator.SteamFriends.BaseAddress;
        }

        public IntPtr SteamGameServer()
        {
            Write($"SteamGameServer");
            return SteamEmulator.SteamGameServer.BaseAddress;
        }
        
        public IntPtr SteamGameServerApps()
        {
            Write($"SteamGameServerApps");
            return SteamEmulator.SteamGameServerApps.BaseAddress;
        }
        
        public IntPtr SteamGameServerHTTP()
        {
            Write($"SteamGameServerHTTP");
            return SteamEmulator.SteamHTTP.BaseAddress;
        }

        public IntPtr SteamGameServerInventory()
        {
            Write($"SteamGameServerInventory");
            return SteamEmulator.SteamGameServerInventory.BaseAddress;
        }
        
        public IntPtr SteamGameServerNetworking()
        {
            Write($"SteamGameServerNetworking");
            return SteamEmulator.SteamGameServerNetworking.BaseAddress;
        }

        public IntPtr SteamGameServerStats()
        {
            Write($"SteamGameServerStats");
            return SteamEmulator.SteamGameServerStats.BaseAddress;
        }

        public IntPtr SteamGameServerUGC()
        {
            Write($"SteamGameServerUGC");
            return SteamEmulator.SteamUGC.BaseAddress;
        }
        
        public IntPtr SteamGameServerUtils()
        {
            Write($"SteamGameServerUtils");
            return SteamEmulator.SteamGameServerUtils.BaseAddress;
        }

        public IntPtr SteamHTTP()
        {
            Write($"SteamHTTP");
            return SteamEmulator.SteamHTTP.BaseAddress;
        }
        
        public IntPtr SteamHTMLSurface()
        {
            Write($"SteamHTMLSurface");
            return SteamEmulator.SteamHTMLSurface.BaseAddress;
        }

        public IntPtr SteamInventory()
        {
            Write($"SteamInventory");
            return SteamEmulator.SteamInventory.BaseAddress;
        }

        public IntPtr SteamMasterServerUpdater()
        {
            Write($"SteamMasterServerUpdater");
            return SteamEmulator.SteamMasterServerUpdater.BaseAddress;
        }

        public IntPtr SteamMatchmaking()
        {
            Write($"SteamMatchmaking");
            return SteamEmulator.SteamMatchmaking.BaseAddress;
        }
        
        public IntPtr SteamMatchmakingServers()
        {
            Write($"SteamMatchmakingServers");
            return SteamEmulator.SteamMatchMakingServers.BaseAddress;
        }

        public IntPtr SteamMusic()
        {
            Write($"SteamMusic");
            return SteamEmulator.SteamMusic.BaseAddress;
        }
        
        public IntPtr SteamMusicRemote()
        {
            Write($"SteamMusicRemote");
            return SteamEmulator.SteamMusicRemote.BaseAddress;
        }

        
        public IntPtr SteamNetworking()
        {
            Write($"SteamNetworking");
            return SteamEmulator.SteamNetworking.BaseAddress;
        }

        
        public IntPtr SteamParentalSettings()
        {
            Write($"SteamParentalSettings");
            return SteamEmulator.SteamParentalSettings.BaseAddress;
        }

        
        public IntPtr SteamRemoteStorage()
        {
            Write($"SteamRemoteStorage");
            return SteamEmulator.SteamRemoteStorage.BaseAddress;
        }

        
        public IntPtr SteamScreenshots()
        {
            Write($"SteamScreenshots");
            return SteamEmulator.SteamScreenshots.BaseAddress;
        }

        
        public IntPtr SteamUGC()
        {
            Write($"SteamUGC");
            return SteamEmulator.SteamUGC.BaseAddress;
        }
        
        public IntPtr SteamUser()
        {
            Write($"SteamUser");
            return SteamEmulator.SteamUser.BaseAddress;
        }

        
        public IntPtr SteamUserStats()
        {
            Write($"SteamUserStats");
            return SteamEmulator.SteamUserStats.BaseAddress;
        }

        
        public IntPtr SteamUtils()
        {
            Write($"SteamUtils");
            return SteamEmulator.SteamUtils.BaseAddress;
        }

        
        public IntPtr SteamVideo()
        {
            Write($"SteamVideo");
            return SteamEmulator.SteamVideo.BaseAddress;
        }

        #endregion

        public void VR_GetGenericInterface(string pchInterfaceVersion, int peError)
        {
            Write($"VR_GetGenericInterface version {pchInterfaceVersion}");
        }

        
        public string VR_GetStringForHmdError(int error)
        {
            Write($"VR_GetStringForHmdError");
            return "";
        }

        
        public void VR_Init(int error, int type)
        {
            Write($"VR_Init");
        }

        
        public bool VR_IsHmdPresent()
        {
            Write($"VR_IsHmdPresent");
            return false;
        }

        
        public void VR_Shutdown()
        {
            Write($"VR_Shutdown");
        }

        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }
}
