using SKYNET;
using SKYNET.Steamworks;
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
        return SteamEmulator.SteamScreenshots.WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamScreenshots_AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight)
    {
        Write("SteamAPI_ISteamScreenshots_AddScreenshotToLibrary");
        return SteamEmulator.SteamScreenshots.AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
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
        return SteamEmulator.SteamScreenshots.SetLocation(hScreenshot, pchLocation);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_TagUser(uint hScreenshot, IntPtr steamID)
    {
        Write("SteamAPI_ISteamScreenshots_TagUser");
        return SteamEmulator.SteamScreenshots.TagUser(hScreenshot, steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_TagPublishedFile(uint hScreenshot, uint unPublishedFileID)
    {
        Write("SteamAPI_ISteamScreenshots_TagPublishedFile");
        return SteamEmulator.SteamScreenshots.TagPublishedFile(hScreenshot, unPublishedFileID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamScreenshots_IsScreenshotsHooked(IntPtr _)
    {
        Write("SteamAPI_ISteamScreenshots_IsScreenshotsHooked");
        return SteamEmulator.SteamScreenshots.IsScreenshotsHooked(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename)
    {
        Write("SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary");
        return SteamEmulator.SteamScreenshots.AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
    }

}
