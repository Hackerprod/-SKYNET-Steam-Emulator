using SKYNET.Interface;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Delegate
{
    [Delegate("SteamUtils")]
    public class DSteamUtils : SteamDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSecondsSinceAppActive();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSecondsSinceComputerActive();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EUniverse GetConnectedUniverse();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetServerRealTime();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetIPCountry();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetImageSize(int iImage, uint pnWidth, uint pnHeight);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetImageRGBA(int iImage, uint pubDest, int nDestBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetCSERIPPort(uint unIP, uint usPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetCurrentBatteryPower();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetAppID();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetIPCCallCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsOverlayEnabled();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BOverlayNeedsPresent();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CheckFileSignature(string szFileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetEnteredGamepadTextLength();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetEnteredGamepadTextInput(string pchText, uint cchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetSteamUILanguage();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamRunningInVR();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamInBigPictureMode();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StartVRDashboard();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsVRHeadsetStreamingEnabled();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetVRHeadsetStreamingEnabled(bool bEnabled);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsSteamChinaLauncher();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InitFilterText();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamIPv6ConnectivityState GetIPv6ConnectivityState(ESteamIPv6ConnectivityProtocol eProtocol);
    }
}