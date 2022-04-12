using SKYNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
{
    public class ISteamInterface
    {
        public string InterfaceVersion { get; set; }

        public void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
