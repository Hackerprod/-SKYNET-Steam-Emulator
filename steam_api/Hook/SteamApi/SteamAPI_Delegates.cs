using EasyHook;
using SKYNET.Interface;
using SKYNET.Managers;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Hook
{
    public partial class SteamAPI
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_InitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_RunCallbacksDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_RegisterCallResultDelegate(CCallbackBase pCallback, SteamAPICall_t hAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_ShutdownDelegate(IntPtr pContextInitData);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_UnregisterCallbackDelegate(IntPtr pCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_UnregisterCallResultDelegate(IntPtr pCallback, SteamAPICall_t hAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_RegisterCallbackDelegate(IntPtr pCallback, int iCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_InitSafeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_InitAnonymousUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_IsSteamRunningDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_RestartAppIfNecessaryDelegate(uint appId);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_GetSteamInstallPathDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate HSteamUser SteamAPI_GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate HSteamPipe SteamAPI_GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate HSteamPipe GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate HSteamUser GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_SetTryCatchCallbacksDelegate(bool bTryCatchCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_SetBreakpadAppIDDelegate(UInt32 unAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_UseBreakpadCrashHandlerDelegate([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_ManualDispatch_RunFrameDelegate(IntPtr hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_ManualDispatch_GetNextCallbackDelegate(HSteamPipe hSteamPipe, IntPtr pCallbackMsg);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_ManualDispatch_FreeLastCallbackDelegate(HSteamPipe hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_ManualDispatch_GetAPICallResultDelegate(HSteamPipe hSteamPipe, IntPtr hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamAPI_RestartAppDelegate(UInt32 appid);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_SetMiniDumpCommentDelegate([MarshalAs(UnmanagedType.LPStr)] string pchMsg);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_WriteMiniDumpDelegate(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_ReleaseCurrentThreadMemoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_gameserveritem_t_ConstructDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate string SteamAPI_gameserveritem_t_GetNameDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void SteamAPI_gameserveritem_t_SetNameDelegate(IntPtr self, IntPtr pName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr g_pSteamClientGameServerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void Steam_RegisterInterfaceFuncsDelegate(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void Steam_RunCallbacksDelegate(IntPtr hSteamPipe, bool bGameServerCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamAppList SteamAPI_SteamAppList_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamApps SteamAPI_SteamApps_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamApps SteamAPI_SteamGameServerApps_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamController SteamAPI_SteamController_v007Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamController SteamAPI_SteamController_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamFriends SteamAPI_SteamFriends_v017Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamAPI_SteamUtils_v010Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamAPI_SteamGameServerUtils_v010Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamAPI_SteamUtils_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamAPI_SteamGameServerUtils_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMatchmaking SteamAPI_SteamMatchmaking_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMatchmakingServers SteamAPI_SteamMatchmakingServers_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamGameSearch SteamAPI_SteamGameSearch_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamParties SteamAPI_SteamParties_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamNetworking SteamAPI_SteamNetworking_v006Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamNetworking SteamAPI_SteamGameServerNetworking_v006Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamScreenshots SteamAPI_SteamScreenshots_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMusic SteamAPI_SteamMusic_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMusicRemote SteamAPI_SteamMusicRemote_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTTP SteamAPI_SteamHTTP_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTTP SteamAPI_SteamGameServerHTTP_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInput SteamAPI_SteamInput_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInput SteamAPI_SteamInput_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTMLSurface SteamAPI_SteamHTMLSurface_v005Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInventory SteamAPI_SteamInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInventory SteamAPI_SteamGameServerInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamVideo SteamAPI_SteamVideo_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamTV SteamAPI_SteamTV_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamParentalSettings SteamAPI_SteamParentalSettings_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamRemotePlay SteamAPI_SteamRemotePlay_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingMessages_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamGameServerNetworkingMessages_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingSockets_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingSockets_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamAPI_SteamNetworkingUtils_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamGameServerStats SteamAPI_SteamGameServerStats_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamAppList SteamAppListDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamApps SteamAppsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate SteamClient steamClientDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamController SteamControllerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamFriends SteamFriendsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamGameServer SteamGameServerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamApps SteamGameServerAppsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTTP SteamGameServerHTTPDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInventory SteamGameServerInventoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamNetworking SteamGameServerNetworkingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamGameServerStats SteamGameServerStatsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUGC SteamGameServerUGCDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamGameServerUtilsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTTP SteamHTTPDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamHTMLSurface SteamHTMLSurfaceDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamInventory SteamInventoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMasterServerUpdater SteamMasterServerUpdaterDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMatchmaking SteamMatchmakingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMatchmakingServers SteamMatchmakingServersDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMusic SteamMusicDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamMusicRemote SteamMusicRemoteDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamNetworking SteamNetworkingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamParentalSettings SteamParentalSettingsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamRemoteStorage SteamRemoteStorageDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamScreenshots SteamScreenshotsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUGC SteamUGCDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUser SteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUserStats SteamUserStatsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamUtils SteamUtilsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate ISteamVideo SteamVideoDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void VR_GetGenericInterfaceDelegate(string pchInterfaceVersion, int peError);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate string VR_GetStringForHmdErrorDelegate(int error);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void VR_InitDelegate(int error, int type);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool VR_IsHmdPresentDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate void VR_ShutdownDelegate();





        private SteamAPI_InitDelegate _SteamAPI_InitDelegate;
        private SteamAPI_RunCallbacksDelegate _SteamAPI_RunCallbacksDelegate;
        private SteamAPI_RegisterCallResultDelegate _SteamAPI_RegisterCallResultDelegate;
        private SteamAPI_ShutdownDelegate _SteamAPI_ShutdownDelegate;
        private SteamAPI_UnregisterCallbackDelegate _SteamAPI_UnregisterCallbackDelegate;
        private SteamAPI_UnregisterCallResultDelegate _SteamAPI_UnregisterCallResultDelegate;
        private SteamAPI_RegisterCallbackDelegate _SteamAPI_RegisterCallbackDelegate;
        private SteamAPI_InitSafeDelegate _SteamAPI_InitSafeDelegate;
        private SteamAPI_InitAnonymousUserDelegate _SteamAPI_InitAnonymousUserDelegate;
        private SteamAPI_IsSteamRunningDelegate _SteamAPI_IsSteamRunningDelegate;
        private SteamAPI_RestartAppIfNecessaryDelegate _SteamAPI_RestartAppIfNecessaryDelegate;
        private SteamAPI_GetSteamInstallPathDelegate _SteamAPI_GetSteamInstallPathDelegate;
        private SteamAPI_GetHSteamUserDelegate _SteamAPI_GetHSteamUserDelegate;
        private SteamAPI_GetHSteamPipeDelegate _SteamAPI_GetHSteamPipeDelegate;
        private GetHSteamPipeDelegate _GetHSteamPipeDelegate;
        private GetHSteamUserDelegate _GetHSteamUserDelegate;
        private SteamAPI_SetTryCatchCallbacksDelegate _SteamAPI_SetTryCatchCallbacksDelegate;
        private SteamAPI_SetBreakpadAppIDDelegate _SteamAPI_SetBreakpadAppIDDelegate;
        private SteamAPI_UseBreakpadCrashHandlerDelegate _SteamAPI_UseBreakpadCrashHandlerDelegate;
        private SteamAPI_ManualDispatch_RunFrameDelegate _SteamAPI_ManualDispatch_RunFrameDelegate;
        private SteamAPI_ManualDispatch_GetNextCallbackDelegate _SteamAPI_ManualDispatch_GetNextCallbackDelegate;
        private SteamAPI_ManualDispatch_FreeLastCallbackDelegate _SteamAPI_ManualDispatch_FreeLastCallbackDelegate;
        private SteamAPI_ManualDispatch_GetAPICallResultDelegate _SteamAPI_ManualDispatch_GetAPICallResultDelegate;
        private SteamAPI_RestartAppDelegate _SteamAPI_RestartAppDelegate;
        private SteamAPI_SetMiniDumpCommentDelegate _SteamAPI_SetMiniDumpCommentDelegate;
        private SteamAPI_WriteMiniDumpDelegate _SteamAPI_WriteMiniDumpDelegate;
        private SteamAPI_ReleaseCurrentThreadMemoryDelegate _SteamAPI_ReleaseCurrentThreadMemoryDelegate;
        private SteamAPI_gameserveritem_t_ConstructDelegate _SteamAPI_gameserveritem_t_ConstructDelegate;
        private SteamAPI_gameserveritem_t_GetNameDelegate _SteamAPI_gameserveritem_t_GetNameDelegate;

        private SteamAPI_gameserveritem_t_SetNameDelegate _SteamAPI_gameserveritem_t_SetNameDelegate;

        private g_pSteamClientGameServerDelegate _g_pSteamClientGameServerDelegate;

        private Steam_RegisterInterfaceFuncsDelegate _Steam_RegisterInterfaceFuncsDelegate;

        private Steam_RunCallbacksDelegate _Steam_RunCallbacksDelegate;

        private SteamAPI_SteamAppList_v001Delegate _SteamAPI_SteamAppList_v001Delegate;

        private SteamAPI_SteamApps_v008Delegate _SteamAPI_SteamApps_v008Delegate;

        private SteamAPI_SteamGameServerApps_v008Delegate _SteamAPI_SteamGameServerApps_v008Delegate;

        private SteamAPI_SteamController_v007Delegate _SteamAPI_SteamController_v007Delegate;

        private SteamAPI_SteamController_v008Delegate _SteamAPI_SteamController_v008Delegate;

        private SteamAPI_SteamFriends_v017Delegate _SteamAPI_SteamFriends_v017Delegate;

        private SteamAPI_SteamUtils_v010Delegate _SteamAPI_SteamUtils_v010Delegate;

        private SteamAPI_SteamGameServerUtils_v010Delegate _SteamAPI_SteamGameServerUtils_v010Delegate;

        private SteamAPI_SteamUtils_v009Delegate _SteamAPI_SteamUtils_v009Delegate;

        private SteamAPI_SteamGameServerUtils_v009Delegate _SteamAPI_SteamGameServerUtils_v009Delegate;

        private SteamAPI_SteamMatchmaking_v009Delegate _SteamAPI_SteamMatchmaking_v009Delegate;

        private SteamAPI_SteamMatchmakingServers_v002Delegate _SteamAPI_SteamMatchmakingServers_v002Delegate;

        private SteamAPI_SteamGameSearch_v001Delegate _SteamAPI_SteamGameSearch_v001Delegate;

        private SteamAPI_SteamParties_v002Delegate _SteamAPI_SteamParties_v002Delegate;

        private SteamAPI_SteamNetworking_v006Delegate _SteamAPI_SteamNetworking_v006Delegate;

        private SteamAPI_SteamGameServerNetworking_v006Delegate _SteamAPI_SteamGameServerNetworking_v006Delegate;

        private SteamAPI_SteamScreenshots_v003Delegate _SteamAPI_SteamScreenshots_v003Delegate;

        private SteamAPI_SteamMusic_v001Delegate _SteamAPI_SteamMusic_v001Delegate;

        private SteamAPI_SteamMusicRemote_v001Delegate _SteamAPI_SteamMusicRemote_v001Delegate;

        private SteamAPI_SteamHTTP_v003Delegate _SteamAPI_SteamHTTP_v003Delegate;

        private SteamAPI_SteamGameServerHTTP_v003Delegate _SteamAPI_SteamGameServerHTTP_v003Delegate;

        private SteamAPI_SteamInput_v001Delegate _SteamAPI_SteamInput_v001Delegate;

        private SteamAPI_SteamInput_v002Delegate _SteamAPI_SteamInput_v002Delegate;

        private SteamAPI_SteamHTMLSurface_v005Delegate _SteamAPI_SteamHTMLSurface_v005Delegate;

        private SteamAPI_SteamInventory_v003Delegate _SteamAPI_SteamInventory_v003Delegate;

        private SteamAPI_SteamGameServerInventory_v003Delegate _SteamAPI_SteamGameServerInventory_v003Delegate;

        private SteamAPI_SteamVideo_v002Delegate _SteamAPI_SteamVideo_v002Delegate;

        private SteamAPI_SteamTV_v001Delegate _SteamAPI_SteamTV_v001Delegate;

        private SteamAPI_SteamParentalSettings_v001Delegate _SteamAPI_SteamParentalSettings_v001Delegate;

        private SteamAPI_SteamRemotePlay_v001Delegate _SteamAPI_SteamRemotePlay_v001Delegate;

        private SteamAPI_SteamNetworkingMessages_v002Delegate _SteamAPI_SteamNetworkingMessages_v002Delegate;

        private SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate;

        private SteamAPI_SteamGameServerNetworkingMessages_v002Delegate _SteamAPI_SteamGameServerNetworkingMessages_v002Delegate;

        private SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate;

        private SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate _SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate;

        private SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate _SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate;

        private SteamAPI_SteamNetworkingSockets_v009Delegate _SteamAPI_SteamNetworkingSockets_v009Delegate;

        private SteamAPI_SteamGameServerNetworkingSockets_v009Delegate _SteamAPI_SteamGameServerNetworkingSockets_v009Delegate;

        private SteamAPI_SteamNetworkingSockets_v008Delegate _SteamAPI_SteamNetworkingSockets_v008Delegate;

        private SteamAPI_SteamGameServerNetworkingSockets_v008Delegate _SteamAPI_SteamGameServerNetworkingSockets_v008Delegate;

        private SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate _SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate;

        private SteamAPI_SteamNetworkingUtils_v003Delegate _SteamAPI_SteamNetworkingUtils_v003Delegate;

        private SteamAPI_SteamGameServerStats_v001Delegate _SteamAPI_SteamGameServerStats_v001Delegate;

        private SteamAppListDelegate _SteamAppListDelegate;

        private SteamAppsDelegate _SteamAppsDelegate;

        private steamClientDelegate _steamClientDelegate;

        private SteamControllerDelegate _SteamControllerDelegate;

        private SteamFriendsDelegate _SteamFriendsDelegate;

        private SteamGameServerDelegate _SteamGameServerDelegate;

        private SteamGameServerAppsDelegate _SteamGameServerAppsDelegate;

        private SteamGameServerHTTPDelegate _SteamGameServerHTTPDelegate;

        private SteamGameServerInventoryDelegate _SteamGameServerInventoryDelegate;

        private SteamGameServerNetworkingDelegate _SteamGameServerNetworkingDelegate;

        private SteamGameServerStatsDelegate _SteamGameServerStatsDelegate;

        private SteamGameServerUGCDelegate _SteamGameServerUGCDelegate;

        private SteamGameServerUtilsDelegate _SteamGameServerUtilsDelegate;

        private SteamHTTPDelegate _SteamHTTPDelegate;

        private SteamHTMLSurfaceDelegate _SteamHTMLSurfaceDelegate;

        private SteamInventoryDelegate _SteamInventoryDelegate;

        private SteamMasterServerUpdaterDelegate _SteamMasterServerUpdaterDelegate;

        private SteamMatchmakingDelegate _SteamMatchmakingDelegate;

        private SteamMatchmakingServersDelegate _SteamMatchmakingServersDelegate;

        private SteamMusicDelegate _SteamMusicDelegate;

        private SteamMusicRemoteDelegate _SteamMusicRemoteDelegate;

        private SteamNetworkingDelegate _SteamNetworkingDelegate;

        private SteamParentalSettingsDelegate _SteamParentalSettingsDelegate;

        private SteamRemoteStorageDelegate _SteamRemoteStorageDelegate;

        private SteamScreenshotsDelegate _SteamScreenshotsDelegate;

        private SteamUGCDelegate _SteamUGCDelegate;

        private SteamUserDelegate _SteamUserDelegate;

        private SteamUserStatsDelegate _SteamUserStatsDelegate;

        private SteamUtilsDelegate _SteamUtilsDelegate;

        private SteamVideoDelegate _SteamVideoDelegate;

        private VR_GetGenericInterfaceDelegate _VR_GetGenericInterfaceDelegate;

        private VR_GetStringForHmdErrorDelegate _VR_GetStringForHmdErrorDelegate;

        private VR_InitDelegate _VR_InitDelegate;

        private VR_IsHmdPresentDelegate _VR_IsHmdPresentDelegate;

        private VR_ShutdownDelegate _VR_ShutdownDelegate;
    }
}

