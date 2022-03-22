using Core.Interface;
using SKYNET.Interface;
using System;

//[Map("STEAMVIDEO_INTERFACE")]
//[Map("SteamVideo")]
public class SteamVideo : IBaseInterface, ISteamVideo
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
