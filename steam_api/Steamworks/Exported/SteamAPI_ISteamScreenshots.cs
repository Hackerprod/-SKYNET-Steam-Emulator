using SKYNET.Managers;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamScreenshots
    {
        static SteamAPI_ISteamScreenshots()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamScreenshots_WriteScreenshot(IntPtr _, IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
        {
            Write("SteamAPI_ISteamScreenshots_WriteScreenshot");
            return SteamEmulator.SteamScreenshots.WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamScreenshots_AddScreenshotToLibrary(IntPtr _, string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
        {
            Write("SteamAPI_ISteamScreenshots_AddScreenshotToLibrary");
            return SteamEmulator.SteamScreenshots.AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamScreenshots_TriggerScreenshot(IntPtr _)
        {
            Write("SteamAPI_ISteamScreenshots_TriggerScreenshot");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamScreenshots_HookScreenshots(IntPtr _, bool bHook)
        {
            Write("SteamAPI_ISteamScreenshots_HookScreenshots");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamScreenshots_SetLocation(IntPtr _, uint hScreenshot, string pchLocation)
        {
            Write("SteamAPI_ISteamScreenshots_SetLocation");
            return SteamEmulator.SteamScreenshots.SetLocation(hScreenshot, pchLocation);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamScreenshots_TagUser(IntPtr _, uint hScreenshot, ulong steamID)
        {
            Write("SteamAPI_ISteamScreenshots_TagUser");
            return SteamEmulator.SteamScreenshots.TagUser(hScreenshot, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamScreenshots_TagPublishedFile(IntPtr _, uint hScreenshot, uint unPublishedFileID)
        {
            Write("SteamAPI_ISteamScreenshots_TagPublishedFile");
            return SteamEmulator.SteamScreenshots.TagPublishedFile(hScreenshot, unPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamScreenshots_IsScreenshotsHooked(IntPtr _)
        {
            Write("SteamAPI_ISteamScreenshots_IsScreenshotsHooked");
            return SteamEmulator.SteamScreenshots.IsScreenshotsHooked();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary(IntPtr _, int eType, string pchFilename, string pchVRFilename)
        {
            Write("SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary");
            return SteamEmulator.SteamScreenshots.AddVRScreenshotToLibrary(eType, pchFilename, pchVRFilename);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamScreenshots_v003()
        {
            Write("SteamAPI_SteamScreenshots_v003");
            return InterfaceManager.FindOrCreateInterface("STEAMSCREENSHOTS_INTERFACE_VERSION003");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
