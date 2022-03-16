using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    [Interface("STEAMVIDEO_INTERFACE")]
    [Interface("SteamVideo")]
    public class SteamVideo : SteamInterface, ISteamVideo
    {
        public void GetVideoURL(IntPtr unVideoAppID)
        {
            //
        }

        public bool IsBroadcasting(int pnNumViewers)
        {
            return false;
        }

        public void GetOPFSettings(IntPtr unVideoAppID)
        {
            //
        }

        public bool GetOPFStringForApp(IntPtr unVideoAppID, string pchBuffer, uint pnBufferSize)
        {
            return false;
        }

    }

}