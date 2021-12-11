using System;

namespace SKYNET.Interface
{
    public interface ISteamUtils
    {
        IntPtr GetSecondsSinceAppActive();
        IntPtr GetSecondsSinceComputerActive();
        IntPtr GetConnectedUniverse();
        IntPtr GetServerRealTime();
        IntPtr GetIPCountry();
        IntPtr GetImageSize();
        IntPtr GetImageRGBA();
        IntPtr GetCSERIPPort();
        IntPtr GetCurrentBatteryPower();
        IntPtr GetAppID();
        IntPtr SetOverlayNotificationPosition();
        IntPtr IsAPICallCompleted();
        IntPtr GetAPICallFailureReason();
        IntPtr GetAPICallResult();
        IntPtr RunFrame();
        IntPtr GetIPCCallCount();
        void SetWarningMessageHook();
        IntPtr IsOverlayEnabled();
        IntPtr OverlayNeedsPresent();
    }
}