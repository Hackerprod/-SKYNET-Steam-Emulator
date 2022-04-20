
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Steamworks
{
    [StructLayout(LayoutKind.Sequential)]
    public class CCallbackBase
    {
        public const byte k_ECallbackFlagsRegistered = 1;

        public const byte k_ECallbackFlagsGameServer = 2;

        public byte m_nCallbackFlags;

        public int m_iCallback;

        public IntPtr m_vfptr;

        public IntPtr ctor;
        public IntPtr Run;
        public IntPtr RunFull;
        public IntPtr GetICallback;
        public IntPtr GetCallbackSizeBytes;
    }
}
