using System;

namespace SKYNET.Interface
{
    [Interface("STEAMSCREENSHOTS_INTERFACE_VERSION003")]
    public class SteamScreenshots003 : ISteamInterface
    {
        public uint WriteScreenshot(IntPtr _, IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
        {
            return SteamEmulator.SteamScreenshots.WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
        }

        public uint AddScreenshotToLibrary(IntPtr _, string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
        {
            return SteamEmulator.SteamScreenshots.AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
        }

        public void TriggerScreenshot(IntPtr _)
        {
            SteamEmulator.SteamScreenshots.TriggerScreenshot();
        }

        public void HookScreenshots(IntPtr _, bool bHook)
        {
            SteamEmulator.SteamScreenshots.HookScreenshots(bHook);
        }

        public bool SetLocation(IntPtr _, uint hScreenshot, string pchLocation)
        {
            return SteamEmulator.SteamScreenshots.SetLocation(hScreenshot, pchLocation);
        }

        public bool TagUser(IntPtr _, uint hScreenshot, ulong steamID)
        {
            return SteamEmulator.SteamScreenshots.TagUser(hScreenshot, steamID);
        }

        public bool TagPublishedFile(IntPtr _, uint hScreenshot, ulong unPublishedFileID)
        {
            return SteamEmulator.SteamScreenshots.TagPublishedFile(hScreenshot, unPublishedFileID);
        }

        public bool IsScreenshotsHooked(IntPtr _)
        {
            return SteamEmulator.SteamScreenshots.IsScreenshotsHooked();
        }

        public uint AddVRScreenshotToLibrary(IntPtr _, int eType, string pchFilename, string pchVRFilename)
        {
            return SteamEmulator.SteamScreenshots.AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
        }

    }
}
