using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    /// <summary>
    /// ISteamNetworkingMessages is a connectionless, channelled P2P API.  The
    /// server only relays envelopes; session state and message queues remain local
    /// to the Steam client, just like Steam's client networking implementation.
    /// </summary>
    public class SteamNetworkingMessages : ISteamInterface
    {
        private const int MaxQueuedMessages = 2048;
        private readonly object _sessionGate = new object();
        private readonly Dictionary<ulong, Session> _sessions = new Dictionary<ulong, Session>();
        private readonly ConcurrentDictionary<int, ConcurrentQueue<IncomingMessage>> _incomingByChannel = new ConcurrentDictionary<int, ConcurrentQueue<IncomingMessage>>();
        private int _queuedMessages;
        private long _nextMessageNumber;

        private sealed class Session
        {
            internal bool Accepted;
            internal bool RequestNotified;
            internal readonly HashSet<int> OpenChannels = new HashSet<int>();
        }

        private sealed class IncomingMessage
        {
            internal ulong RemoteSteamId;
            internal int Channel;
            internal byte[] Payload;
            internal long MessageNumber;
        }

        public static SteamNetworkingMessages Instance;

        public SteamNetworkingMessages()
        {
            Instance = this;
            InterfaceName = "SteamNetworkingMessages";
            InterfaceVersion = "SteamNetworkingMessages002";
        }

        public int SendMessageToUser(IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId) ||
                cubData > 512 * 1024 || (cubData > 0 && pubData == IntPtr.Zero))
            {
                return (int)EResult.k_EResultInvalidParam;
            }

            var payload = new byte[cubData];
            if (payload.Length > 0)
            {
                Marshal.Copy(pubData, payload, 0, payload.Length);
            }

            lock (_sessionGate)
            {
                if (!_sessions.TryGetValue(remoteSteamId, out var session))
                {
                    session = new Session();
                    _sessions.Add(remoteSteamId, session);
                }

                // Sending is documented to implicitly accept a pending session.
                session.Accepted = true;
                session.OpenChannels.Add(nRemoteChannel);
            }

            // APIClient copies payload and schedules HTTP work on its dedicated
            // dispatcher. This call therefore never waits for the relay.
            return APIClient.SendP2PPacket(remoteSteamId, payload, nSendFlags, nRemoteChannel, "messages")
                ? (int)EResult.k_EResultOK
                : (int)EResult.k_EResultNoConnection;
        }

        public int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages)
        {
            if (ppOutMessages == IntPtr.Zero || nMaxMessages <= 0)
            {
                return 0;
            }

            if (!_incomingByChannel.TryGetValue(nLocalChannel, out var queue))
            {
                return 0;
            }

            var delivered = 0;
            while (delivered < nMaxMessages && queue.TryDequeue(out var message))
            {
                System.Threading.Interlocked.Decrement(ref _queuedMessages);
                var native = SteamNetworkingMessageStore.CreateReceived(
                    message.Payload,
                    message.RemoteSteamId,
                    0,
                    message.Channel,
                    0,
                    message.MessageNumber);
                Marshal.WriteIntPtr(ppOutMessages, delivered * IntPtr.Size, native);
                delivered++;
            }

            return delivered;
        }

        public bool AcceptSessionWithUser(IntPtr identityRemote)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId))
            {
                return false;
            }

            lock (_sessionGate)
            {
                if (!_sessions.TryGetValue(remoteSteamId, out var session))
                {
                    return false;
                }

                session.Accepted = true;
                return true;
            }
        }

        public bool CloseSessionWithUser(IntPtr identityRemote)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId))
            {
                return false;
            }

            lock (_sessionGate)
            {
                return _sessions.Remove(remoteSteamId);
            }
        }

        public bool CloseChannelWithUser(IntPtr identityRemote, int nLocalChannel)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId))
            {
                return false;
            }

            lock (_sessionGate)
            {
                if (!_sessions.TryGetValue(remoteSteamId, out var session))
                {
                    return false;
                }

                session.OpenChannels.Remove(nLocalChannel);
                if (session.OpenChannels.Count == 0)
                {
                    _sessions.Remove(remoteSteamId);
                }

                return true;
            }
        }

        public int GetSessionConnectionInfo(IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId))
            {
                return (int)ConnectionState.None;
            }

            Session session;
            lock (_sessionGate)
            {
                if (!_sessions.TryGetValue(remoteSteamId, out session))
                {
                    return (int)ConnectionState.None;
                }
            }

            if (pConnectionInfo != IntPtr.Zero)
            {
                Marshal.StructureToPtr(CreateConnectionInfo(remoteSteamId, session.Accepted ? ConnectionState.Connected : ConnectionState.Connecting, 0, 0, 0, string.Empty), pConnectionInfo, false);
            }

            return (int)(session.Accepted ? ConnectionState.Connected : ConnectionState.Connecting);
        }

        internal void ProcessMessage(ulong remoteSteamId, int channel, byte[] payload)
        {
            if (remoteSteamId == 0 || payload == null)
            {
                return;
            }

            var notifyRequest = false;
            lock (_sessionGate)
            {
                if (!_sessions.TryGetValue(remoteSteamId, out var session))
                {
                    session = new Session();
                    _sessions.Add(remoteSteamId, session);
                }

                session.OpenChannels.Add(channel);
                if (!session.RequestNotified)
                {
                    session.RequestNotified = true;
                    notifyRequest = true;
                }
            }

            if (System.Threading.Interlocked.Increment(ref _queuedMessages) > MaxQueuedMessages)
            {
                System.Threading.Interlocked.Decrement(ref _queuedMessages);
                SteamEmulator.Write("SteamNetworkingMessages", "Dropping incoming message because the receive queue is saturated");
                return;
            }

            var queue = _incomingByChannel.GetOrAdd(channel, _ => new ConcurrentQueue<IncomingMessage>());
            queue.Enqueue(new IncomingMessage
            {
                RemoteSteamId = remoteSteamId,
                Channel = channel,
                Payload = payload,
                MessageNumber = System.Threading.Interlocked.Increment(ref _nextMessageNumber)
            });

            if (notifyRequest)
            {
                SteamNetworkingUtils.Instance?.NotifyMessagesSessionRequest(remoteSteamId);
            }
        }

        internal static SteamNetConnectionInfo_t CreateConnectionInfo(ulong remoteSteamId, ConnectionState state, uint listenSocket, long userData, int endReason, string debug)
        {
            var description = "SKYNET P2P " + remoteSteamId;
            return new SteamNetConnectionInfo_t
            {
                m_identityRemote = SteamNetworkingIdentity_t.FromSteamId(remoteSteamId),
                m_nUserData = userData,
                m_hListenSocket = listenSocket,
                m_addrRemote = new SteamNetworkingIPAddr { m_ipv6 = new byte[16] },
                m_eState = state,
                m_eEndReason = endReason,
                m_szEndDebug = FixedUtf8(debug, 128),
                m_szConnectionDescription = FixedUtf8(description, 128),
                reserved = new uint[63]
            };
        }

        internal static byte[] FixedUtf8(string value, int size)
        {
            var buffer = new byte[size];
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(value);
                Array.Copy(bytes, buffer, Math.Min(bytes.Length, size - 1));
            }
            return buffer;
        }
    }
}
