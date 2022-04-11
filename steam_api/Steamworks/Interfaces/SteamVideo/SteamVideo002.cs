using System;

namespace SKYNET.Interface
{
    [Interface("STEAMVIDEO_INTERFACE_V002")]
    public class SteamVideo002 : ISteamInterface
    {
        public void GetVideoURL(IntPtr _, uint unVideoAppID)
        {
            SteamEmulator.SteamVideo.GetVideoURL(unVideoAppID);
        }

        public bool IsBroadcasting(IntPtr _, int pnNumViewers)
        {
            return SteamEmulator.SteamVideo.IsBroadcasting(pnNumViewers);
        }

        public void GetOPFSettings(IntPtr _, uint unVideoAppID)
        {
            SteamEmulator.SteamVideo.GetOPFSettings(unVideoAppID);
        }

        public bool GetOPFStringForApp(IntPtr _, uint unVideoAppID, string pchBuffer, int pnBufferSize)
        {
            return SteamEmulator.SteamVideo.GetOPFStringForApp(unVideoAppID, pchBuffer, pnBufferSize);
        }

    }
}
