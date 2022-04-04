using System;
using System.Runtime.InteropServices;

using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using Steamworks;

public class SteamUtils : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }

    public uint GetSecondsSinceAppActive(IntPtr _)
    {
        Write("GetSecondsSinceAppActive");
        return 0;
    }

    public uint GetSecondsSinceComputerActive(IntPtr _)
    {
        Write("GetSecondsSinceComputerActive");
        return 0;
    }

    public EUniverse GetConnectedUniverse(IntPtr _)
    {
        Write("GetConnectedUniverse");
        return EUniverse.Public;
    }

    public uint GetServerRealTime(IntPtr _)
    {
        Write("GetServerRealTime");
        return 0;
    }

    public string GetIPCountry(IntPtr _)
    {
        Write("GetIPCountry");
        return "";
    }

    public bool GetImageSize(IntPtr _, int iImage, uint pnWidth, uint pnHeight)
    {
        Write("GetImageSize");
        return false;
    }

    public bool GetImageRGBA(IntPtr _, int iImage, uint pubDest, int nDestBufferSize)
    {
        Write("GetImageRGBA");
        return false;
    }

    public bool GetCSERIPPort(IntPtr _, uint unIP, uint usPort)
    {
        Write("GetCSERIPPort");
        return false;
    }

    public uint GetCurrentBatteryPower(IntPtr _)
    {
        Write("GetCurrentBatteryPower");
        return 0;
    }

    public uint GetAppID(IntPtr _)
    {
        Write($"GetAppID {SteamEmulator.AppId}");
        return SteamEmulator.AppId;
    }

    public void SetOverlayNotificationPosition(IntPtr _, ENotificationPosition eNotificationPosition)
    {
        Write("SetOverlayNotificationPosition");
    }

    public bool IsAPICallCompleted(IntPtr _, SteamAPICall_t hSteamAPICall, bool pbFailed)
    {
        Write("IsAPICallCompleted");
        if (hSteamAPICall.m_SteamAPICall == 1)
        {
            if (pbFailed)
                pbFailed = true;
            return true;
        }

        return true;
    }

    public ESteamAPICallFailure GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall)
    {
        Write("GetAPICallFailureReason");
        return default;
    }

    public bool GetAPICallResult(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
    {
        Write("GetAPICallResult");
        return false;
    }

    public uint GetIPCCallCount(IntPtr _)
    {
        Write("GetIPCCallCount");
        return 0;
    }

    public void SetWarningMessageHook(IntPtr _, SteamAPIWarningMessageHook_t pFunction)
    {
        Write("SetWarningMessageHook");
    }

    public bool IsOverlayEnabled(IntPtr _)
    {
        Write("IsOverlayEnabled");
        return false;
    }

    public bool BOverlayNeedsPresent(IntPtr _)
    {
        Write("BOverlayNeedsPresent");
        return false;
    }

    public SteamAPICall_t CheckFileSignature(IntPtr _, string szFileName)
    {
        Write("CheckFileSignature");
        return default;
    }

    public bool ShowGamepadTextInput(IntPtr _, EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
    {
        Write("ShowGamepadTextInput");
        return false;
    }

    public uint GetEnteredGamepadTextLength(IntPtr _)
    {
        Write("GetEnteredGamepadTextLength");
        return 0;
    }

    public bool GetEnteredGamepadTextInput(IntPtr _, string pchText, uint cchText)
    {
        Write("GetEnteredGamepadTextInput");
        return false;
    }

    public string GetSteamUILanguage(IntPtr _)
    {
        Write("GetSteamUILanguage");
        return "";
    }

    public bool IsSteamRunningInVR(IntPtr _)
    {
        Write("IsSteamRunningInVR");
        return false;
    }

    public void SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset)
    {
        Write("SetOverlayNotificationInset");
    }

    public bool IsSteamInBigPictureMode(IntPtr _)
    {
        Write("IsSteamInBigPictureMode");
        return false;
    }

    public void StartVRDashboard(IntPtr _)
    {
        Write("StartVRDashboard");
    }

    public bool IsVRHeadsetStreamingEnabled(IntPtr _)
    {
        Write("IsVRHeadsetStreamingEnabled");
        return false;
    }

    public void SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled)
    {
        Write("SetVRHeadsetStreamingEnabled");
    }

    public bool IsSteamChinaLauncher(IntPtr _)
    {
        Write("IsSteamChinaLauncher");
        return false;
    }

    public bool InitFilterText(IntPtr _)
    {
        Write("InitFilterText");
        return true;
    }

    public int FilterText(IntPtr _, string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
    {
        Write($"FilterText {pchInputMessage}");
        return 0;
    }

    public ESteamIPv6ConnectivityState GetIPv6ConnectivityState(IntPtr _, ESteamIPv6ConnectivityProtocol eProtocol)
    {
        Write("GetIPv6ConnectivityState");
        return default;
    }

    #region New

    public void RunFrame(IntPtr _)
    {
        Write("RunFrame");
    }

    public int GetSteamRealm(IntPtr _)
    {
        Write("GetSteamRealm");
        return 1;
    }

    public uint SetAppIDForCurrentPipe(uint AppId, bool @bool)
    {
        Write("GetSteamRealm");
        return AppId;
    }

    public void RecordSteamInterfaceCreation(string version, string @string)
    {
        Write("RecordSteamInterfaceCreation");
    }

    public uint GetLauncherType(IntPtr _)
    {
        Write("GetLauncherType");
        return 0;
    }
    
    #endregion

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}