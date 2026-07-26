using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Steamworks
{
    /// <summary>
    /// Native SteamNetworking ABI helpers.  These types intentionally mirror the
    /// SDK layouts instead of exposing managed objects across the steam_api ABI.
    /// The game owns received SteamNetworkingMessage_t instances and releases them
    /// through SteamNetworkingMessage_t::Release when it has consumed the payload.
    /// </summary>
    internal static class SteamNetworkingIdentityInterop
    {
        internal const int Size = 136;
        internal const int DataCapacity = 128;
        private const int SteamIdPayloadSize = sizeof(ulong);
        private const int IpAddressPayloadSize = 18;
        private const int TypeOffset = 0;
        private const int SizeOffset = 4;
        private const int DataOffset = 8;

        internal static bool TryReadSteamId(IntPtr identity, out ulong steamId)
        {
            steamId = 0;
            if (identity == IntPtr.Zero ||
                Marshal.ReadInt32(identity, TypeOffset) != (int)NetIdentityType.SteamID ||
                Marshal.ReadInt32(identity, SizeOffset) != SteamIdPayloadSize)
            {
                return false;
            }

            steamId = unchecked((ulong)Marshal.ReadInt64(identity, DataOffset));
            return steamId != 0;
        }

        internal static void WriteSteamId(IntPtr identity, ulong steamId)
        {
            if (identity == IntPtr.Zero)
            {
                return;
            }

            Zero(identity, Size);
            Marshal.WriteInt32(identity, TypeOffset, (int)NetIdentityType.SteamID);
            Marshal.WriteInt32(identity, SizeOffset, SteamIdPayloadSize);
            Marshal.WriteInt64(identity, DataOffset, unchecked((long)steamId));
        }

        internal static void Clear(IntPtr identity)
        {
            if (identity != IntPtr.Zero)
            {
                Zero(identity, Size);
            }
        }

        internal static string Format(IntPtr identity)
        {
            return identity == IntPtr.Zero ? "invalid" : Format(Read(identity));
        }

        internal static bool TryParse(string value, UIntPtr identitySize, out SteamNetworkingIdentity_t identity)
        {
            identity = Invalid();
            var nativeSize = identitySize.ToUInt64();
            if (nativeSize < 32 || string.IsNullOrEmpty(value))
            {
                return false;
            }

            var capacity = (int)Math.Min(DataCapacity, nativeSize - DataOffset);
            if (value.StartsWith("steamid:", StringComparison.Ordinal))
            {
                return ulong.TryParse(value.Substring(8), NumberStyles.None, CultureInfo.InvariantCulture, out var steamId) &&
                    IsValidSteamId(steamId) &&
                    TryCreate(NetIdentityType.SteamID, BitConverter.GetBytes(steamId), capacity, out identity);
            }

            if (value.StartsWith("xboxpwid:", StringComparison.Ordinal))
            {
                return TryCreateNullTerminated(NetIdentityType.XboxPairwiseID, value.Substring(9), capacity, requireContent: true, out identity);
            }

            if (value.StartsWith("psn:", StringComparison.Ordinal))
            {
                return ulong.TryParse(value.Substring(4), NumberStyles.None, CultureInfo.InvariantCulture, out var psnId) &&
                    TryCreate(NetIdentityType.SonyPSN, BitConverter.GetBytes(psnId), capacity, out identity);
            }

            if (value.StartsWith("ip:", StringComparison.Ordinal) &&
                SteamNetworkingIPAddrInterop.TryParse(value.Substring(3), out var address))
            {
                return TryCreate(NetIdentityType.IPAddress, SteamNetworkingIPAddrInterop.ToBytes(address), capacity, out identity);
            }

            if (value.StartsWith("str:", StringComparison.Ordinal))
            {
                return TryCreateNullTerminated(NetIdentityType.GenericString, value.Substring(4), capacity, requireContent: false, out identity);
            }

            if (value.StartsWith("gen:", StringComparison.Ordinal))
            {
                var hex = value.Substring(4);
                if (hex.Length < 2 || (hex.Length & 1) != 0 || hex.Length / 2 > capacity)
                {
                    return false;
                }

                var bytes = new byte[hex.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    if (!byte.TryParse(hex.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out bytes[i]))
                    {
                        return false;
                    }
                }

                return TryCreate(NetIdentityType.GenericBytes, bytes, capacity, out identity);
            }

            var separator = value.IndexOf(':');
            if (separator <= 0 || separator > 16 || Encoding.UTF8.GetByteCount(value) + 1 > Math.Min(capacity, 128))
            {
                return false;
            }

            for (var i = 0; i < separator; i++)
            {
                var c = value[i];
                if ((c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '_')
                {
                    return false;
                }
            }

            return TryCreateNullTerminated(NetIdentityType.UnknownType, value, capacity, requireContent: true, out identity);
        }

        internal static SteamNetworkingIdentity_t Read(IntPtr identity)
        {
            if (identity == IntPtr.Zero)
            {
                return Invalid();
            }

            var value = Marshal.PtrToStructure<SteamNetworkingIdentity_t>(identity);
            value.m_data = NormalizeData(value.m_data);
            return value;
        }

        internal static void Write(IntPtr destination, SteamNetworkingIdentity_t identity)
        {
            if (destination == IntPtr.Zero)
            {
                return;
            }

            identity.m_data = NormalizeData(identity.m_data);
            Marshal.StructureToPtr(identity, destination, false);
        }

        internal static SteamNetworkingIdentity_t Clone(SteamNetworkingIdentity_t identity)
        {
            var clone = identity;
            clone.m_data = NormalizeData(identity.m_data);
            return clone;
        }

        internal static SteamNetworkingIdentity_t LocalHost()
        {
            return FromIpAddress(SteamNetworkingIPAddrInterop.LocalHost());
        }

        internal static SteamNetworkingIdentity_t FromIpAddress(SteamNetworkingIPAddr address)
        {
            return Create(NetIdentityType.IPAddress, SteamNetworkingIPAddrInterop.ToBytes(address));
        }

        internal static bool IsInvalid(IntPtr identity)
        {
            return identity == IntPtr.Zero || Marshal.ReadInt32(identity, TypeOffset) == (int)NetIdentityType.Invalid;
        }

        internal static bool IsLocalHost(IntPtr identity)
        {
            if (identity == IntPtr.Zero ||
                Marshal.ReadInt32(identity, TypeOffset) != (int)NetIdentityType.IPAddress ||
                Marshal.ReadInt32(identity, SizeOffset) != IpAddressPayloadSize)
            {
                return false;
            }

            return SteamNetworkingIPAddrInterop.IsLocalHost(IntPtr.Add(identity, DataOffset));
        }

        internal static bool Equals(IntPtr first, IntPtr second)
        {
            if (first == IntPtr.Zero || second == IntPtr.Zero)
            {
                return first == second;
            }

            var firstType = Marshal.ReadInt32(first, TypeOffset);
            var secondType = Marshal.ReadInt32(second, TypeOffset);
            var firstSize = Marshal.ReadInt32(first, SizeOffset);
            var secondSize = Marshal.ReadInt32(second, SizeOffset);
            if (firstType != secondType || firstSize != secondSize || firstSize < 0 || firstSize > DataCapacity)
            {
                return false;
            }

            for (var i = 0; i < firstSize; i++)
            {
                if (Marshal.ReadByte(first, DataOffset + i) != Marshal.ReadByte(second, DataOffset + i))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool SetBytes(IntPtr identity, NetIdentityType type, byte[] data, bool nullTerminate, bool requireContent)
        {
            if (identity == IntPtr.Zero || data == null || (requireContent && data.Length == 0))
            {
                return false;
            }

            var payloadSize = data.Length + (nullTerminate ? 1 : 0);
            if (payloadSize > DataCapacity)
            {
                return false;
            }

            Clear(identity);
            Marshal.WriteInt32(identity, TypeOffset, (int)type);
            Marshal.WriteInt32(identity, SizeOffset, payloadSize);
            if (data.Length > 0)
            {
                Marshal.Copy(data, 0, IntPtr.Add(identity, DataOffset), data.Length);
            }
            return true;
        }

        internal static IntPtr GetDataPointer(IntPtr identity, NetIdentityType expectedType, IntPtr sizeOutput = default(IntPtr))
        {
            if (sizeOutput != IntPtr.Zero)
            {
                Marshal.WriteInt32(sizeOutput, 0);
            }

            if (identity == IntPtr.Zero || Marshal.ReadInt32(identity, TypeOffset) != (int)expectedType)
            {
                return IntPtr.Zero;
            }

            var size = Marshal.ReadInt32(identity, SizeOffset);
            if (size < 0 || size > DataCapacity)
            {
                return IntPtr.Zero;
            }

            if (sizeOutput != IntPtr.Zero)
            {
                Marshal.WriteInt32(sizeOutput, size);
            }
            return IntPtr.Add(identity, DataOffset);
        }

        private static string Format(SteamNetworkingIdentity_t identity)
        {
            var data = NormalizeData(identity.m_data);
            var size = Math.Max(0, Math.Min(identity.m_cbSize, DataCapacity));
            switch ((NetIdentityType)identity.m_eType)
            {
                case NetIdentityType.Invalid:
                    return "invalid";
                case NetIdentityType.SteamID:
                    return size == sizeof(ulong)
                        ? "steamid:" + BitConverter.ToUInt64(data, 0).ToString(CultureInfo.InvariantCulture)
                        : "bad_type:" + identity.m_eType;
                case NetIdentityType.XboxPairwiseID:
                    return "xboxpwid:" + ReadNullTerminated(data, size);
                case NetIdentityType.SonyPSN:
                    return size == sizeof(ulong)
                        ? "psn:" + BitConverter.ToUInt64(data, 0).ToString(CultureInfo.InvariantCulture)
                        : "bad_type:" + identity.m_eType;
                case NetIdentityType.IPAddress:
                    if (size != IpAddressPayloadSize)
                    {
                        return "bad_type:" + identity.m_eType;
                    }
                    var address = SteamNetworkingIPAddrInterop.FromBytes(data);
                    return "ip:" + SteamNetworkingIPAddrInterop.Format(address, address.m_port != 0);
                case NetIdentityType.GenericString:
                    return "str:" + ReadNullTerminated(data, size);
                case NetIdentityType.GenericBytes:
                    var result = new StringBuilder(size * 2 + 4);
                    result.Append("gen:");
                    for (var i = 0; i < size; i++)
                    {
                        result.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
                    }
                    return result.ToString();
                case NetIdentityType.UnknownType:
                    return ReadNullTerminated(data, size);
                default:
                    return "bad_type:" + identity.m_eType;
            }
        }

        private static bool TryCreateNullTerminated(
            NetIdentityType type,
            string value,
            int capacity,
            bool requireContent,
            out SteamNetworkingIdentity_t identity)
        {
            var data = Encoding.UTF8.GetBytes(value ?? string.Empty);
            if ((requireContent && data.Length == 0) || data.Length + 1 > capacity)
            {
                identity = Invalid();
                return false;
            }

            var payload = new byte[data.Length + 1];
            Array.Copy(data, payload, data.Length);
            return TryCreate(type, payload, capacity, out identity);
        }

        private static bool TryCreate(NetIdentityType type, byte[] payload, int capacity, out SteamNetworkingIdentity_t identity)
        {
            if (payload == null || payload.Length > capacity || payload.Length > DataCapacity)
            {
                identity = Invalid();
                return false;
            }

            identity = Create(type, payload);
            return true;
        }

        private static SteamNetworkingIdentity_t Create(NetIdentityType type, byte[] payload)
        {
            var data = new byte[DataCapacity];
            if (payload != null)
            {
                Array.Copy(payload, data, Math.Min(payload.Length, data.Length));
            }

            return new SteamNetworkingIdentity_t
            {
                m_eType = (int)type,
                m_cbSize = payload?.Length ?? 0,
                m_data = data
            };
        }

        private static SteamNetworkingIdentity_t Invalid()
        {
            return new SteamNetworkingIdentity_t { m_data = new byte[DataCapacity] };
        }

        private static byte[] NormalizeData(byte[] data)
        {
            var normalized = new byte[DataCapacity];
            if (data != null)
            {
                Array.Copy(data, normalized, Math.Min(data.Length, normalized.Length));
            }
            return normalized;
        }

        private static string ReadNullTerminated(byte[] data, int size)
        {
            var length = 0;
            while (length < size && data[length] != 0)
            {
                length++;
            }
            return Encoding.UTF8.GetString(data, 0, length);
        }

        private static bool IsValidSteamId(ulong steamId)
        {
            var accountId = (uint)steamId;
            var accountInstance = (uint)((steamId >> 32) & 0xfffff);
            var accountType = (int)((steamId >> 52) & 0xf);
            var universe = (int)((steamId >> 56) & 0xff);
            if (accountType <= 0 || accountType >= 11 || universe <= 0 || universe >= 5)
            {
                return false;
            }

            if (accountType == 1)
            {
                return accountId != 0 && accountInstance == 1;
            }

            if (accountType == 7)
            {
                return accountId != 0 && accountInstance == 0;
            }

            return accountType != 3 || accountId != 0;
        }

        private static void Zero(IntPtr destination, int count)
        {
            var buffer = new byte[count];
            Marshal.Copy(buffer, 0, destination, count);
        }
    }

    internal static class SteamNetworkingIPAddrInterop
    {
        internal const int Size = 18;
        private const int AddressSize = 16;

        internal static SteamNetworkingIPAddr Read(IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return Empty();
            }

            var value = Marshal.PtrToStructure<SteamNetworkingIPAddr>(address);
            value.m_ipv6 = Normalize(value.m_ipv6);
            return value;
        }

        internal static void Write(IntPtr destination, SteamNetworkingIPAddr address)
        {
            if (destination == IntPtr.Zero)
            {
                return;
            }

            address.m_ipv6 = Normalize(address.m_ipv6);
            Marshal.StructureToPtr(address, destination, false);
        }

        internal static void Clear(IntPtr destination)
        {
            Write(destination, Empty());
        }

        internal static SteamNetworkingIPAddr Empty()
        {
            return new SteamNetworkingIPAddr { m_ipv6 = new byte[AddressSize] };
        }

        internal static SteamNetworkingIPAddr LocalHost(ushort port = 0)
        {
            var address = Empty();
            address.m_ipv6[15] = 1;
            address.m_port = port;
            return address;
        }

        internal static SteamNetworkingIPAddr FromIPv4(uint ipv4, ushort port)
        {
            var address = Empty();
            address.m_ipv6[10] = 0xff;
            address.m_ipv6[11] = 0xff;
            address.m_ipv6[12] = (byte)(ipv4 >> 24);
            address.m_ipv6[13] = (byte)(ipv4 >> 16);
            address.m_ipv6[14] = (byte)(ipv4 >> 8);
            address.m_ipv6[15] = (byte)ipv4;
            address.m_port = port;
            return address;
        }

        internal static bool IsIPv6AllZeros(IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return true;
            }

            for (var i = 0; i < AddressSize; i++)
            {
                if (Marshal.ReadByte(address, i) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsIPv4(SteamNetworkingIPAddr address)
        {
            var bytes = Normalize(address.m_ipv6);
            for (var i = 0; i < 10; i++)
            {
                if (bytes[i] != 0)
                {
                    return false;
                }
            }
            return bytes[10] == 0xff && bytes[11] == 0xff;
        }

        internal static uint GetIPv4(SteamNetworkingIPAddr address)
        {
            if (!IsIPv4(address))
            {
                return 0;
            }

            var bytes = address.m_ipv6;
            return ((uint)bytes[12] << 24) |
                ((uint)bytes[13] << 16) |
                ((uint)bytes[14] << 8) |
                bytes[15];
        }

        internal static bool IsLocalHost(IntPtr address)
        {
            return address != IntPtr.Zero && IsLocalHost(Read(address));
        }

        internal static bool IsLocalHost(SteamNetworkingIPAddr address)
        {
            var bytes = Normalize(address.m_ipv6);
            if (IsIPv4(address))
            {
                return GetIPv4(address) == 0x7f000001;
            }

            for (var i = 0; i < 15; i++)
            {
                if (bytes[i] != 0)
                {
                    return false;
                }
            }
            return bytes[15] == 1;
        }

        internal static bool Equals(IntPtr first, IntPtr second)
        {
            if (first == IntPtr.Zero || second == IntPtr.Zero)
            {
                return first == second;
            }

            for (var i = 0; i < Size; i++)
            {
                if (Marshal.ReadByte(first, i) != Marshal.ReadByte(second, i))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string Format(SteamNetworkingIPAddr address, bool withPort)
        {
            var bytes = Normalize(address.m_ipv6);
            string host;
            if (IsIPv4(address))
            {
                host = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}.{2}.{3}",
                    bytes[12],
                    bytes[13],
                    bytes[14],
                    bytes[15]);
                return withPort ? host + ":" + address.m_port.ToString(CultureInfo.InvariantCulture) : host;
            }

            host = new IPAddress(bytes).ToString();
            return withPort ? "[" + host + "]:" + address.m_port.ToString(CultureInfo.InvariantCulture) : host;
        }

        internal static bool TryParse(string value, out SteamNetworkingIPAddr address)
        {
            address = Empty();
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim();
            string host = value;
            ushort port = 0;
            if (value[0] == '[')
            {
                var close = value.IndexOf(']');
                if (close < 0)
                {
                    return false;
                }

                host = value.Substring(1, close - 1);
                if (close + 1 < value.Length)
                {
                    if (value[close + 1] != ':' ||
                        !ushort.TryParse(value.Substring(close + 2), NumberStyles.None, CultureInfo.InvariantCulture, out port))
                    {
                        return false;
                    }
                }
            }
            else
            {
                var firstColon = value.IndexOf(':');
                var lastColon = value.LastIndexOf(':');
                if (firstColon > 0 && firstColon == lastColon && value.IndexOf('.') >= 0)
                {
                    host = value.Substring(0, firstColon);
                    if (!ushort.TryParse(value.Substring(firstColon + 1), NumberStyles.None, CultureInfo.InvariantCulture, out port))
                    {
                        return false;
                    }
                }
            }

            if (!IPAddress.TryParse(host, out var parsed))
            {
                return false;
            }

            var bytes = parsed.GetAddressBytes();
            if (bytes.Length == 4)
            {
                address = FromIPv4(
                    ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3],
                    port);
                return true;
            }

            if (bytes.Length != AddressSize)
            {
                return false;
            }

            address = new SteamNetworkingIPAddr { m_ipv6 = bytes, m_port = port };
            return true;
        }

        internal static byte[] ToBytes(SteamNetworkingIPAddr address)
        {
            var bytes = new byte[Size];
            Array.Copy(Normalize(address.m_ipv6), bytes, AddressSize);
            bytes[16] = (byte)address.m_port;
            bytes[17] = (byte)(address.m_port >> 8);
            return bytes;
        }

        internal static SteamNetworkingIPAddr FromBytes(byte[] bytes)
        {
            var address = Empty();
            if (bytes == null)
            {
                return address;
            }

            Array.Copy(bytes, address.m_ipv6, Math.Min(AddressSize, bytes.Length));
            if (bytes.Length >= Size)
            {
                address.m_port = (ushort)(bytes[16] | (bytes[17] << 8));
            }
            return address;
        }

        private static byte[] Normalize(byte[] bytes)
        {
            var normalized = new byte[AddressSize];
            if (bytes != null)
            {
                Array.Copy(bytes, normalized, Math.Min(bytes.Length, normalized.Length));
            }
            return normalized;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamNetworkingIdentity_t
    {
        public int m_eType;
        public int m_cbSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_data;

        internal static SteamNetworkingIdentity_t FromSteamId(ulong steamId)
        {
            var data = new byte[SteamNetworkingIdentityInterop.DataCapacity];
            BitConverter.GetBytes(steamId).CopyTo(data, 0);
            return new SteamNetworkingIdentity_t
            {
                m_eType = (int)NetIdentityType.SteamID,
                m_cbSize = sizeof(ulong),
                m_data = data
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamNetConnectionInfo_t
    {
        public SteamNetworkingIdentity_t m_identityRemote;
        public long m_nUserData;
        public uint m_hListenSocket;
        public SteamNetworkingIPAddr m_addrRemote;
        public ushort m__pad1;
        public uint m_idPOPRemote;
        public uint m_idPOPRelay;
        public ConnectionState m_eState;
        public int m_eEndReason;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_szEndDebug;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_szConnectionDescription;
        public int m_nFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
        public uint[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamNetworkingMessage_t
    {
        public IntPtr m_pData;
        public int m_cbSize;
        public uint m_conn;
        public SteamNetworkingIdentity_t m_identityPeer;
        public long m_nConnUserData;
        public long m_usecTimeReceived;
        public long m_nMessageNumber;
        public IntPtr m_pfnFreeData;
        public IntPtr m_pfnRelease;
        public int m_nChannel;
        public int m_nFlags;
        public long m_nUserData;
        public ushort m_idxLane;
        public ushort m__pad1;
    }

    /// <summary>
    /// Owns native message allocations until the game calls Release.  It never
    /// retains managed references to game buffers: outbound data is copied before
    /// queuing and inbound data is copied into a dedicated unmanaged allocation.
    /// </summary>
    internal static class SteamNetworkingMessageStore
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseDelegate(IntPtr message);

        private sealed class Allocation
        {
            internal IntPtr Data;
            internal IntPtr Message;
        }

        private static readonly ConcurrentDictionary<IntPtr, Allocation> Allocations = new ConcurrentDictionary<IntPtr, Allocation>();
        private static readonly ReleaseDelegate ReleaseThunk = Release;
        private static readonly IntPtr ReleaseThunkPointer = Marshal.GetFunctionPointerForDelegate(ReleaseThunk);

        internal static IntPtr CreateReceived(byte[] payload, ulong remoteSteamId, uint connection, int channel, long connectionUserData, long messageNumber)
        {
            return CreateReceived(
                payload,
                SteamNetworkingIdentity_t.FromSteamId(remoteSteamId),
                connection,
                channel,
                connectionUserData,
                messageNumber);
        }

        internal static IntPtr CreateReceived(
            byte[] payload,
            SteamNetworkingIdentity_t remoteIdentity,
            uint connection,
            int channel,
            long connectionUserData,
            long messageNumber)
        {
            payload = payload ?? Array.Empty<byte>();
            var data = Marshal.AllocHGlobal(Math.Max(1, payload.Length));
            if (payload.Length > 0)
            {
                Marshal.Copy(payload, 0, data, payload.Length);
            }

            var message = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNetworkingMessage_t)));
            var native = new SteamNetworkingMessage_t
            {
                m_pData = data,
                m_cbSize = payload.Length,
                m_conn = connection,
                m_identityPeer = SteamNetworkingIdentityInterop.Clone(remoteIdentity),
                m_nConnUserData = connectionUserData,
                m_usecTimeReceived = DateTime.UtcNow.Ticks / 10,
                m_nMessageNumber = messageNumber,
                m_pfnRelease = ReleaseThunkPointer,
                m_nChannel = channel
            };

            Marshal.StructureToPtr(native, message, false);
            Allocations[message] = new Allocation { Data = data, Message = message };
            return message;
        }

        internal static IntPtr AllocateOutbound(int bufferSize)
        {
            if (bufferSize < 0)
            {
                return IntPtr.Zero;
            }

            var data = Marshal.AllocHGlobal(Math.Max(1, bufferSize));
            var message = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamNetworkingMessage_t)));
            var native = new SteamNetworkingMessage_t
            {
                m_pData = data,
                m_cbSize = bufferSize,
                m_identityPeer = new SteamNetworkingIdentity_t { m_data = new byte[128] },
                m_pfnRelease = ReleaseThunkPointer
            };

            Marshal.StructureToPtr(native, message, false);
            Allocations[message] = new Allocation { Data = data, Message = message };
            return message;
        }

        internal static bool TryRead(IntPtr message, out SteamNetworkingMessage_t native, out byte[] payload)
        {
            native = default(SteamNetworkingMessage_t);
            payload = Array.Empty<byte>();
            if (message == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                native = Marshal.PtrToStructure<SteamNetworkingMessage_t>(message);
                if (native.m_cbSize < 0 || native.m_pData == IntPtr.Zero)
                {
                    return native.m_cbSize == 0;
                }

                payload = new byte[native.m_cbSize];
                if (payload.Length > 0)
                {
                    Marshal.Copy(native.m_pData, payload, 0, payload.Length);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void Release(IntPtr message)
        {
            if (!Allocations.TryRemove(message, out var allocation))
            {
                return;
            }

            // SendMessages may take ownership of a caller-supplied data buffer.
            // Respect the Steam ABI contract by invoking its free callback before
            // releasing our message object. Failures are contained because this is
            // foreign game code and must never destabilize the steam_api process.
            try
            {
                var native = Marshal.PtrToStructure<SteamNetworkingMessage_t>(message);
                if (native.m_pfnFreeData != IntPtr.Zero)
                {
                    Marshal.GetDelegateForFunctionPointer<ReleaseDelegate>(native.m_pfnFreeData)(message);
                }
            }
            catch
            {
            }

            if (allocation.Data != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(allocation.Data);
            }

            if (allocation.Message != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(allocation.Message);
            }
        }
    }
}
