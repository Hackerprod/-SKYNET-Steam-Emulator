using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMTV_INTERFACE_V001",
        "IsBroadcasting", "AddBroadcastGameData", "RemoveBroadcastGameData", "AddTimelineMarker",
        "RemoveTimelineMarker", "AddRegion", "RemoveRegion")]
    public class SteamTV001 : ISteamInterface
    {
        public bool IsBroadcasting(IntPtr _, IntPtr pnNumViewers)
        {
            if (pnNumViewers != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnNumViewers, 0);
            }

            return SteamEmulator.SteamTV.IsBroadcasting(0);
        }

        public void AddBroadcastGameData(IntPtr _, string pchKey, string pchValue)
        {
            SteamEmulator.SteamTV.AddBroadcastGameData(pchKey, pchValue);
        }

        public void RemoveBroadcastGameData(IntPtr _, string pchKey)
        {
            SteamEmulator.SteamTV.RemoveBroadcastGameData(pchKey);
        }

        public void AddTimelineMarker(IntPtr _, string pchTemplateName, bool bPersistent, byte nColorR, byte nColorG, byte nColorB)
        {
            SteamEmulator.SteamTV.AddTimelineMarker(pchTemplateName, bPersistent, nColorR, nColorG, nColorB);
        }

        public void RemoveTimelineMarker(IntPtr _)
        {
            SteamEmulator.SteamTV.RemoveTimelineMarker();
        }

        public uint AddRegion(IntPtr _, string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior)
        {
            return SteamEmulator.SteamTV.AddRegion(pchElementName, pchTimelineDataSection, pSteamTVRegion, eSteamTVRegionBehavior);
        }

        public void RemoveRegion(IntPtr _, uint unRegionHandle)
        {
            SteamEmulator.SteamTV.RemoveRegion(unRegionHandle);
        }
    }
}
