using System;
using System.Runtime.InteropServices;
using Core.Interface;
using SKYNET;
using SKYNET.Steamworks;
using Steamworks;

//[Map("SteamUtils")]
public class SteamUtils : IBaseInterface
{
    public uint GetSecondsSinceAppActive(IntPtr _)
    {
        return 0;
    }

    public uint GetSecondsSinceComputerActive(IntPtr _)
    {
        return 0;
    }

    public EUniverse GetConnectedUniverse(IntPtr _)
    {
        return default;
    }

    public uint GetServerRealTime(IntPtr _)
    {
        return 0;
    }

    public string GetIPCountry(IntPtr _)
    {
        return "";
    }

    public bool GetImageSize(IntPtr _, int iImage, uint pnWidth, uint pnHeight)
    {
        return false;
    }

    public bool GetImageRGBA(IntPtr _, int iImage, uint pubDest, int nDestBufferSize)
    {
        return false;
    }

    public bool GetCSERIPPort(IntPtr _, uint unIP, uint usPort)
    {
        return false;
    }

    public uint GetCurrentBatteryPower(IntPtr _)
    {
        return 0;
    }

    public uint GetAppID(IntPtr _)
    {
        return 0;
    }

    public void SetOverlayNotificationPosition(IntPtr _, ENotificationPosition eNotificationPosition)
    {
        //
    }

    public bool IsAPICallCompleted(IntPtr _, SteamAPICall_t hSteamAPICall, bool pbFailed)
    {
        return false;
    }

    public ESteamAPICallFailure GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall)
    {
        return default;
    }

    public bool GetAPICallResult(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        return false;
    }

    public uint GetIPCCallCount(IntPtr _)
    {
        return 0;
    }

    public void SetWarningMessageHook(IntPtr _, SteamAPIWarningMessageHook_t pFunction)
    {
        //
    }

    public bool IsOverlayEnabled(IntPtr _)
    {
        return false;
    }

    public bool BOverlayNeedsPresent(IntPtr _)
    {
        return false;
    }

    public SteamAPICall_t CheckFileSignature(IntPtr _, string szFileName)
    {
        return default;
    }

    public bool ShowGamepadTextInput(IntPtr _, EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
    {
        return false;
    }

    public uint GetEnteredGamepadTextLength(IntPtr _)
    {
        return 0;
    }

    public bool GetEnteredGamepadTextInput(IntPtr _, string pchText, uint cchText)
    {
        return false;
    }

    public string GetSteamUILanguage(IntPtr _)
    {
        return "";
    }

    public bool IsSteamRunningInVR(IntPtr _)
    {
        return false;
    }

    public void SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset)
    {
        //
    }

    public bool IsSteamInBigPictureMode(IntPtr _)
    {
        return false;
    }

    public void StartVRDashboard(IntPtr _)
    {
        //
    }

    public bool IsVRHeadsetStreamingEnabled(IntPtr _)
    {
        return false;
    }

    public void SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled)
    {
        //
    }

    public bool IsSteamChinaLauncher(IntPtr _)
    {
        return false;
    }

    public bool InitFilterText(IntPtr _)
    {
        return false;
    }

    public int FilterText(IntPtr _, string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
    {
        return 0;
    }

    public ESteamIPv6ConnectivityState GetIPv6ConnectivityState(IntPtr _, ESteamIPv6ConnectivityProtocol eProtocol)
    {
        return default;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}