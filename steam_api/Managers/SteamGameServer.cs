using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

public partial class SteamGameServer : SteamInterface, ISteamGameServer
{
    internal HSteamUser GetHSteamUser()
    {
        return (HSteamUser)1;
    }

    internal HSteamPipe GetHSteamPipe()
    {
        return (HSteamPipe)1;
    }

    internal void RunCallbacks()
    {

    }
}