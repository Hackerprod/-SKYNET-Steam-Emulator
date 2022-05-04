using SKYNET;
using SKYNET.Helpers;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamTV : ISteamInterface
    {
        public SteamTV()
        {
            InterfaceName = "SteamTV";
        }

        public bool IsBroadcasting(int pnNumViewers)
        {
            Write($"IsBroadcasting");
            return false;
        }

        public void AddBroadcastGameData(string pchKey, string pchValue)
        {
            Write($"AddBroadcastGameData");
        }

        public void RemoveBroadcastGameData(string pchKey)
        {
            Write($"RemoveBroadcastGameData");
        }

        public void AddTimelineMarker(string pchTemplateName, bool bPersistent, uint nColorR, uint nColorG, uint nColorB)
        {
            Write($"AddTimelineMarker");
        }

        public void RemoveTimelineMarker()
        {
            Write($"RemoveTimelineMarker");
        }

        public uint AddRegion(string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior)
        {
            Write($"AddRegion");
            return 0;
        }

        public void RemoveRegion(uint unRegionHandle)
        {
            Write($"RemoveRegion");
        }
    }
}