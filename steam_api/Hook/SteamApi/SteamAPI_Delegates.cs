using EasyHook;
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
        public delegate bool SteamAPI_InitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_RunCallbacksDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_RegisterCallResultDelegate(CCallbackBase pCallback, SteamAPICall_t hAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ShutdownDelegate(IntPtr pContextInitData);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_UnregisterCallbackDelegate(IntPtr pCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_UnregisterCallResultDelegate(IntPtr pCallback, SteamAPICall_t hAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_RegisterCallbackDelegate(IntPtr pCallback, int iCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_InitSafeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_InitAnonymousUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_IsSteamRunningDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_RestartAppIfNecessaryDelegate(uint appId);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_GetSteamInstallPathDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_SetTryCatchCallbacksDelegate(bool bTryCatchCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_SetBreakpadAppIDDelegate(UInt32 unAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_UseBreakpadCrashHandlerDelegate([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ManualDispatch_RunFrameDelegate(IntPtr hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ManualDispatch_GetNextCallbackDelegate(int hSteamPipe, IntPtr pCallbackMsg);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ManualDispatch_FreeLastCallbackDelegate(int hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ManualDispatch_GetAPICallResultDelegate(int hSteamPipe, IntPtr hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_RestartAppDelegate(UInt32 appid);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_SetMiniDumpCommentDelegate([MarshalAs(UnmanagedType.LPStr)] string pchMsg);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_WriteMiniDumpDelegate(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ReleaseCurrentThreadMemoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_gameserveritem_t_ConstructDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_gameserveritem_t_GetNameDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_gameserveritem_t_SetNameDelegate(IntPtr self, IntPtr pName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr g_pSteamClientGameServerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void Steam_RegisterInterfaceFuncsDelegate(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void Steam_RunCallbacksDelegate(IntPtr hSteamPipe, bool bGameServerCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamAppList_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamApps_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerApps_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamController_v007Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamController_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamFriends_v017Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamUtils_v010Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerUtils_v010Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamUtils_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerUtils_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamMatchmaking_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamMatchmakingServers_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameSearch_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamParties_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworking_v006Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworking_v006Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamScreenshots_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamMusic_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamMusicRemote_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamHTTP_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerHTTP_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamInput_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamInput_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamHTMLSurface_v005Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamVideo_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamTV_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamParentalSettings_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamRemotePlay_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingMessages_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingMessages_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingSockets_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_v009Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingSockets_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingSockets_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingUtils_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerStats_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAppListDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAppsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate SteamClient steamClientDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamControllerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamFriendsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerAppsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerHTTPDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerInventoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerNetworkingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerStatsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerUGCDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamGameServerUtilsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamHTTPDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamHTMLSurfaceDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInventoryDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamMasterServerUpdaterDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamMatchmakingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamMatchmakingServersDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamMusicDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamMusicRemoteDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamNetworkingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamParentalSettingsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamRemoteStorageDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamScreenshotsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamUGCDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamUserStatsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamUtilsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamVideoDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void VR_GetGenericInterfaceDelegate(string pchInterfaceVersion, int peError);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string VR_GetStringForHmdErrorDelegate(int error);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void VR_InitDelegate(int error, int type);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool VR_IsHmdPresentDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void VR_ShutdownDelegate();





        public static SteamAPI_InitDelegate _SteamAPI_InitDelegate;
        public static SteamAPI_RunCallbacksDelegate _SteamAPI_RunCallbacksDelegate;
        public static SteamAPI_RegisterCallResultDelegate _SteamAPI_RegisterCallResultDelegate;
        public static SteamAPI_ShutdownDelegate _SteamAPI_ShutdownDelegate;
        public static SteamAPI_UnregisterCallbackDelegate _SteamAPI_UnregisterCallbackDelegate;
        public static SteamAPI_UnregisterCallResultDelegate _SteamAPI_UnregisterCallResultDelegate;
        public static SteamAPI_RegisterCallbackDelegate _SteamAPI_RegisterCallbackDelegate;
        public static SteamAPI_InitSafeDelegate _SteamAPI_InitSafeDelegate;
        public static SteamAPI_InitAnonymousUserDelegate _SteamAPI_InitAnonymousUserDelegate;
        public static SteamAPI_IsSteamRunningDelegate _SteamAPI_IsSteamRunningDelegate;
        public static SteamAPI_RestartAppIfNecessaryDelegate _SteamAPI_RestartAppIfNecessaryDelegate;
        public static SteamAPI_GetSteamInstallPathDelegate _SteamAPI_GetSteamInstallPathDelegate;
        public static SteamAPI_GetHSteamUserDelegate _SteamAPI_GetHSteamUserDelegate;
        public static SteamAPI_GetHSteamPipeDelegate _SteamAPI_GetHSteamPipeDelegate;
        public static GetHSteamPipeDelegate _GetHSteamPipeDelegate;
        public static GetHSteamUserDelegate _GetHSteamUserDelegate;
        public static SteamAPI_SetTryCatchCallbacksDelegate _SteamAPI_SetTryCatchCallbacksDelegate;
        public static SteamAPI_SetBreakpadAppIDDelegate _SteamAPI_SetBreakpadAppIDDelegate;
        public static SteamAPI_UseBreakpadCrashHandlerDelegate _SteamAPI_UseBreakpadCrashHandlerDelegate;
        public static SteamAPI_ManualDispatch_RunFrameDelegate _SteamAPI_ManualDispatch_RunFrameDelegate;
        public static SteamAPI_ManualDispatch_GetNextCallbackDelegate _SteamAPI_ManualDispatch_GetNextCallbackDelegate;
        public static SteamAPI_ManualDispatch_FreeLastCallbackDelegate _SteamAPI_ManualDispatch_FreeLastCallbackDelegate;
        public static SteamAPI_ManualDispatch_GetAPICallResultDelegate _SteamAPI_ManualDispatch_GetAPICallResultDelegate;
        public static SteamAPI_RestartAppDelegate _SteamAPI_RestartAppDelegate;
        public static SteamAPI_SetMiniDumpCommentDelegate _SteamAPI_SetMiniDumpCommentDelegate;
        public static SteamAPI_WriteMiniDumpDelegate _SteamAPI_WriteMiniDumpDelegate;
        public static SteamAPI_ReleaseCurrentThreadMemoryDelegate _SteamAPI_ReleaseCurrentThreadMemoryDelegate;
        public static SteamAPI_gameserveritem_t_ConstructDelegate _SteamAPI_gameserveritem_t_ConstructDelegate;
        public static SteamAPI_gameserveritem_t_GetNameDelegate _SteamAPI_gameserveritem_t_GetNameDelegate;
        public static SteamAPI_gameserveritem_t_SetNameDelegate _SteamAPI_gameserveritem_t_SetNameDelegate;
        public static g_pSteamClientGameServerDelegate _g_pSteamClientGameServerDelegate;
        public static Steam_RegisterInterfaceFuncsDelegate _Steam_RegisterInterfaceFuncsDelegate;
        public static Steam_RunCallbacksDelegate _Steam_RunCallbacksDelegate;
        public static SteamAPI_SteamAppList_v001Delegate _SteamAPI_SteamAppList_v001Delegate;
        public static SteamAPI_SteamApps_v008Delegate _SteamAPI_SteamApps_v008Delegate;
        public static SteamAPI_SteamGameServerApps_v008Delegate _SteamAPI_SteamGameServerApps_v008Delegate;
        public static SteamAPI_SteamController_v007Delegate _SteamAPI_SteamController_v007Delegate;
        public static SteamAPI_SteamController_v008Delegate _SteamAPI_SteamController_v008Delegate;
        public static SteamAPI_SteamFriends_v017Delegate _SteamAPI_SteamFriends_v017Delegate;
        public static SteamAPI_SteamUtils_v010Delegate _SteamAPI_SteamUtils_v010Delegate;
        public static SteamAPI_SteamGameServerUtils_v010Delegate _SteamAPI_SteamGameServerUtils_v010Delegate;
        public static SteamAPI_SteamUtils_v009Delegate _SteamAPI_SteamUtils_v009Delegate;
        public static SteamAPI_SteamGameServerUtils_v009Delegate _SteamAPI_SteamGameServerUtils_v009Delegate;

        public static SteamAPI_SteamMatchmaking_v009Delegate _SteamAPI_SteamMatchmaking_v009Delegate;

        public static SteamAPI_SteamMatchmakingServers_v002Delegate _SteamAPI_SteamMatchmakingServers_v002Delegate;

        public static SteamAPI_SteamGameSearch_v001Delegate _SteamAPI_SteamGameSearch_v001Delegate;

        public static SteamAPI_SteamParties_v002Delegate _SteamAPI_SteamParties_v002Delegate;

        public static SteamAPI_SteamNetworking_v006Delegate _SteamAPI_SteamNetworking_v006Delegate;

        public static SteamAPI_SteamGameServerNetworking_v006Delegate _SteamAPI_SteamGameServerNetworking_v006Delegate;

        public static SteamAPI_SteamScreenshots_v003Delegate _SteamAPI_SteamScreenshots_v003Delegate;

        public static SteamAPI_SteamMusic_v001Delegate _SteamAPI_SteamMusic_v001Delegate;

        public static SteamAPI_SteamMusicRemote_v001Delegate _SteamAPI_SteamMusicRemote_v001Delegate;

        public static SteamAPI_SteamHTTP_v003Delegate _SteamAPI_SteamHTTP_v003Delegate;

        public static SteamAPI_SteamGameServerHTTP_v003Delegate _SteamAPI_SteamGameServerHTTP_v003Delegate;

        public static SteamAPI_SteamInput_v001Delegate _SteamAPI_SteamInput_v001Delegate;

        public static SteamAPI_SteamInput_v002Delegate _SteamAPI_SteamInput_v002Delegate;

        public static SteamAPI_SteamHTMLSurface_v005Delegate _SteamAPI_SteamHTMLSurface_v005Delegate;

        public static SteamAPI_SteamInventory_v003Delegate _SteamAPI_SteamInventory_v003Delegate;

        public static SteamAPI_SteamGameServerInventory_v003Delegate _SteamAPI_SteamGameServerInventory_v003Delegate;

        public static SteamAPI_SteamVideo_v002Delegate _SteamAPI_SteamVideo_v002Delegate;

        public static SteamAPI_SteamTV_v001Delegate _SteamAPI_SteamTV_v001Delegate;

        public static SteamAPI_SteamParentalSettings_v001Delegate _SteamAPI_SteamParentalSettings_v001Delegate;

        public static SteamAPI_SteamRemotePlay_v001Delegate _SteamAPI_SteamRemotePlay_v001Delegate;

        public static SteamAPI_SteamNetworkingMessages_v002Delegate _SteamAPI_SteamNetworkingMessages_v002Delegate;

        public static SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate;

        public static SteamAPI_SteamGameServerNetworkingMessages_v002Delegate _SteamAPI_SteamGameServerNetworkingMessages_v002Delegate;

        public static SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate;

        public static SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate _SteamAPI_SteamNetworkingSockets_SteamAPI_v009Delegate;

        public static SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate _SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009Delegate;

        public static SteamAPI_SteamNetworkingSockets_v009Delegate _SteamAPI_SteamNetworkingSockets_v009Delegate;

        public static SteamAPI_SteamGameServerNetworkingSockets_v009Delegate _SteamAPI_SteamGameServerNetworkingSockets_v009Delegate;

        public static SteamAPI_SteamNetworkingSockets_v008Delegate _SteamAPI_SteamNetworkingSockets_v008Delegate;

        public static SteamAPI_SteamGameServerNetworkingSockets_v008Delegate _SteamAPI_SteamGameServerNetworkingSockets_v008Delegate;

        public static SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate _SteamAPI_SteamNetworkingUtils_SteamAPI_v003Delegate;

        public static SteamAPI_SteamNetworkingUtils_v003Delegate _SteamAPI_SteamNetworkingUtils_v003Delegate;

        public static SteamAPI_SteamGameServerStats_v001Delegate _SteamAPI_SteamGameServerStats_v001Delegate;

        public static SteamAppListDelegate _SteamAppListDelegate;

        public static SteamAppsDelegate _SteamAppsDelegate;

        public static steamClientDelegate _steamClientDelegate;

        public static SteamControllerDelegate _SteamControllerDelegate;

        public static SteamFriendsDelegate _SteamFriendsDelegate;

        public static SteamGameServerDelegate _SteamGameServerDelegate;

        public static SteamGameServerAppsDelegate _SteamGameServerAppsDelegate;

        public static SteamGameServerHTTPDelegate _SteamGameServerHTTPDelegate;

        public static SteamGameServerInventoryDelegate _SteamGameServerInventoryDelegate;

        public static SteamGameServerNetworkingDelegate _SteamGameServerNetworkingDelegate;

        public static SteamGameServerStatsDelegate _SteamGameServerStatsDelegate;

        public static SteamGameServerUGCDelegate _SteamGameServerUGCDelegate;

        public static SteamGameServerUtilsDelegate _SteamGameServerUtilsDelegate;

        public static SteamHTTPDelegate _SteamHTTPDelegate;

        public static SteamHTMLSurfaceDelegate _SteamHTMLSurfaceDelegate;

        public static SteamInventoryDelegate _SteamInventoryDelegate;

        public static SteamMasterServerUpdaterDelegate _SteamMasterServerUpdaterDelegate;

        public static SteamMatchmakingDelegate _SteamMatchmakingDelegate;

        public static SteamMatchmakingServersDelegate _SteamMatchmakingServersDelegate;

        public static SteamMusicDelegate _SteamMusicDelegate;

        public static SteamMusicRemoteDelegate _SteamMusicRemoteDelegate;

        public static SteamNetworkingDelegate _SteamNetworkingDelegate;

        public static SteamParentalSettingsDelegate _SteamParentalSettingsDelegate;

        public static SteamRemoteStorageDelegate _SteamRemoteStorageDelegate;

        public static SteamScreenshotsDelegate _SteamScreenshotsDelegate;

        public static SteamUGCDelegate _SteamUGCDelegate;

        public static SteamUserDelegate _SteamUserDelegate;

        public static SteamUserStatsDelegate _SteamUserStatsDelegate;

        public static SteamUtilsDelegate _SteamUtilsDelegate;

        public static SteamVideoDelegate _SteamVideoDelegate;

        public static VR_GetGenericInterfaceDelegate _VR_GetGenericInterfaceDelegate;

        public static VR_GetStringForHmdErrorDelegate _VR_GetStringForHmdErrorDelegate;

        public static VR_InitDelegate _VR_InitDelegate;

        public static VR_IsHmdPresentDelegate _VR_IsHmdPresentDelegate;

        public static VR_ShutdownDelegate _VR_ShutdownDelegate;
    }
}

