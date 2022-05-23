using SKYNET;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamVideo : ISteamInterface
    {
        public static SteamVideo Instance;

        public SteamVideo()
        {
            Instance = this;
            InterfaceName = "SteamVideo";
            InterfaceVersion = "STEAMVIDEO_INTERFACE_V002";
        }

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
    }
}
