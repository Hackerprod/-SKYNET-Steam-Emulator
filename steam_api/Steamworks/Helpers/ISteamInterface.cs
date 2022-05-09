using SKYNET;
using SKYNET.Callback;
using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
{
    public class ISteamInterface
    {
        public string InterfaceVersion { get; set; }
        public string InterfaceName { get; set; }

        public void Write(object msg)
        {
            SteamEmulator.Write(InterfaceName, msg);
        }
    }
}
