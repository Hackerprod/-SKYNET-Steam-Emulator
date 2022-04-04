using SKYNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
{
    public interface ISteamInterface
    {
        IntPtr MemoryAddress { get; set; }
        string InterfaceVersion { get; set; }
    }
}
