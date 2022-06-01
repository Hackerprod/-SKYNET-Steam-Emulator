using EasyHook;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_InitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_RunCallbacksDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_RegisterCallResultDelegate(IntPtr pCallback, SteamAPICall_t hAPICall);

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
        public delegate IntPtr SteamAPI_GetSteamInstallPathDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate HSteamUser SteamAPI_GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate HSteamPipe SteamAPI_GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate HSteamPipe GetHSteamPipeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate HSteamUser GetHSteamUserDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_SetTryCatchCallbacksDelegate(bool bTryCatchCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_SetBreakpadAppIDDelegate(UInt32 unAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_UseBreakpadCrashHandlerDelegate([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ManualDispatch_RunFrameDelegate(HSteamPipe hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ManualDispatch_GetNextCallbackDelegate(HSteamPipe hSteamPipe, IntPtr pCallbackMs);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ManualDispatch_FreeLastCallbackDelegate(HSteamPipe hSteamPipe);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ManualDispatch_GetAPICallResultDelegate(HSteamPipe hSteamPipe, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed);

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
        public delegate IntPtr SteamAPI_SteamAppList_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamApps_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamController_v008Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamFriends_v017Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamUtils_v010Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerUtils_v010Delegate();

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
        public delegate IntPtr SteamAPI_SteamHTMLSurface_v005Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerInventory_v003Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamVideo_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamParentalSettings_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamRemotePlay_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_SteamGameServerStats_v001Delegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamClientDelegate();



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
        public static SteamAPI_SetMiniDumpCommentDelegate _SteamAPI_SetMiniDumpCommentDelegate;
        public static SteamAPI_WriteMiniDumpDelegate _SteamAPI_WriteMiniDumpDelegate;
        public static SteamAPI_ReleaseCurrentThreadMemoryDelegate _SteamAPI_ReleaseCurrentThreadMemoryDelegate;
        public static SteamAPI_gameserveritem_t_ConstructDelegate _SteamAPI_gameserveritem_t_ConstructDelegate;
        public static SteamAPI_gameserveritem_t_GetNameDelegate _SteamAPI_gameserveritem_t_GetNameDelegate;
        public static SteamAPI_gameserveritem_t_SetNameDelegate _SteamAPI_gameserveritem_t_SetNameDelegate;
        public static g_pSteamClientGameServerDelegate _g_pSteamClientGameServerDelegate;
        public static SteamAPI_SteamAppList_v001Delegate _SteamAPI_SteamAppList_v001Delegate;
        public static SteamAPI_SteamApps_v008Delegate _SteamAPI_SteamApps_v008Delegate;
        public static SteamAPI_SteamController_v008Delegate _SteamAPI_SteamController_v008Delegate;
        public static SteamAPI_SteamFriends_v017Delegate _SteamAPI_SteamFriends_v017Delegate;
        public static SteamAPI_SteamUtils_v010Delegate _SteamAPI_SteamUtils_v010Delegate;
        public static SteamAPI_SteamGameServerUtils_v010Delegate _SteamAPI_SteamGameServerUtils_v010Delegate;
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
        public static SteamAPI_SteamHTMLSurface_v005Delegate _SteamAPI_SteamHTMLSurface_v005Delegate;
        public static SteamAPI_SteamInventory_v003Delegate _SteamAPI_SteamInventory_v003Delegate;
        public static SteamAPI_SteamGameServerInventory_v003Delegate _SteamAPI_SteamGameServerInventory_v003Delegate;
        public static SteamAPI_SteamVideo_v002Delegate _SteamAPI_SteamVideo_v002Delegate;
        public static SteamAPI_SteamParentalSettings_v001Delegate _SteamAPI_SteamParentalSettings_v001Delegate;
        public static SteamAPI_SteamRemotePlay_v001Delegate _SteamAPI_SteamRemotePlay_v001Delegate;
        public static SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamNetworkingMessages_SteamAPI_v002Delegate;
        public static SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate _SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002Delegate;
        public static SteamAPI_SteamGameServerStats_v001Delegate _SteamAPI_SteamGameServerStats_v001Delegate;
        public static SteamClientDelegate _SteamClientDelegate;

    }
}

