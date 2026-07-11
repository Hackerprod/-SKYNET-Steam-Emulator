using System.Runtime.InteropServices;

namespace SKYNET.Steamworks
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 20)]
    public struct SteamIPAddress_t
    {
        [FieldOffset(0)]
        private uint m_unIPv4;

        [FieldOffset(0)]
        private ulong m_ipv6Qword0;

        [FieldOffset(8)]
        private ulong m_ipv6Qword1;

        [FieldOffset(16)]
        private SteamIPType m_eType;

        public SteamIPAddress_t(uint ipv4)
        {
            m_ipv6Qword0 = 0;
            m_ipv6Qword1 = 0;
            m_eType = SteamIPType.Type4;
            m_unIPv4 = ipv4;
        }

        public SteamIPAddress_t(System.Net.IPAddress iPAddress)
        {
            m_unIPv4 = 0;
            m_ipv6Qword0 = 0;
            m_ipv6Qword1 = 0;
            m_eType = SteamIPType.Type4;

            byte[] bytes = iPAddress.GetAddressBytes();
            switch (iPAddress.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    {
                        if (bytes.Length != 4)
                        {
                            throw new System.TypeInitializationException("SteamIPAddress_t: Unexpected byte length for Ipv4." + bytes.Length, null);
                        }

                        m_unIPv4 = (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
                        m_eType = SteamIPType.Type4;
                        break;
                    }
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    {
                        if (bytes.Length != 16)
                        {
                            throw new System.TypeInitializationException("SteamIPAddress_t: Unexpected byte length for Ipv6: " + bytes.Length, null);
                        }

                        m_ipv6Qword0 = System.BitConverter.ToUInt64(bytes, 0);
                        m_ipv6Qword1 = System.BitConverter.ToUInt64(bytes, 8);
                        m_eType = SteamIPType.Type6;
                        break;
                    }
                default:
                    {
                        throw new System.TypeInitializationException("SteamIPAddress_t: Unexpected address family " + iPAddress.AddressFamily, null);
                    }
            }
        }

        public System.Net.IPAddress ToIPAddress()
        {
            if (m_eType == SteamIPType.Type4)
            {
                byte[] bytes = System.BitConverter.GetBytes(m_unIPv4);
                return new System.Net.IPAddress(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] });
            }
            else
            {
                byte[] bytes = new byte[16];
                System.BitConverter.GetBytes(m_ipv6Qword0).CopyTo(bytes, 0);
                System.BitConverter.GetBytes(m_ipv6Qword1).CopyTo(bytes, 8);
                return new System.Net.IPAddress(bytes);
            }
        }

        public override string ToString()
        {
            return ToIPAddress().ToString();
        }

        public SteamIPType GetIPType()
        {
            return m_eType;
        }

        public bool IsSet()
        {
            return m_eType == SteamIPType.Type4
                ? m_unIPv4 != 0
                : m_ipv6Qword0 != 0 || m_ipv6Qword1 != 0;
        }
    }
}

