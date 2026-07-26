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
            SteamNetworkingIPAddrInterop.Clear(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros(IntPtr _)
        {
            return SteamNetworkingIPAddrInterop.IsIPv6AllZeros(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv6(IntPtr _, IntPtr ipv6, ushort nPort)
        {
            if (_ == IntPtr.Zero || ipv6 == IntPtr.Zero)
            {
                return;
            }

            var bytes = new byte[16];
            Marshal.Copy(ipv6, bytes, 0, bytes.Length);
            SteamNetworkingIPAddrInterop.Write(_, new SteamNetworkingIPAddr { m_ipv6 = bytes, m_port = nPort });
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv4(IntPtr _, uint nIP, ushort nPort)
        {
            SteamNetworkingIPAddrInterop.Write(_, SteamNetworkingIPAddrInterop.FromIPv4(nIP, nPort));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsIPv4(IntPtr _)
        {
            return _ != IntPtr.Zero && SteamNetworkingIPAddrInterop.IsIPv4(SteamNetworkingIPAddrInterop.Read(_));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(IntPtr _)
        {
            return _ == IntPtr.Zero ? 0 : SteamNetworkingIPAddrInterop.GetIPv4(SteamNetworkingIPAddrInterop.Read(_));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(IntPtr _, ushort nPort)
        {
            SteamNetworkingIPAddrInterop.Write(_, SteamNetworkingIPAddrInterop.LocalHost(nPort));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(IntPtr _)
        {
            return SteamNetworkingIPAddrInterop.IsLocalHost(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIPAddr_ToString(IntPtr _, IntPtr buf, UIntPtr cbBuf, bool bWithPort)
        {
            var value = _ == IntPtr.Zero
                ? string.Empty
                : SteamNetworkingIPAddrInterop.Format(SteamNetworkingIPAddrInterop.Read(_), bWithPort);
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), value);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_ParseString(IntPtr _, string pszStr)
        {
            if (!SteamNetworkingIPAddrInterop.TryParse(pszStr, out var address))
            {
                SteamNetworkingIPAddrInterop.Clear(_);
                return false;
            }

            SteamNetworkingIPAddrInterop.Write(_, address);
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(IntPtr _, IntPtr x)
        {
            return SteamNetworkingIPAddrInterop.Equals(_, x);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
