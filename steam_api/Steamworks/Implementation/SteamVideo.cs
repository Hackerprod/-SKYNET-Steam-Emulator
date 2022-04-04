using SKYNET;
using SKYNET.Helper;
using System;

public class SteamVideo : SteamInterface
{
    public void GetVideoURL(IntPtr unVideoAppID)
    {
        Write($"GetVideoURL");
    }

    public bool IsBroadcasting(int pnNumViewers)
    {
        Write($"IsBroadcasting");
        return false;
    }

    public void GetOPFSettings(IntPtr unVideoAppID)
    {
        Write($"GetOPFSettings");
    }

    public bool GetOPFStringForApp(IntPtr unVideoAppID, string pchBuffer, uint pnBufferSize)
    {
        Write($"GetOPFStringForApp");
        return false;
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}
