using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    public class SteamScreenshots : ISteamScreenshots
    {
        public uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
        {
            return 0;
        }

        public uint AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight)
        {
            return 0;
        }

        public void TriggerScreenshot()
        {
            //
        }

        public void HookScreenshots(bool bHook)
        {
            //
        }

        public bool SetLocation(uint hScreenshot, char pchLocation)
        {
            return false;
        }

        public bool TagUser(uint hScreenshot, IntPtr steamID)
        {
            return false;
        }

        public bool TagPublishedFile(uint hScreenshot, uint unPublishedFileID)
        {
            return false;
        }

        public bool IsScreenshotsHooked()
        {
            return false;
        }

        public uint AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename)
        {
            return 0;
        }

    }

}