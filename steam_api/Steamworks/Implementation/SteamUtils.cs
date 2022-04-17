using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUtils : ISteamInterface
    {
        private DateTime ActiveTime;
        public SteamUtils()
        {
            InterfaceVersion = "SteamUtils";
            ActiveTime = DateTime.Now;
        }

        public uint GetSecondsSinceAppActive()
        {
            Write("GetSecondsSinceAppActive");
            return (uint)(DateTime.Now - ActiveTime).Seconds;
        }

        public uint GetSecondsSinceComputerActive()
        {
            Write("GetSecondsSinceComputerActive");
            return (uint)(DateTime.Now - ActiveTime).Seconds + 3000;
        }

        public EUniverse GetConnectedUniverse()
        {
            Write("GetConnectedUniverse");
            return EUniverse.k_EUniversePublic;
        }

        public uint GetServerRealTime()
        {
            uint ServerTime = (uint)(new DateTimeOffset(DateTime.Now)).ToUnixTimeSeconds(); 
            Write($"GetServerRealTime {ServerTime}");
            return ServerTime;
        }

        public string GetIPCountry()
        {
            Write("GetIPCountry");
            return "US";
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
            return 100;
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

        public bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
        {
            Write("IsAPICallCompleted");
            if (hSteamAPICall == 1)
            {
                if (pbFailed)
                    pbFailed = true;
                return true;
            }

            if (CallbackManager.Contains(hSteamAPICall))
                return false;

            if (pbFailed) pbFailed = false;
            return true;
        }

        public ESteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
        {
            Write("GetAPICallFailureReason");
            return ESteamAPICallFailure.k_ESteamAPICallFailureNone;
        }

        public bool GetAPICallResult(ulong handle, IntPtr callback, int callback_size, int callback_expected, ref bool failed)
        {
            try
            {
                Write("GetAPICallResult");
                var result = CallbackManager.GetCallResult(handle, callback, callback_size, callback_expected);

                if (result == null)
                {
                    failed = true;
                    return false;
                }

                Marshal.Copy(result, 0, callback, callback_size);
                return !failed;
            }
            catch (Exception ex)
            {
                Write($"GetAPICallResult {ex}");
            }
            return false;
        }

        public void RunFrame()
        {
            Write("RunFrame");
        }

        public uint GetIPCCallCount()
        {
            Write("GetIPCCallCount");
            return (uint)new Random().Next(100, 200);
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

        public SteamAPICall_t CheckFileSignature(string szFileName)
        {
            Write("CheckFileSignature");
            // CheckFileSignature_t
            return 0;
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
            return SteamEmulator.Language;
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
            return false;
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
            return ESteamIPv6ConnectivityState.k_ESteamIPv6ConnectivityState_Unknown;
        }
    }
}