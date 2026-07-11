using SKYNET.Steamworks;

using System;
using SKYNET.Helpers;
using System.Runtime.InteropServices;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamUtils010")]
    public class SteamUtils010 : ISteamInterface
    {
        public uint GetSecondsSinceAppActive(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive();
        }

        public uint GetSecondsSinceComputerActive(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive();
        }

        public int GetConnectedUniverse(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetConnectedUniverse();
        }

        public uint GetServerRealTime(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetServerRealTime();
        }

        public IntPtr GetIPCountry(IntPtr _)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUtils.GetIPCountry());
        }

        public bool GetImageSize(IntPtr _, int iImage, ref uint pnWidth, ref uint pnHeight)
        {
            return SteamEmulator.SteamUtils.GetImageSize(iImage, ref pnWidth, ref pnHeight);
        }

        public bool GetImageRGBA(IntPtr _, int iImage, IntPtr pubDest, int nDestBufferSize)
        {
            return SteamEmulator.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);
        }

        public bool GetCSERIPPort(IntPtr _, uint unIP, uint usPort)
        {
            return SteamEmulator.SteamUtils.GetCSERIPPort(unIP, usPort);
        }

        public int GetCurrentBatteryPower(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetCurrentBatteryPower();
        }

        public uint GetAppID(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetAppID();
        }

        public void SetOverlayNotificationPosition(IntPtr _, int eNotificationPosition)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition(eNotificationPosition);
        }

        public bool IsAPICallCompleted(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pbFailed)
        {
            bool failed = false;
            bool result = SteamEmulator.SteamUtils.IsAPICallCompleted(hSteamAPICall, ref failed);
            if (pbFailed != IntPtr.Zero)
            {
                Marshal.WriteByte(pbFailed, failed ? (byte)1 : (byte)0);
            }
            return result;
        }

        public int GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall)
        {
            return SteamEmulator.SteamUtils.GetAPICallFailureReason(hSteamAPICall);
        }

        public bool GetAPICallResult(IntPtr _, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
        {
            return SteamEmulator.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
        }

        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamUtils.RunFrame();
        }

        public uint GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetIPCCallCount();
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            SteamEmulator.SteamUtils.SetWarningMessageHook(pFunction);
        }

        public bool IsOverlayEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsOverlayEnabled();
        }

        public bool BOverlayNeedsPresent(IntPtr _)
        {
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent();
        }

        public SteamAPICall_t CheckFileSignature(IntPtr _, string szFileName)
        {
            return SteamEmulator.SteamUtils.CheckFileSignature(szFileName);
        }

        public bool ShowGamepadTextInput(IntPtr _, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            return SteamEmulator.SteamUtils.ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        public uint GetEnteredGamepadTextLength(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength();
        }

        public bool GetEnteredGamepadTextInput(IntPtr _, IntPtr pchText, uint cchText)
        {
            NativeStringCache.WriteUtf8Buffer(pchText, checked((int)cchText), string.Empty);
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(string.Empty, cchText);
        }

        public IntPtr GetSteamUILanguage(IntPtr _)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamUtils.GetSteamUILanguage());
        }

        public bool IsSteamRunningInVR(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamRunningInVR();
        }

        public void SetOverlayNotificationInset(IntPtr _, int nHorizontalInset, int nVerticalInset)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationInset(nHorizontalInset, nVerticalInset);
        }

        public bool IsSteamInBigPictureMode(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamInBigPictureMode();
        }

        public void StartVRDashboard(IntPtr _)
        {
            SteamEmulator.SteamUtils.StartVRDashboard();
        }

        public bool IsVRHeadsetStreamingEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsVRHeadsetStreamingEnabled();
        }

        public void SetVRHeadsetStreamingEnabled(IntPtr _, bool bEnabled)
        {
            SteamEmulator.SteamUtils.SetVRHeadsetStreamingEnabled(bEnabled);
        }

        public bool IsSteamChinaLauncher(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamChinaLauncher();
        }

        public bool InitFilterText(IntPtr _, uint unFilterOptions)
        {
            return SteamEmulator.SteamUtils.InitFilterText();
        }

        public int FilterText(IntPtr _, int eContext, ulong sourceSteamID, string pchInputMessage, IntPtr pchOutFilteredText, uint nByteSizeOutFilteredText )
        {
            NativeStringCache.WriteUtf8Buffer(pchOutFilteredText, checked((int)nByteSizeOutFilteredText), pchInputMessage);
            return SteamEmulator.SteamUtils.FilterText(eContext, sourceSteamID, pchInputMessage, pchInputMessage, nByteSizeOutFilteredText);
        }

        public int GetIPv6ConnectivityState(IntPtr _, int eProtocol)
        {
            return SteamEmulator.SteamUtils.GetIPv6ConnectivityState(eProtocol);
        }

        public bool IsSteamRunningOnSteamDeck(IntPtr _)
        {
            return false;
        }

        public bool ShowFloatingGamepadTextInput(IntPtr _, int eKeyboardMode, int nTextFieldXPosition, int nTextFieldYPosition, int nTextFieldWidth, int nTextFieldHeight)
        {
            return false;
        }

        public void SetGameLauncherMode(IntPtr _, bool bLauncherMode)
        {
        }

        public bool DismissFloatingGamepadTextInput(IntPtr _)
        {
            return false;
        }

        public bool DismissGamepadTextInput(IntPtr _)
        {
            return false;
        }
    }
}
