using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

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
        private const int TypeOffset = 0;
        private const int SizeOffset = 4;
        private const int SteamIdOffset = 8;

        internal static bool TryReadSteamId(IntPtr identity, out ulong steamId)
        {
            steamId = 0;
            if (identity == IntPtr.Zero || Marshal.ReadInt32(identity, TypeOffset) != (int)NetIdentityType.SteamID)
            {
                return false;
            }

            steamId = unchecked((ulong)Marshal.ReadInt64(identity, SteamIdOffset));
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
            Marshal.WriteInt32(identity, SizeOffset, Size);
            Marshal.WriteInt64(identity, SteamIdOffset, unchecked((long)steamId));
        }

        internal static void Clear(IntPtr identity)
        {
            if (identity != IntPtr.Zero)
            {
                Zero(identity, Size);
                Marshal.WriteInt32(identity, SizeOffset, Size);
            }
        }

        internal static string Format(IntPtr identity)
        {
            return TryReadSteamId(identity, out var steamId) ? "steamid:" + steamId : string.Empty;
        }

        internal static bool TryParseSteamId(string value, out ulong steamId)
        {
            steamId = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            const string prefix = "steamid:";
            value = value.Trim();
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(prefix.Length);
            }

            return ulong.TryParse(value, out steamId) && steamId != 0;
        }

        private static void Zero(IntPtr destination, int count)
        {
            var buffer = new byte[count];
            Marshal.Copy(buffer, 0, destination, count);
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
            var data = new byte[128];
            BitConverter.GetBytes(steamId).CopyTo(data, 0);
            return new SteamNetworkingIdentity_t
            {
                m_eType = (int)NetIdentityType.SteamID,
                m_cbSize = SteamNetworkingIdentityInterop.Size,
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
                m_identityPeer = SteamNetworkingIdentity_t.FromSteamId(remoteSteamId),
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
