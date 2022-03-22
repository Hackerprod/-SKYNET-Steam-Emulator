using System;
using Core;
using Core.Interface;
using System.Runtime.InteropServices;


namespace InterfaceUtils
{
    [Impl(Name = "SteamUtils009", ServerMapped = true)]
    public class SteamUtils009 : IBaseInterface
    {
        Utils u;

        public SteamUtils009()
        {
            u = new Utils();
        }

        public int GetSecondsSinceAppActive()
        {
            return u.GetComputerTime();
        }

        public int GetSecondsSinceComputerActive()
        {
            return u.GetComputerTime();
        }

        public uint GetConnectedUniverse()
        {
            return (uint)1;
        }
        public long GetServerRealTime()
        {
            return u.GetComputerTime();
        }

        public string GetIPCountry()
        {
            return "";
        }

        public bool GetImageSize(int image, ref uint width, ref uint height)
        {
            return false;
        }

        public bool GetImageRGBA(int image, IntPtr dest, int total_dest)
        {
            return false;
        }

        public bool GetCSERIPPort(ref uint ip, ref ushort port)
        {
            return false;
        }

        public byte GetCurrentBatteryPower()
        {
            return 255;
        }

        public int GetAppId()
        {
            return (int)SteamEmulator.AppId;
        }

        public void SetOverlayNotificationPosition(uint pos)
        {

        }

        public bool IsAPICallCompleted(int handle, ref bool failed)
        {
            return failed = u.APICalledFinished(handle);
        }

        public int GetAPICallFailureReason(int handle)
        {
            return u.APICallFailureReason(handle);
        }

        public bool GetAPICallResult(int handle, IntPtr callback, int callback_size, int callback_expected, ref bool failed)
        {
            failed = !u.APICallResult(handle, callback, callback_size, callback_expected);

            return !failed;
        }

        public void RunFrame()
        {
            u.RunFrame();
        }

        public uint GetIPCCallCount()
        {
            return 0;
        }

        public void SetWarningMessageHook(IntPtr function)
        {

        }

        public bool IsOverlayEnabled()
        {
            return true;
        }

        public bool OverlayNeedsPresent()
        {
            return false;
        }

        public int CheckFileSignature(string file_name)
        {
            return 0;
        }

        public bool ShowGamePadTextInput(uint input_mode, uint input_line_mode, IntPtr description, uint max_description, string existing_text)
        {
            return false;
        }

        public uint GetEnteredGamepadTextLength()
        {
            return 0;
        }

        public bool GetEnteredGamepadTextInput(string text, int length)
        {
            return false;
        }

        public string GetSteamUILanguage()
        {
            return u.GetUILanguage();
        }

        public bool IsSteamRunningInVR()
        {
            return false;
        }

        public void SetOverlayNotificationInset(int horizontal, int vertical)
        {

        }

        public bool IsSteamInBigPictureMode()
        {
            return false;
        }

        public void StartVRDashboard()
        {

        }

        public bool IsVRHeadsetStreamingEnabled()
        {
            return false;
        }

        public void SetVRHeadsetStreamingEnabled(bool enabled)
        {

        }
    }
}
