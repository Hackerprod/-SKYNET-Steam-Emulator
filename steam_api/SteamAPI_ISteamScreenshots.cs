using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamScreenshots : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamScreenshots_WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
    {
        Write("SteamAPI_ISteamScreenshots_WriteScreenshot");
        return SteamClient.SteamScreenshots.WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamScreenshots_AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight)
    {
        Write("SteamAPI_ISteamScreenshots_AddScreenshotToLibrary");
        return SteamClient.SteamScreenshots.AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamScreenshots_TriggerScreenshot()
    {
        Write("SteamAPI_ISteamScreenshots_TriggerScreenshot");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamScreenshots_HookScreenshots(bool bHook)
    {
        Write("SteamAPI_ISteamScreenshots_HookScreenshots");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_SetLocation(uint hScreenshot, char pchLocation)
    {
        Write("SteamAPI_ISteamScreenshots_SetLocation");
        return SteamClient.SteamScreenshots.SetLocation(hScreenshot, pchLocation);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_TagUser(uint hScreenshot, IntPtr steamID)
    {
        Write("SteamAPI_ISteamScreenshots_TagUser");
        return SteamClient.SteamScreenshots.TagUser(hScreenshot, steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_TagPublishedFile(uint hScreenshot, uint unPublishedFileID)
    {
        Write("SteamAPI_ISteamScreenshots_TagPublishedFile");
        return SteamClient.SteamScreenshots.TagPublishedFile(hScreenshot, unPublishedFileID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_IsScreenshotsHooked()
    {
        Write("SteamAPI_ISteamScreenshots_IsScreenshotsHooked");
        return SteamClient.SteamScreenshots.IsScreenshotsHooked();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename)
    {
        Write("SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary");
        return SteamClient.SteamScreenshots.AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
    }

}
