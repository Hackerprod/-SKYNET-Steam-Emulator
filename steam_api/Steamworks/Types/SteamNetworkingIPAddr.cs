using System.Runtime.InteropServices;

namespace SKYNET.Steamworks
{
    /// Store an IP and port.  IPv6 is always used; IPv4 is represented using
    /// "IPv4-mapped" addresses: IPv4 aa.bb.cc.dd => IPv6 ::ffff:aabb:ccdd
    /// (RFC 4291 section 2.5.5.2.)
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SteamNetworkingIPAddr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] m_ipv6;
        public ushort m_port; // Host byte order

        // Max length of the buffer needed to hold IP formatted using ToString, including '\0'
        public const int k_cchMaxString = 48;
    }
}
