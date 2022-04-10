using SKYNET.Steamworks;
using Steamworks;
using System;

namespace SKYNET.Interface
{
    [Interface("SteamUtils009")]
    public class SteamUtils009 : ISteamInterface
    {
        public int GetSecondsSinceAppActive(IntPtr _)
        {
            return (int)SteamEmulator.SteamUtils.GetSecondsSinceAppActive(_);
        }
        public int GetSecondsSinceComputerActive(IntPtr _)
        {
            return (int)SteamEmulator.SteamUtils.GetSecondsSinceComputerActive(_);
        }
        public uint GetConnectedUniverse(IntPtr _)
        {
            return (uint)SteamEmulator.SteamUtils.GetConnectedUniverse(_);
        }
        public long GetServerRealTime(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetServerRealTime(_);
        }
        public string GetIPCountry(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetIPCountry(_);
        }
        public bool GetImageSize(IntPtr _, int image, ref uint width, ref uint height)
        {
            return SteamEmulator.SteamUtils.GetImageSize(_, image, width, height);
        }
        public bool GetImageRGBA(IntPtr _, int image, IntPtr dest, int total_dest)
        {
            return SteamEmulator.SteamUtils.GetImageRGBA(_, image, (uint)dest, total_dest);
        }
        public bool GetCSERIPPort(IntPtr _, ref uint ip, ref short port)
        {
            return SteamEmulator.SteamUtils.GetCSERIPPort(_, ip, (uint)port);
        }
        public System.Byte GetCurrentBatteryPower(IntPtr _)
        {
            return (Byte)SteamEmulator.SteamUtils.GetCurrentBatteryPower(_);
        }
        public int GetAppId(IntPtr _)
        {
            return (int)SteamEmulator.SteamUtils.GetAppID(_);
        }
        public void SetOverlayNotificationPosition(IntPtr _, uint pos)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition(_, (ENotificationPosition)pos);
        }
        public bool IsAPICallCompleted(IntPtr _, int handle, ref bool failed)
        {
            return SteamEmulator.SteamUtils.IsAPICallCompleted(_, handle, failed);
        }
        public int GetAPICallFailureReason(IntPtr _, int handle)
        {
            return (int)SteamEmulator.SteamUtils.GetAPICallFailureReason(_, handle);
        }
        public bool GetAPICallResult(IntPtr _, int handle, IntPtr callback, int callback_size, int callback_expected, ref bool failed)
        {
            return SteamEmulator.SteamUtils.GetAPICallResult(_, handle, callback, callback_size, callback_expected, failed);
        }
        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamUtils.RunFrame(_);
        }
        public uint GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetIPCCallCount(_);
        }
        public void SetWarningMessageHook(IntPtr _, IntPtr function)
        {
            SteamEmulator.SteamUtils.SetWarningMessageHook(_, default);
        }
        public bool IsOverlayEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsOverlayEnabled(_);
        }
        public bool OverlayNeedsPresent(IntPtr _)
        {
            //return SteamEmulator.SteamUtils.OverlayNeedsPresent(_);
            return false;
        }
        public int CheckFileSignature(IntPtr _, string file_name)
        {
            return SteamEmulator.SteamUtils.CheckFileSignature(_, file_name);
        }
        public bool ShowGamePadTextInput(IntPtr _, uint input_mode, uint input_line_mode, IntPtr description, uint max_description, string existing_text)
        {
            //return SteamEmulator.SteamUtils.ShowGamePadTextInput(_);
            return false;
        }
        public uint GetEnteredGamepadTextLength(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength(_);
        }
        public bool GetEnteredGamepadTextInput(IntPtr _, string text, int length)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(_, text, (uint)length);
        }
        public string GetSteamUILanguage(IntPtr _)
        {
            return SteamEmulator.SteamUtils.GetSteamUILanguage(_);
        }
        public bool IsSteamRunningInVR(IntPtr _)
        {
            return SteamEmulator.SteamUtils.IsSteamRunningInVR(_);
        }
        public void SetOverlayNotificationInset(IntPtr _, int horizontal, int vertical)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationInset(_, horizontal, vertical);
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
        public void SetVRHeadsetStreamingEnabled(IntPtr _, bool enabled)
        {
            SteamEmulator.SteamUtils.SetVRHeadsetStreamingEnabled(_, enabled);
        }
    }
}
