using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamUtils ABI used by the original Left 4 Dead SDK.</summary>
    [Interface("SteamUtils004")]
    public class SteamUtils004 : ISteamInterface
    {
        public uint GetSecondsSinceAppActive(IntPtr _) => SteamEmulator.SteamUtils.GetSecondsSinceAppActive();
        public uint GetSecondsSinceComputerActive(IntPtr _) => SteamEmulator.SteamUtils.GetSecondsSinceComputerActive();
        public int GetConnectedUniverse(IntPtr _) => SteamEmulator.SteamUtils.GetConnectedUniverse();
        public uint GetServerRealTime(IntPtr _) => SteamEmulator.SteamUtils.GetServerRealTime();
        public string GetIPCountry(IntPtr _) => SteamEmulator.SteamUtils.GetIPCountry();

        public bool GetImageSize(IntPtr _, int iImage, ref uint pnWidth, ref uint pnHeight) =>
            SteamEmulator.SteamUtils.GetImageSize(iImage, ref pnWidth, ref pnHeight);

        public bool GetImageRGBA(IntPtr _, int iImage, IntPtr pubDest, int nDestBufferSize) =>
            SteamEmulator.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);

        public bool GetCSERIPPort(IntPtr _, IntPtr unIP, IntPtr usPort) =>
            SteamEmulator.SteamUtils.GetCSERIPPort(unIP, usPort);

        public byte GetCurrentBatteryPower(IntPtr _) => SteamEmulator.SteamUtils.GetCurrentBatteryPower();
        public uint GetAppID(IntPtr _) => SteamEmulator.SteamUtils.GetAppID();

        public void SetOverlayNotificationPosition(IntPtr _, int eNotificationPosition) =>
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition(eNotificationPosition);

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

        public int GetAPICallFailureReason(IntPtr _, SteamAPICall_t hSteamAPICall) =>
            SteamEmulator.SteamUtils.GetAPICallFailureReason(hSteamAPICall);

        public bool GetAPICallResult(
            IntPtr _,
            SteamAPICall_t hSteamAPICall,
            IntPtr pCallback,
            int cubCallback,
            int iCallbackExpected,
            IntPtr pbFailed)
        {
            bool failed = false;
            bool result = SteamEmulator.SteamUtils.GetAPICallResult(
                hSteamAPICall,
                pCallback,
                cubCallback,
                iCallbackExpected,
                ref failed);
            if (pbFailed != IntPtr.Zero)
            {
                Marshal.WriteByte(pbFailed, failed ? (byte)1 : (byte)0);
            }
            return result;
        }

        public void RunFrame(IntPtr _) => SteamEmulator.SteamUtils.RunFrame();
        public uint GetIPCCallCount(IntPtr _) => SteamEmulator.SteamUtils.GetIPCCallCount();

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction) =>
            SteamEmulator.SteamUtils.SetWarningMessageHook(pFunction);

        public bool IsOverlayEnabled(IntPtr _) => SteamEmulator.SteamUtils.IsOverlayEnabled();
    }
}
