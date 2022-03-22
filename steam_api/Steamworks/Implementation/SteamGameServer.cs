using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Core.Interface;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

//[Map("SteamGameServer")]
public class SteamGameServer : IBaseInterface, ISteamGameServer
{
    public int GetHSteamUser(IntPtr _)
    {
        return 1;
    }

    public int GetHSteamPipe(IntPtr _)
    {
        return 1;
    }

    public void RunCallbacks(IntPtr _)
    {

    }
}
