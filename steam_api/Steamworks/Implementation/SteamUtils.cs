using System;
using System.Runtime.InteropServices;
using Core.Interface;
using SKYNET.Interface;
using Steamworks;

//[Map("SteamUtils")]
public class SteamUtils : IBaseInterface, ISteamUtils
{
    public uint GetSecondsSinceAppActive()
    {
        return 0;
    }

    public uint GetSecondsSinceComputerActive()
    {
        return 0;
    }

    public EUniverse GetConnectedUniverse()
    {
        return default;
    }

    public uint GetServerRealTime()
    {
        return 0;
    }

    public string GetIPCountry()
    {
        return "";
    }

    public bool GetImageSize(int iImage, uint pnWidth, uint pnHeight)
    {
        return false;
    }

    public bool GetImageRGBA(int iImage, uint pubDest, int nDestBufferSize)
    {
        return false;
    }

    public bool GetCSERIPPort(uint unIP, uint usPort)
    {
        return false;
    }

    public uint GetCurrentBatteryPower()
    {
        return 0;
    }

    public uint GetAppID()
    {
        return 0;
    }

    public void SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition)
    {
        //
    }

    public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, bool pbFailed)
    {
        return false;
    }

    public ESteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
    {
        return default;
    }

    public bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        return false;
    }

    public uint GetIPCCallCount()
    {
        return 0;
    }

    public void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
    {
        //
    }

    public bool IsOverlayEnabled()
    {
        return false;
    }

    public bool BOverlayNeedsPresent()
    {
        return false;
    }

    public SteamAPICall_t CheckFileSignature(string szFileName)
    {
        return default;
    }

    public bool ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
    {
        return false;
    }

    public uint GetEnteredGamepadTextLength()
    {
        return 0;
    }

    public bool GetEnteredGamepadTextInput(string pchText, uint cchText)
    {
        return false;
    }

    public string GetSteamUILanguage()
    {
        return "";
    }

    public bool IsSteamRunningInVR()
    {
        return false;
    }

    public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
    {
        //
    }

    public bool IsSteamInBigPictureMode()
    {
        return false;
    }

    public void StartVRDashboard()
    {
        //
    }

    public bool IsVRHeadsetStreamingEnabled()
    {
        return false;
    }

    public void SetVRHeadsetStreamingEnabled(bool bEnabled)
    {
        //
    }

    public bool IsSteamChinaLauncher()
    {
        return false;
    }

    public bool InitFilterText()
    {
        return false;
    }

    public int FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
    {
        return 0;
    }

    public ESteamIPv6ConnectivityState GetIPv6ConnectivityState(ESteamIPv6ConnectivityProtocol eProtocol)
    {
        return default;
    }

}