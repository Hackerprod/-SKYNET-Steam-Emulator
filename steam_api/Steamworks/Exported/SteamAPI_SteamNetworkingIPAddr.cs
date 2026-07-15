using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingIPAddr
    {
        static SteamAPI_SteamNetworkingIPAddr()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_Clear(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_Clear");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv6(IntPtr _, IntPtr ipv6, ushort nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv6");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv4(IntPtr _, uint nIP, ushort nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv4");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsIPv4(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsIPv4");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_GetIPv4");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(IntPtr _, ushort nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsLocalHost");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_ToString(IntPtr _, IntPtr buf, UIntPtr cbBuf, bool bWithPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_ToString");
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), string.Empty);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_ParseString(IntPtr _, string pszStr)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_ParseString");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(IntPtr _, IntPtr x)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsEqualTo");
            return false;
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
