using SKYNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamTV : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamTV_IsBroadcasting(int pnNumViewers)
        {
            Write("SteamAPI_ISteamTV_IsBroadcasting");
            return SteamEmulator.SteamTV.IsBroadcasting(pnNumViewers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_AddBroadcastGameData(string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamTV_AddBroadcastGameData");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveBroadcastGameData(string pchKey)
        {
            Write("SteamAPI_ISteamTV_RemoveBroadcastGameData");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_AddTimelineMarker(string pchTemplateName, bool bPersistent, uint nColorR, uint nColorG, uint nColorB)
        {
            Write("SteamAPI_ISteamTV_AddTimelineMarker");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveTimelineMarker()
        {
            Write("SteamAPI_ISteamTV_RemoveTimelineMarker");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamTV_AddRegion(string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior)
        {
            Write("SteamAPI_ISteamTV_AddRegion");
            return SteamEmulator.SteamTV.AddRegion(pchElementName, pchTimelineDataSection, pSteamTVRegion, eSteamTVRegionBehavior);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTV_RemoveRegion(uint unRegionHandle)
        {
            Write("SteamAPI_ISteamTV_RemoveRegion");
        }
    }
}

