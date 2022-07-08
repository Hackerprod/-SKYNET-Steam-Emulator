using System;
using System.Runtime.InteropServices;

namespace SKYNET.Wave.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEHDR
    {
        public IntPtr lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public IntPtr dwUser;
        public uint dwFlags;
        public uint dwLoops;
        public IntPtr lpNext;
        public uint reserved;
    }
}
