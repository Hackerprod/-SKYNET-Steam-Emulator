using SKYNET;
using SKYNET.Steamworks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamUtils
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetSecondsSinceAppActive(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetSecondsSinceAppActive");
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetSecondsSinceComputerActive(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetSecondsSinceComputerActive");
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_GetConnectedUniverse(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetConnectedUniverse");
            return SteamEmulator.SteamUtils.GetConnectedUniverse();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetServerRealTime(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetServerRealTime");
            return SteamEmulator.SteamUtils.GetServerRealTime();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamUtils_GetIPCountry(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetIPCountry");
            return SteamEmulator.SteamUtils.GetIPCountry();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetImageSize(IntPtr _, int iImage, ref uint pnWidth, ref uint pnHeight)
        {
            Write("SteamAPI_ISteamUtils_GetImageSize");
            return SteamEmulator.SteamUtils.GetImageSize(iImage, ref pnWidth, ref pnHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetImageRGBA(IntPtr _, int iImage, IntPtr pubDest, int nDestBufferSize)
        {
            Write("SteamAPI_ISteamUtils_GetImageRGBA");
            return SteamEmulator.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetCSERIPPort(IntPtr _, uint unIP, uint usPort)
        {
            Write("SteamAPI_ISteamUtils_GetCSERIPPort");
            return SteamEmulator.SteamUtils.GetCSERIPPort(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_GetCurrentBatteryPower(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetCurrentBatteryPower");
            return SteamEmulator.SteamUtils.GetCurrentBatteryPower();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetAppID(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetAppID");
            return SteamEmulator.SteamUtils.GetAppID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetOverlayNotificationPosition(IntPtr _, int eNotificationPosition)
        {
            Write("SteamAPI_ISteamUtils_SetOverlayNotificationPosition");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsAPICallCompleted(IntPtr _, SteamAPICall_t hSteamAPICall, ref bool pbFailed)
        {
            Write("SteamAPI_ISteamUtils_IsAPICallCompleted");
            return SteamEmulator.SteamUtils.IsAPICallCompleted(hSteamAPICall, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall)
        {
            Write("SteamAPI_ISteamUtils_GetAPICallFailureReason");
            return SteamEmulator.SteamUtils.GetAPICallFailureReason(hSteamAPICall);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetAPICallResult(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
        {
            Write("SteamAPI_ISteamUtils_GetAPICallResult");
            return SteamEmulator.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetIPCCallCount(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetIPCCallCount");
            return SteamEmulator.SteamUtils.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            Write("SteamAPI_ISteamUtils_SetWarningMessageHook");
            SteamEmulator.SteamUtils.SetWarningMessageHook(pFunction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsOverlayEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_IsOverlayEnabled");
            return SteamEmulator.SteamUtils.IsOverlayEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_BOverlayNeedsPresent(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_BOverlayNeedsPresent");
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUtils_CheckFileSignature(IntPtr _, string szFileName)
        {
            Write("SteamAPI_ISteamUtils_CheckFileSignature");
            return SteamEmulator.SteamUtils.CheckFileSignature(szFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_ShowGamepadTextInput(IntPtr _, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            Write("SteamAPI_ISteamUtils_ShowGamepadTextInput");
            return SteamEmulator.SteamUtils.ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetEnteredGamepadTextLength(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextLength");
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetEnteredGamepadTextInput(IntPtr _, string pchText, uint cchText)
        {
            Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextInput");
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(pchText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamUtils_GetSteamUILanguage(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_GetSteamUILanguage");
            return SteamEmulator.SteamUtils.GetSteamUILanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamRunningInVR(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_IsSteamRunningInVR");
            return SteamEmulator.SteamUtils.IsSteamRunningInVR();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset)
        {
            Write("SteamAPI_ISteamUtils_SetOverlayNotificationInset");

        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamInBigPictureMode(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_IsSteamInBigPictureMode");
            return SteamEmulator.SteamUtils.IsSteamInBigPictureMode();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_StartVRDashboard(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_StartVRDashboard");

        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsVRHeadsetStreamingEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_IsVRHeadsetStreamingEnabled");
            return SteamEmulator.SteamUtils.IsVRHeadsetStreamingEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled)
        {
            Write("SteamAPI_ISteamUtils_SetVRHeadsetStreamingEnabled");

        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamChinaLauncher(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_IsSteamChinaLauncher");
            return SteamEmulator.SteamUtils.IsSteamChinaLauncher();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_InitFilterText(IntPtr _)
        {
            Write("SteamAPI_ISteamUtils_InitFilterText");
            return SteamEmulator.SteamUtils.InitFilterText();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_FilterText(IntPtr _, string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
        {
            Write("SteamAPI_ISteamUtils_FilterText");
            return SteamEmulator.SteamUtils.FilterText(pchOutFilteredText, nByteSizeOutFilteredText, pchInputMessage, bLegalOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_GetIPv6ConnectivityState(IntPtr _, int eProtocol)
        {
            Write("SteamAPI_ISteamUtils_GetIPv6ConnectivityState");
            return SteamEmulator.SteamUtils.GetIPv6ConnectivityState(eProtocol);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

