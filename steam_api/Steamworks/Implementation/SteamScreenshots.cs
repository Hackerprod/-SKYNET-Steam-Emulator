using Core.Interface;
using SKYNET;

using SKYNET.Steamworks;
using System;

//[Map("STEAMSCREENSHOTS_INTERFACE_VERSION")]
//[Map("SteamScreenshots")]
public class SteamScreenshots : IBaseInterface
{
    public uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
        return 0;
    }

    public uint AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight)
    {
        return 0;
    }

    public void TriggerScreenshot(IntPtr _)
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

    public bool IsScreenshotsHooked(IntPtr _)
    {
        return false;
    }

    public uint AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename)
    {
        return 0;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}