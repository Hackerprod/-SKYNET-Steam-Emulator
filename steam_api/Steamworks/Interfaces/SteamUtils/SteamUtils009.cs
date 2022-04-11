using SKYNET.Steamworks;
using Steamworks;
using System;

namespace SKYNET.Interface
{
    [Interface("SteamUtils009")]
    public class SteamUtils009 : ISteamInterface
    {
        public uint GetSecondsSinceAppActive(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive(_);
        }

        public uint GetSecondsSinceComputerActive(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive(_);
        }

        public EUniverse GetConnectedUniverse(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetConnectedUniverse(_);
        }

        public uint GetServerRealTime(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetServerRealTime(_);
        }

        public string GetIPCountry(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetIPCountry(_);
        }

        public bool GetImageSize(IntPtr _, int iImage, uint pnWidth, uint pnHeight)
        {
            return SteamEmulator.SteamUtils.GetImageSize(_, iImage, pnWidth, pnHeight);
        }

        public bool GetImageRGBA(IntPtr _, int iImage, int pubDest, int nDestBufferSize)
        {
            return SteamEmulator.SteamUtils.GetImageRGBA(_, iImage, pubDest, nDestBufferSize);
        }

        public bool GetCSERIPPort(IntPtr _, uint unIP, uint usPort)
        {
            return SteamEmulator.SteamUtils.GetCSERIPPort(_, unIP, usPort);
        }

        public int GetCurrentBatteryPower(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetCurrentBatteryPower(_);
        }

        public uint GetAppID(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetAppID(_);
        }

        public void SetOverlayNotificationPosition(IntPtr _, int eNotificationPosition)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition(_, eNotificationPosition);
        }

        public bool IsAPICallCompleted(IntPtr _, ulong hSteamAPICall, bool pbFailed)
        {
            return SteamEmulator.SteamUtils.IsAPICallCompleted(_, hSteamAPICall, pbFailed);
        }

        public ESteamAPICallFailure GetAPICallFailureReason(IntPtr _, ulong hSteamAPICall)
        {
            return SteamEmulator.SteamUtils.GetAPICallFailureReason(_, hSteamAPICall);
        }

        public bool GetAPICallResult(IntPtr _, ulong hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed)
        {
            return SteamEmulator.SteamUtils.GetAPICallResult(_, hSteamAPICall, pCallback, cubCallback, iCallbackExpected, pbFailed);
        }

        // 	STEAM_PRIVATE_API( virtual void RunFrame() = 0; )

        public uint GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetIPCCallCount(_);
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            SteamEmulator.SteamUtils.SetWarningMessageHook(_, pFunction);
        }

        public bool IsOverlayEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsOverlayEnabled(_);
        }

        public bool BOverlayNeedsPresent(IntPtr _)
        {
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent(_);
        }

        public ulong CheckFileSignature(IntPtr _, string szFileName)
        {
            return SteamEmulator.SteamUtils.CheckFileSignature(_, szFileName);
        }

        public bool ShowGamepadTextInput(IntPtr _, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            return SteamEmulator.SteamUtils.ShowGamepadTextInput(_, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        public uint GetEnteredGamepadTextLength(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength(_);
        }

        public bool GetEnteredGamepadTextInput(IntPtr _, string pchText, uint cchText)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(_, pchText, cchText);
        }

        public string GetSteamUILanguage(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSteamUILanguage(_);
        }

        public bool IsSteamRunningInVR(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamRunningInVR(_);
        }

        public void SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationInset(_, nHorizontalInset, nVerticalInset);
        }

        public bool IsSteamInBigPictureMode(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamInBigPictureMode(_);
        }

        public void StartVRDashboard(IntPtr _)
        {
            SteamEmulator.SteamUtils.StartVRDashboard(_);
        }

        public bool IsVRHeadsetStreamingEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsVRHeadsetStreamingEnabled(_);
        }

        public void SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled)
        {
            SteamEmulator.SteamUtils.SetVRHeadsetStreamingEnabled(_, bEnabled);
        }

        public bool IsSteamChinaLauncher(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamChinaLauncher(_);
        }

        public bool InitFilterText(IntPtr _)
        {
            return SteamEmulator.SteamUtils.InitFilterText(_);
        }

        public int FilterText(IntPtr _, string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly)
        {
            return SteamEmulator.SteamUtils.FilterText(_, pchOutFilteredText, nByteSizeOutFilteredText, pchInputMessage, bLegalOnly);
        }

        public ESteamIPv6ConnectivityState GetIPv6ConnectivityState(IntPtr _, int eProtocol)
        {
            return SteamEmulator.SteamUtils.GetIPv6ConnectivityState(_, eProtocol);
        }


    }
}
