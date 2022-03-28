using SKYNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamVideo : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamVideo_GetVideoURL(IntPtr unVideoAppID)
    {
        Write("SteamAPI_ISteamVideo_GetVideoURL");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamVideo_IsBroadcasting(int pnNumViewers)
    {
        Write("SteamAPI_ISteamVideo_IsBroadcasting");
        return SteamEmulator.SteamVideo.IsBroadcasting(pnNumViewers);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamVideo_GetOPFSettings(IntPtr unVideoAppID)
    {
        Write("SteamAPI_ISteamVideo_GetOPFSettings");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamVideo_GetOPFStringForApp(IntPtr unVideoAppID, string pchBuffer, uint pnBufferSize)
    {
        Write("SteamAPI_ISteamVideo_GetOPFStringForApp");
        return SteamEmulator.SteamVideo.GetOPFStringForApp(unVideoAppID, pchBuffer, pnBufferSize);
    }

}

