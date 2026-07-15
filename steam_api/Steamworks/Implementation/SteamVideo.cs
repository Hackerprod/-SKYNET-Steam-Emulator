using SKYNET.Steamworks.Interfaces;
using SKYNET.Helpers;
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
            InterfaceVersion = "STEAMVIDEO_INTERFACE_V007";
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

        public bool IsBroadcasting(IntPtr pnNumViewers)
        {
            Write($"IsBroadcasting");
            if (pnNumViewers != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnNumViewers, 0);
            }

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

        public bool GetOPFStringForApp(uint unVideoAppID, IntPtr pchBuffer, IntPtr pnBufferSize)
        {
            Write($"GetOPFStringForApp");
            if (pnBufferSize != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnBufferSize, 0);
            }

            NativeStringCache.WriteUtf8Buffer(pchBuffer, 1, string.Empty);
            return false;
        }
    }
}
