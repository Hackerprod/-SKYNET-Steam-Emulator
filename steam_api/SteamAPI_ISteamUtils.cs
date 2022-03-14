using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamUtils : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetSecondsSinceAppActive()
    {
        Write("SteamAPI_ISteamUtils_GetSecondsSinceAppActive");
        return SteamClient.SteamUtils.GetSecondsSinceAppActive();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetSecondsSinceComputerActive()
    {
        Write("SteamAPI_ISteamUtils_GetSecondsSinceComputerActive");
        return SteamClient.SteamUtils.GetSecondsSinceComputerActive();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EUniverse SteamAPI_ISteamUtils_GetConnectedUniverse()
    {
        Write("SteamAPI_ISteamUtils_GetConnectedUniverse");
        return SteamClient.SteamUtils.GetConnectedUniverse();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetServerRealTime()
    {
        Write("SteamAPI_ISteamUtils_GetServerRealTime");
        return SteamClient.SteamUtils.GetServerRealTime();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamUtils_GetIPCountry()
    {
        Write("SteamAPI_ISteamUtils_GetIPCountry");
        return SteamClient.SteamUtils.GetIPCountry();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_GetImageSize(int iImage, uint pnWidth, uint pnHeight)
    {
        Write("SteamAPI_ISteamUtils_GetImageSize");
        return SteamClient.SteamUtils.GetImageSize(iImage, pnWidth, pnHeight);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_GetImageRGBA(int iImage, uint pubDest, int nDestBufferSize)
    {
        Write("SteamAPI_ISteamUtils_GetImageRGBA");
        return SteamClient.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_GetCSERIPPort(uint unIP, uint usPort)
    {
        Write("SteamAPI_ISteamUtils_GetCSERIPPort");
        return SteamClient.SteamUtils.GetCSERIPPort(unIP, usPort);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetCurrentBatteryPower()
    {
        Write("SteamAPI_ISteamUtils_GetCurrentBatteryPower");
        return SteamClient.SteamUtils.GetCurrentBatteryPower();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetAppID()
    {
        Write("SteamAPI_ISteamUtils_GetAppID");
        return SteamClient.SteamUtils.GetAppID();
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
        return SteamClient.SteamUtils.IsAPICallCompleted(hSteamAPICall, pbFailed);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamAPICallFailure SteamAPI_ISteamUtils_GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
    {
        Write("SteamAPI_ISteamUtils_GetAPICallFailureReason");
        return SteamClient.SteamUtils.GetAPICallFailureReason(hSteamAPICall);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        Write("SteamAPI_ISteamUtils_GetAPICallResult");
        return SteamClient.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, pbFailed);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetIPCCallCount()
    {
        Write("SteamAPI_ISteamUtils_GetIPCCallCount");
        return SteamClient.SteamUtils.GetIPCCallCount();
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
        return SteamClient.SteamUtils.IsOverlayEnabled();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_BOverlayNeedsPresent()
    {
        Write("SteamAPI_ISteamUtils_BOverlayNeedsPresent");
        return SteamClient.SteamUtils.BOverlayNeedsPresent();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUtils_CheckFileSignature(string szFileName)
    {
        Write("SteamAPI_ISteamUtils_CheckFileSignature");
        return SteamClient.SteamUtils.CheckFileSignature(szFileName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
    {
        Write("SteamAPI_ISteamUtils_ShowGamepadTextInput");
        return SteamClient.SteamUtils.ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUtils_GetEnteredGamepadTextLength()
    {
        Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextLength");
        return SteamClient.SteamUtils.GetEnteredGamepadTextLength();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_GetEnteredGamepadTextInput(string pchText, uint cchText)
    {
        Write("SteamAPI_ISteamUtils_GetEnteredGamepadTextInput");
        return SteamClient.SteamUtils.GetEnteredGamepadTextInput(pchText, cchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamUtils_GetSteamUILanguage()
    {
        Write("SteamAPI_ISteamUtils_GetSteamUILanguage");
        return SteamClient.SteamUtils.GetSteamUILanguage();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_IsSteamRunningInVR()
    {
        Write("SteamAPI_ISteamUtils_IsSteamRunningInVR");
        return SteamClient.SteamUtils.IsSteamRunningInVR();
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
        return SteamClient.SteamUtils.IsSteamInBigPictureMode();
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
        return SteamClient.SteamUtils.IsVRHeadsetStreamingEnabled();
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
        return SteamClient.SteamUtils.IsSteamChinaLauncher();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUtils_InitFilterText()
    {
        Write("SteamAPI_ISteamUtils_InitFilterText");
        return SteamClient.SteamUtils.InitFilterText();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamUtils_FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
    {
        Write("SteamAPI_ISteamUtils_FilterText");
        return SteamClient.SteamUtils.FilterText(pchOutFilteredText, nByteSizeOutFilteredText, pchInputMessage, bLegalOnly);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamIPv6ConnectivityState SteamAPI_ISteamUtils_GetIPv6ConnectivityState(ESteamIPv6ConnectivityProtocol eProtocol)
    {
        Write("SteamAPI_ISteamUtils_GetIPv6ConnectivityState");
        return SteamClient.SteamUtils.GetIPv6ConnectivityState(eProtocol);
    }

}

