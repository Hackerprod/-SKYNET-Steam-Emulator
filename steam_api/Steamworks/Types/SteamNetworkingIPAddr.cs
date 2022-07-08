using System.Runtime.InteropServices;

namespace SKYNET.Steamworks
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SteamNetworkingIPAddr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] m_ipv6;
        public ushort m_port; // Host byte order

        public const int k_cchMaxString = 48;
    }
}
