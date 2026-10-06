using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamVideo
    {
        static SteamAPI_ISteamVideo()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_GetVideoURL(IntPtr _, uint unVideoAppID)
        {
            Write("SteamAPI_ISteamVideo_GetVideoURL");
            SteamEmulator.SteamVideo.GetVideoURL(unVideoAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamVideo_IsBroadcasting(IntPtr _, IntPtr pnNumViewers)
        {
            Write("SteamAPI_ISteamVideo_IsBroadcasting");
            return SteamEmulator.SteamVideo.IsBroadcasting(pnNumViewers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_GetOPFSettings(IntPtr _, uint unVideoAppID)
        {
            Write("SteamAPI_ISteamVideo_GetOPFSettings");
            SteamEmulator.SteamVideo.GetOPFSettings(unVideoAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamVideo_GetOPFStringForApp(IntPtr _, uint unVideoAppID, IntPtr pchBuffer, IntPtr pnBufferSize)
        {
            Write("SteamAPI_ISteamVideo_GetOPFStringForApp");
            return SteamEmulator.SteamVideo.GetOPFStringForApp(unVideoAppID, pchBuffer, pnBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_AddTimelineHighlightMarker(IntPtr _, string pchIcon, string pchTitle, string pchDescription, uint unPriority)
        {
            Write("SteamAPI_ISteamVideo_AddTimelineHighlightMarker");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_AddTimelineTimestamp(IntPtr _, string pchTitle)
        {
            Write("SteamAPI_ISteamVideo_AddTimelineTimestamp");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_AddTimelineRangeStart(IntPtr _, string pchID, string pchTitle)
        {
            Write("SteamAPI_ISteamVideo_AddTimelineRangeStart");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_AddTimelineRangeEnd(IntPtr _, string pchID)
        {
            Write("SteamAPI_ISteamVideo_AddTimelineRangeEnd");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamVideo_SetTimelineGameMode(IntPtr _, int eMode)
        {
            Write("SteamAPI_ISteamVideo_SetTimelineGameMode");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
