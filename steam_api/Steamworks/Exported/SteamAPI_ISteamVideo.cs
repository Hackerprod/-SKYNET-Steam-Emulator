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

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
