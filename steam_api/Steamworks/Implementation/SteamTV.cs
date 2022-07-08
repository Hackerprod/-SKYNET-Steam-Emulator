using System;
using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamTV : ISteamInterface
    {
        public static SteamTV Instance;

        public SteamTV()
        {
            Instance = this;
            InterfaceName = "SteamTV";
            InterfaceVersion = "STEAMTV_INTERFACE_V002";
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