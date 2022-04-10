using SKYNET;
using SKYNET.Helpers;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamTV : ISteamInterface
    {
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

        public void RemoveTimelineMarker(IntPtr _)
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

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamTV()
        {
            InterfaceVersion = "SteamTV";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}