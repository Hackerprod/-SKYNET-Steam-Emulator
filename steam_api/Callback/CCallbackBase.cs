
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SKYNET.Callback
{
    [StructLayout(LayoutKind.Sequential)]
    public class CCallbackBase
    {
        public IntPtr CCallbackMgr;

        public byte m_nCallbackFlags;

        public int m_iCallback;

        public const byte k_ECallbackFlagsRegistered = 0x01;
        public const byte k_ECallbackFlagsGameServer = 0x02;
    }
}
