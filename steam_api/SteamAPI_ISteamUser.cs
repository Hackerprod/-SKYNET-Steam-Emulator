using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET.Helper;
using Steamworks;

public class SteamAPI_ISteamUser : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ulong SteamAPI_ISteamUser_GetSteamID(IntPtr instancePtr)
    {
        DEBUG($"{"SteamAPI_ISteamUser_GetSteamID"}");

        //return true;
        return 76561198640235100;
    }

}
