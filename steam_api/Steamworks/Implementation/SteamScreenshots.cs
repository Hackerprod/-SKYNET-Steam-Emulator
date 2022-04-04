
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;

public class SteamScreenshots : SteamInterface
{
    public uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
        Write("WriteScreenshot");
        return 0;
    }

    public uint AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight)
    {
        Write("AddScreenshotToLibrary");
        return 0;
    }

    public void TriggerScreenshot(IntPtr _)
    {
        Write("TriggerScreenshot");
    }

    public void HookScreenshots(bool bHook)
    {
        Write("HookScreenshots");
    }

    public bool SetLocation(uint hScreenshot, char pchLocation)
    {
        Write("SetLocation");
        return false;
    }

    public bool TagUser(uint hScreenshot, IntPtr steamID)
    {
        Write("TagUser");
        return false;
    }

    public bool TagPublishedFile(uint hScreenshot, uint unPublishedFileID)
    {
        Write("TagPublishedFile");
        return false;
    }

    public bool IsScreenshotsHooked(IntPtr _)
    {
        Write("IsScreenshotsHooked");
        return false;
    }

    public uint AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename)
    {
        Write("AddVRScreenshotToLibrary");
        return 0;
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}