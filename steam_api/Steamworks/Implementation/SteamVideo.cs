using SKYNET;
using SKYNET.Helpers;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamVideo : ISteamInterface
    {
        public void GetVideoURL(uint unVideoAppID)
        {
            Write($"GetVideoURL");
        }

        public bool IsBroadcasting(int pnNumViewers)
        {
            Write($"IsBroadcasting");
            return false;
        }

        public void GetOPFSettings(uint unVideoAppID)
        {
            Write($"GetOPFSettings");
        }

        public bool GetOPFStringForApp(uint unVideoAppID, string pchBuffer, int pnBufferSize)
        {
            Write($"GetOPFStringForApp");
            return false;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamVideo()
        {
            InterfaceVersion = "SteamVideo";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
