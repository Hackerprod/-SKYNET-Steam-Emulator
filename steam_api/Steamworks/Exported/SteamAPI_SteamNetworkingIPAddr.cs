using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingIPAddr
    {
        public void SteamAPI_SteamNetworkingIPAddr_Clear(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_Clear");
        }

        public bool SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros");
            return false;
        }

        public void SteamAPI_SteamNetworkingIPAddr_SetIPv6(IntPtr _, int ipv6, int nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv6");
        }

        public void SteamAPI_SteamNetworkingIPAddr_SetIPv4(IntPtr _, uint nIP, int nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv4");
        }

        public bool SteamAPI_SteamNetworkingIPAddr_IsIPv4(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsIPv4");
            return true;
        }

        public uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_GetIPv4");
            return 0;
        }

        public void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(IntPtr _, int nPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost");
        }

        public bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_IsLocalHost");
            return true;
        }

        public void SteamAPI_SteamNetworkingIPAddr_ToString(IntPtr _, char buf, uint cbBuf, bool bWithPort)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_ToString");
        }

        public bool SteamAPI_SteamNetworkingIPAddr_ParseString(IntPtr _, char pszStr)
        {
            Write($"SteamAPI_SteamNetworkingIPAddr_ParseString");
            return false;
        }

        public bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(IntPtr _, IntPtr x)
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
