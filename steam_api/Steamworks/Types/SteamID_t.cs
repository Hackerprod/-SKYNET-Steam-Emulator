using System;
using System.Runtime.InteropServices;

namespace Steamworks
{

    [StructLayout(LayoutKind.Sequential, Pack= 8)]
    public struct SteamID_t
    {
        public UInt32 low32Bits;    
        public UInt32 high32Bits;  
    }
}
