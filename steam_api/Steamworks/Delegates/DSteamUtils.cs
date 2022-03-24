using Core.Interface;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamUtils")]
    public class DSteamUtils 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSecondsSinceAppActive(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSecondsSinceComputerActive(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EUniverse GetConnectedUniverse(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetServerRealTime(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetIPCountry(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetImageSize(IntPtr _, int iImage, uint pnWidth, uint pnHeight);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetImageRGBA(IntPtr _, int iImage, uint pubDest, int nDestBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetCSERIPPort(IntPtr _, uint unIP, uint usPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetCurrentBatteryPower(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetAppID(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetOverlayNotificationPosition(IntPtr _, ENotificationPosition eNotificationPosition);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsAPICallCompleted(IntPtr _, SteamAPICall_t hSteamAPICall, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamAPICallFailure GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAPICallResult(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetIPCCallCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetWarningMessageHook(IntPtr _, SteamAPIWarningMessageHook_t pFunction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsOverlayEnabled(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BOverlayNeedsPresent(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CheckFileSignature(IntPtr _, string szFileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ShowGamepadTextInput(IntPtr _, EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetEnteredGamepadTextLength(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetEnteredGamepadTextInput(IntPtr _, string pchText, uint cchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetSteamUILanguage(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamRunningInVR(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamInBigPictureMode(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StartVRDashboard(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsVRHeadsetStreamingEnabled(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamChinaLauncher(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InitFilterText(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int FilterText(IntPtr _, string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamIPv6ConnectivityState GetIPv6ConnectivityState(IntPtr _, ESteamIPv6ConnectivityProtocol eProtocol);
    }
}