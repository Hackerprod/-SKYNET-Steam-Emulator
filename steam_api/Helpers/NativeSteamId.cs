using System;
using System.Runtime.InteropServices;
using SKYNET.Steamworks;

namespace SKYNET.Helpers
{
    internal static class NativeSteamId
    {
        public static IntPtr Write(IntPtr destination, CSteamID steamID)
        {
            return Write(destination, (ulong)steamID);
        }

        public static IntPtr Write(IntPtr destination, ulong steamID)
        {
            if (destination == IntPtr.Zero)
            {
                return destination;
            }

            Marshal.WriteInt64(destination, unchecked((long)steamID));
            return destination;
        }
    }
}
