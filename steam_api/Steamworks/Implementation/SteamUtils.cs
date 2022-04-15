using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUtils : ISteamInterface
    {
        public SteamUtils()
        {
            InterfaceVersion = "SteamUtils";
        }

        public uint GetSecondsSinceAppActive()
        {
            Write("GetSecondsSinceAppActive");
            return 0;
        }

        public uint GetSecondsSinceComputerActive()
        {
            Write("GetSecondsSinceComputerActive");
            return 0;
        }

        public EUniverse GetConnectedUniverse()
        {
            Write("GetConnectedUniverse");
            return EUniverse.k_EUniversePublic;
        }

        public uint GetServerRealTime()
        {
            Write("GetServerRealTime");
            return (uint)DateTime.Now.Millisecond;
        }

        public string GetIPCountry()
        {
            Write("GetIPCountry");
            return "";
        }

        public bool GetImageSize(int iImage, uint pnWidth, uint pnHeight)
        {
            Write("GetImageSize");
            return false;
        }

        public bool GetImageRGBA(int iImage, int pubDest, int nDestBufferSize)
        {
            Write("GetImageRGBA");
            return false;
        }

        public bool GetCSERIPPort(uint unIP, uint usPort)
        {
            Write("GetCSERIPPort");
            return false;
        }

        public int GetCurrentBatteryPower()
        {
            Write("GetCurrentBatteryPower");
            return 0;
        }

        public uint GetAppID()
        {
            uint appId = SteamEmulator.AppId == 0 ? 570 : SteamEmulator.AppId;
            Write($"GetAppID {appId}");
            return appId;
        }

        public void SetOverlayNotificationPosition(int eNotificationPosition)
        {
            Write("SetOverlayNotificationPosition");
        }

        public bool IsAPICallCompleted(ulong hSteamAPICall, bool pbFailed)
        {
            Write("IsAPICallCompleted");
            if (hSteamAPICall == 1)
            {
                if (pbFailed)
                    pbFailed = true;
                return true;
            }

            return true;
        }

        public ESteamAPICallFailure GetAPICallFailureReason(ulong hSteamAPICall)
        {
            Write("GetAPICallFailureReason");
            return default;
        }

        public bool GetAPICallResult(ulong hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
        {
            Write("GetAPICallResult");
            return false;
        }

        public void RunFrame()
        {
            Write("RunFrame");
        }

        public uint GetIPCCallCount()
        {
            Write("GetIPCCallCount");
            return 0;
        }

        public void SetWarningMessageHook(IntPtr pFunction)
        {
            Write("SetWarningMessageHook");
        }

        public bool IsOverlayEnabled()
        {
            Write("IsOverlayEnabled");
            return false;
        }

        public bool BOverlayNeedsPresent()
        {
            Write("BOverlayNeedsPresent");
            return false;
        }

        public ulong CheckFileSignature(string szFileName)
        {
            Write("CheckFileSignature");
            return default;
        }

        public bool ShowGamepadTextInput(int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            Write("ShowGamepadTextInput");
            return false;
        }

        public uint GetEnteredGamepadTextLength()
        {
            Write("GetEnteredGamepadTextLength");
            return 0;
        }

        public bool GetEnteredGamepadTextInput(string pchText, uint cchText)
        {
            Write("GetEnteredGamepadTextInput");
            return false;
        }

        public string GetSteamUILanguage()
        {
            Write("GetSteamUILanguage");
            return "English";
        }

        public bool IsSteamRunningInVR()
        {
            Write("IsSteamRunningInVR");
            return false;
        }

        public void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
        {
            Write("SetOverlayNotificationInset");
        }

        public bool IsSteamInBigPictureMode()
        {
            Write("IsSteamInBigPictureMode");
            return false;
        }

        public void StartVRDashboard()
        {
            Write("StartVRDashboard");
        }

        public bool IsVRHeadsetStreamingEnabled()
        {
            Write("IsVRHeadsetStreamingEnabled");
            return false;
        }

        public void SetVRHeadsetStreamingEnabled(bool bEnabled)
        {
            Write("SetVRHeadsetStreamingEnabled");
        }

        public bool IsSteamChinaLauncher()
        {
            Write("IsSteamChinaLauncher");
            return false;
        }

        public bool InitFilterText()
        {
            Write("InitFilterText");
            return true;
        }

        public int FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
        {
            Write($"FilterText {pchInputMessage}");
            return 0;
        }

        public int FilterText(int eContext, ulong sourceSteamID, string pchInputMessage, string pchOutFilteredText, uint nByteSizeOutFilteredText)
        {
            Write($"FilterText {pchInputMessage}");
            return 0;
        }

        public ESteamIPv6ConnectivityState GetIPv6ConnectivityState(int eProtocol)
        {
            Write("GetIPv6ConnectivityState");
            return default;
        }
    }
}