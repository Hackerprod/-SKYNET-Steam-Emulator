using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamUtils : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetSecondsSinceAppActive()
        {
            Write("SteamAPI_ISteamUtils_GetSecondsSinceAppActive");
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetSecondsSinceComputerActive()
        {
            Write("SteamAPI_ISteamUtils_GetSecondsSinceComputerActive");
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EUniverse SteamAPI_ISteamUtils_GetConnectedUniverse()
        {
            Write("SteamAPI_ISteamUtils_GetConnectedUniverse");
            return SteamEmulator.SteamUtils.GetConnectedUniverse(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetServerRealTime()
        {
            Write("SteamAPI_ISteamUtils_GetServerRealTime");
            return SteamEmulator.SteamUtils.GetServerRealTime(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamUtils_GetIPCountry()
        {
            Write("SteamAPI_ISteamUtils_GetIPCountry");
            return SteamEmulator.SteamUtils.GetIPCountry(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetImageSize(int iImage, uint pnWidth, uint pnHeight)
        {
            Write("SteamAPI_ISteamUtils_GetImageSize");
            return SteamEmulator.SteamUtils.GetImageSize(SteamEmulator.SteamUtils.MemoryAddress, iImage, pnWidth, pnHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetImageRGBA(int iImage, uint pubDest, int nDestBufferSize)
        {
            Write("SteamAPI_ISteamUtils_GetImageRGBA");
            return SteamEmulator.SteamUtils.GetImageRGBA(SteamEmulator.SteamUtils.MemoryAddress, iImage, pubDest, nDestBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetCSERIPPort(uint unIP, uint usPort)
        {
            Write("SteamAPI_ISteamUtils_GetCSERIPPort");
            return SteamEmulator.SteamUtils.GetCSERIPPort(SteamEmulator.SteamUtils.MemoryAddress, unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetCurrentBatteryPower()
        {
            Write("SteamAPI_ISteamUtils_GetCurrentBatteryPower");
            return SteamEmulator.SteamUtils.GetCurrentBatteryPower(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetAppID()
        {
            Write("SteamAPI_ISteamUtils_GetAppID");
            return SteamEmulator.SteamUtils.GetAppID(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition)
        {
            Write("SteamAPI_ISteamUtils_SetOverlayNotificationPosition");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsAPICallCompleted(SteamAPICall_t hSteamAPICall, bool pbFailed)
        {
            Write("SteamAPI_ISteamUtils_IsAPICallCompleted");
            return SteamEmulator.SteamUtils.IsAPICallCompleted(SteamEmulator.SteamUtils.MemoryAddress, hSteamAPICall, pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESteamAPICallFailure SteamAPI_ISteamUtils_GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
        {
            Write("SteamAPI_ISteamUtils_GetAPICallFailureReason");
            return SteamEmulator.SteamUtils.GetAPICallFailureReason(SteamEmulator.SteamUtils.MemoryAddress, hSteamAPICall);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
        {
            Write("SteamAPI_ISteamUtils_GetAPICallResult");
            return SteamEmulator.SteamUtils.GetAPICallResult(SteamEmulator.SteamUtils.MemoryAddress, hSteamAPICall, pCallback, cubCallback, iCallbackExpected, pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetIPCCallCount()
        {
            Write("SteamAPI_ISteamUtils_GetIPCCallCount");
            return SteamEmulator.SteamUtils.GetIPCCallCount(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
        {
            Write("SteamAPI_ISteamUtils_SetWarningMessageHook");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsOverlayEnabled()
        {
            Write("SteamAPI_ISteamUtils_IsOverlayEnabled");
            return SteamEmulator.SteamUtils.IsOverlayEnabled(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_BOverlayNeedsPresent()
        {
            Write("SteamAPI_ISteamUtils_BOverlayNeedsPresent");
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUtils_CheckFileSignature(string szFileName)
        {
            Write("SteamAPI_ISteamUtils_CheckFileSignature");
            return SteamEmulator.SteamUtils.CheckFileSignature(SteamEmulator.SteamUtils.MemoryAddress, szFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            Write("SteamAPI_ISteamUtils_ShowGamepadTextInput");
            return SteamEmulator.SteamUtils.ShowGamepadTextInput(SteamEmulator.SteamUtils.MemoryAddress, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUtils_GetEnteredGamepadTextLength()
        {
            Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextLength");
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_GetEnteredGamepadTextInput(string pchText, uint cchText)
        {
            Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextInput");
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(SteamEmulator.SteamUtils.MemoryAddress, pchText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamUtils_GetSteamUILanguage()
        {
            Write("SteamAPI_ISteamUtils_GetSteamUILanguage");
            return SteamEmulator.SteamUtils.GetSteamUILanguage(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamRunningInVR()
        {
            Write("SteamAPI_ISteamUtils_IsSteamRunningInVR");
            return SteamEmulator.SteamUtils.IsSteamRunningInVR(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
        {
            Write("SteamAPI_ISteamUtils_SetOverlayNotificationInset");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamInBigPictureMode()
        {
            Write("SteamAPI_ISteamUtils_IsSteamInBigPictureMode");
            return SteamEmulator.SteamUtils.IsSteamInBigPictureMode(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_StartVRDashboard()
        {
            Write("SteamAPI_ISteamUtils_StartVRDashboard");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsVRHeadsetStreamingEnabled()
        {
            Write("SteamAPI_ISteamUtils_IsVRHeadsetStreamingEnabled");
            return SteamEmulator.SteamUtils.IsVRHeadsetStreamingEnabled(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUtils_SetVRHeadsetStreamingEnabled(bool bEnabled)
        {
            Write("SteamAPI_ISteamUtils_SetVRHeadsetStreamingEnabled");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_IsSteamChinaLauncher()
        {
            Write("SteamAPI_ISteamUtils_IsSteamChinaLauncher");
            return SteamEmulator.SteamUtils.IsSteamChinaLauncher(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUtils_InitFilterText()
        {
            Write("SteamAPI_ISteamUtils_InitFilterText");
            return SteamEmulator.SteamUtils.InitFilterText(SteamEmulator.SteamUtils.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUtils_FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
        {
            Write("SteamAPI_ISteamUtils_FilterText");
            return SteamEmulator.SteamUtils.FilterText(SteamEmulator.SteamUtils.MemoryAddress, pchOutFilteredText, nByteSizeOutFilteredText, pchInputMessage, bLegalOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESteamIPv6ConnectivityState SteamAPI_ISteamUtils_GetIPv6ConnectivityState(ESteamIPv6ConnectivityProtocol eProtocol)
        {
            Write("SteamAPI_ISteamUtils_GetIPv6ConnectivityState");
            return SteamEmulator.SteamUtils.GetIPv6ConnectivityState(SteamEmulator.SteamUtils.MemoryAddress, eProtocol);
        }
    }
}

