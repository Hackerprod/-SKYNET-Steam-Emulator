using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamTV
    {
        static SteamAPI_ISteamTV()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamTV_IsBroadcasting(IntPtr _, IntPtr pnNumViewers)
        {
            Write("SteamAPI_ISteamTV_IsBroadcasting");
            if (pnNumViewers != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnNumViewers, 0);
            }

            return SteamEmulator.SteamTV.IsBroadcasting(0);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_AddBroadcastGameData(IntPtr _, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamTV_AddBroadcastGameData");
            SteamEmulator.SteamTV.AddBroadcastGameData(pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveBroadcastGameData(IntPtr _, string pchKey)
        {
            Write("SteamAPI_ISteamTV_RemoveBroadcastGameData");
            SteamEmulator.SteamTV.RemoveBroadcastGameData(pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_AddTimelineMarker(IntPtr _, string pchTemplateName, bool bPersistent, byte nColorR, byte nColorG, byte nColorB)
        {
            Write("SteamAPI_ISteamTV_AddTimelineMarker");
            SteamEmulator.SteamTV.AddTimelineMarker(pchTemplateName, bPersistent, nColorR, nColorG, nColorB);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveTimelineMarker(IntPtr _)
        {
            Write("SteamAPI_ISteamTV_RemoveTimelineMarker");
            SteamEmulator.SteamTV.RemoveTimelineMarker();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamTV_AddRegion(IntPtr _, string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior)
        {
            Write("SteamAPI_ISteamTV_AddRegion");
            return SteamEmulator.SteamTV.AddRegion(pchElementName, pchTimelineDataSection, pSteamTVRegion, eSteamTVRegionBehavior);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveRegion(IntPtr _, uint unRegionHandle)
        {
            Write("SteamAPI_ISteamTV_RemoveRegion");
            SteamEmulator.SteamTV.RemoveRegion(unRegionHandle);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
