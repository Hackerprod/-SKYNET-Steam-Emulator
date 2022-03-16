using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    [Interface("SteamGameServer")]
    public class SteamGameServer : SteamInterface, ISteamGameServer
    {
        public HSteamUser GetHSteamUser()
        {
            return (HSteamUser)1;
        }

        public HSteamPipe GetHSteamPipe()
        {
            return (HSteamPipe)1;
        }

        public void RunCallbacks()
        {

        }
    }
}