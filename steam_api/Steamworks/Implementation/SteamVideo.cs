using SKYNET;
using SKYNET.Helpers;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamVideo : ISteamInterface
    {
        public SteamVideo()
        {
            InterfaceName = "SteamVideo";
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
