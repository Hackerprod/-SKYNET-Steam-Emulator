using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using HSteamNetConnection = System.UInt32;
using HSteamListenSocket = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    /// <summary>
    /// Stateful P2P transport for ISteamNetworkingSockets.  Connection handles are
    /// process-local; peer selection is always the tuple (SteamID, virtual port).
    /// Control and data frames travel through APIClient's bounded async relay queue,
    /// so no socket API call performs network I/O on a game thread.
    /// </summary>
    public class SteamNetworkingSockets : ISteamInterface
    {
        private const string TransportOpen = "sockets_open";
        private const string TransportAccept = "sockets_accept";
        private const string TransportReject = "sockets_reject";
        private const string TransportClose = "sockets_close";
        private const string TransportData = "sockets_data";
        private const int MaxQueuedMessagesPerConnection = 2048;

        private readonly object _gate = new object();
        private readonly Dictionary<uint, ListenSocket> _listenSockets = new Dictionary<uint, ListenSocket>();
        private readonly Dictionary<uint, SocketConnection> _connections = new Dictionary<uint, SocketConnection>();
        private readonly HashSet<uint> _pollGroups = new HashSet<uint>();
        private int _nextHandle;
        private long _nextMessageNumber;

        private sealed class ListenSocket
        {
            internal uint Handle;
            internal ushort Port;
            internal int VirtualPort;
            internal SteamNetworkingIPAddr Address;
        }

        internal sealed class SocketConnection
        {
            internal uint Handle;
            internal uint ListenSocket;
            internal ulong RemoteSteamId;
            internal int VirtualPort;
            internal ConnectionState State;
            internal long UserData;
            internal string Name = string.Empty;
            internal int EndReason;
            internal string Debug = string.Empty;
            internal uint PollGroup;
            internal long NextOutboundMessageNumber;
            internal readonly ConcurrentQueue<IncomingPacket> Incoming = new ConcurrentQueue<IncomingPacket>();
            internal int IncomingCount;
            internal uint LoopbackPeer;
        }

        internal sealed class IncomingPacket
        {
            internal byte[] Payload;
            internal long MessageNumber;
        }

        public static SteamNetworkingSockets Instance;

        public SteamNetworkingSockets()
        {
            Instance = this;
            InterfaceName = "SteamNetworkingSockets";
            InterfaceVersion = "SteamNetworkingSockets012";
        }

        public HSteamListenSocket CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions)
        {
            if (localAddress == IntPtr.Zero)
            {
                return 0;
            }

            var address = Marshal.PtrToStructure<SteamNetworkingIPAddr>(localAddress);
            lock (_gate)
            {
                var handle = AllocateHandleLocked();
                _listenSockets.Add(handle, new ListenSocket
                {
                    Handle = handle,
                    Port = address.m_port,
                    VirtualPort = -1,
                    Address = address
                });
                return handle;
            }
        }

        public HSteamNetConnection ConnectByIPAddress(IntPtr address, int nOptions, IntPtr pOptions)
        {
            // IP sockets do not provide a Steam identity to route through the
            // authenticated relay.  Keep the API honest instead of returning a
            // fake connected handle; callers can use CreateSocketPair for local
            // loopback or ConnectP2P for a Steam identity.
            return 0;
        }

        public HSteamListenSocket CreateListenSocketP2P(int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            lock (_gate)
            {
                foreach (var existing in _listenSockets.Values)
                {
                    if (existing.VirtualPort == nVirtualPort)
                    {
                        return 0;
                    }
                }

                var handle = AllocateHandleLocked();
                _listenSockets.Add(handle, new ListenSocket
                {
                    Handle = handle,
                    VirtualPort = nVirtualPort,
                    Address = new SteamNetworkingIPAddr { m_ipv6 = new byte[16] }
                });
                return handle;
            }
        }

        public HSteamNetConnection ConnectP2P(IntPtr identityRemote, int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            if (!SteamNetworkingIdentityInterop.TryReadSteamId(identityRemote, out var remoteSteamId))
            {
                return 0;
            }

            SocketConnection connection;
            lock (_gate)
            {
                connection = new SocketConnection
                {
                    Handle = AllocateHandleLocked(),
                    RemoteSteamId = remoteSteamId,
                    VirtualPort = nVirtualPort,
                    State = ConnectionState.Connecting
                };
                _connections.Add(connection.Handle, connection);
            }

            NotifyStateChange(connection, ConnectionState.None);
            SendFrame(remoteSteamId, nVirtualPort, TransportOpen, Array.Empty<byte>());
            return connection.Handle;
        }

        public int AcceptConnection(HSteamNetConnection hConn)
        {
            SocketConnection connection;
            ConnectionState oldState;
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out connection))
                {
                    return (int)EResult.k_EResultInvalidParam;
                }

                if (connection.State == ConnectionState.Connected)
                {
                    return (int)EResult.k_EResultOK;
                }

                if (connection.State != ConnectionState.Connecting)
                {
                    return (int)EResult.k_EResultInvalidState;
                }

                oldState = connection.State;
                connection.State = ConnectionState.Connected;
            }

            NotifyStateChange(connection, oldState);
            if (connection.LoopbackPeer == 0)
            {
                SendFrame(connection.RemoteSteamId, connection.VirtualPort, TransportAccept, Array.Empty<byte>());
            }
            return (int)EResult.k_EResultOK;
        }

        public bool CloseConnection(HSteamNetConnection hPeer, int nReason, string pszDebug, bool bEnableLinger)
        {
            SocketConnection connection;
            ConnectionState oldState;
            lock (_gate)
            {
                if (!_connections.TryGetValue(hPeer, out connection) || IsTerminal(connection.State))
                {
                    return false;
                }

                oldState = connection.State;
                connection.State = ConnectionState.ProblemDetectedLocally;
                connection.EndReason = nReason == 0 ? (int)NetConnectionEnd.App_Generic : nReason;
                connection.Debug = pszDebug ?? string.Empty;
            }

            NotifyStateChange(connection, oldState);
            if (connection.LoopbackPeer != 0)
            {
                CloseLoopbackPeer(connection.LoopbackPeer, connection.EndReason, connection.Debug);
            }
            else
            {
                SendFrame(connection.RemoteSteamId, connection.VirtualPort, TransportClose, Array.Empty<byte>());
            }

            return true;
        }

        public bool CloseListenSocket(HSteamListenSocket hSocket)
        {
            List<SocketConnection> affected = new List<SocketConnection>();
            lock (_gate)
            {
                if (!_listenSockets.Remove(hSocket))
                {
                    return false;
                }

                foreach (var connection in _connections.Values)
                {
                    if (connection.ListenSocket == hSocket && !IsTerminal(connection.State))
                    {
                        var oldState = connection.State;
                        connection.State = ConnectionState.ProblemDetectedLocally;
                        connection.EndReason = (int)NetConnectionEnd.App_Generic;
                        connection.Debug = "Listen socket closed";
                        affected.Add(connection);
                        NotifyStateChange(connection, oldState);
                    }
                }
            }

            foreach (var connection in affected)
            {
                SendFrame(connection.RemoteSteamId, connection.VirtualPort, TransportClose, Array.Empty<byte>());
            }
            return true;
        }

        public bool SetConnectionUserData(HSteamNetConnection hPeer, long nUserData)
        {
            lock (_gate)
            {
                if (!_connections.TryGetValue(hPeer, out var connection))
                {
                    return false;
                }

                connection.UserData = nUserData;
                return true;
            }
        }

        public long GetConnectionUserData(HSteamNetConnection hPeer)
        {
            lock (_gate)
            {
                return _connections.TryGetValue(hPeer, out var connection) ? connection.UserData : -1;
            }
        }

        public void SetConnectionName(HSteamNetConnection hPeer, string pszName)
        {
            lock (_gate)
            {
                if (_connections.TryGetValue(hPeer, out var connection))
                {
                    connection.Name = pszName ?? string.Empty;
                }
            }
        }

        public bool GetConnectionName(HSteamNetConnection hPeer, IntPtr pszName, int nMaxLen)
        {
            lock (_gate)
            {
                if (!_connections.TryGetValue(hPeer, out var connection))
                {
                    return false;
                }

                NativeStringCache.WriteUtf8Buffer(pszName, nMaxLen, connection.Name);
                return true;
            }
        }

        public int SendMessageToConnection(HSteamNetConnection hConn, IntPtr pData, UInt32 cbData, int nSendFlags, IntPtr pOutMessageNumber)
        {
            if (cbData > 512 * 1024 || (cbData > 0 && pData == IntPtr.Zero))
            {
                return (int)EResult.k_EResultInvalidParam;
            }

            SocketConnection connection;
            long messageNumber;
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out connection))
                {
                    return (int)EResult.k_EResultInvalidParam;
                }

                if (IsTerminal(connection.State))
                {
                    return (int)EResult.k_EResultNoConnection;
                }

                messageNumber = ++connection.NextOutboundMessageNumber;
            }

            var payload = new byte[cbData];
            if (payload.Length > 0)
            {
                Marshal.Copy(pData, payload, 0, payload.Length);
            }

            if (pOutMessageNumber != IntPtr.Zero)
            {
                Marshal.WriteInt64(pOutMessageNumber, messageNumber);
            }

            if (connection.LoopbackPeer != 0)
            {
                QueueLoopback(connection.LoopbackPeer, payload);
                return (int)EResult.k_EResultOK;
            }

            return SendFrame(connection.RemoteSteamId, connection.VirtualPort, TransportData, payload, nSendFlags)
                ? (int)EResult.k_EResultOK
                : (int)EResult.k_EResultNoConnection;
        }

        public void SendMessages(int nMessages, IntPtr pMessages, IntPtr pOutMessageNumberOrResult)
        {
            if (nMessages <= 0 || pMessages == IntPtr.Zero)
            {
                return;
            }

            for (var i = 0; i < nMessages; i++)
            {
                var message = Marshal.ReadIntPtr(pMessages, i * IntPtr.Size);
                var result = (int)EResult.k_EResultInvalidParam;
                long numberOrResult;
                if (SteamNetworkingMessageStore.TryRead(message, out var native, out var payload))
                {
                    var buffer = Marshal.AllocHGlobal(Math.Max(1, payload.Length));
                    try
                    {
                        if (payload.Length > 0)
                        {
                            Marshal.Copy(payload, 0, buffer, payload.Length);
                        }
                        result = SendMessageToConnection(native.m_conn, buffer, (uint)payload.Length, native.m_nFlags, IntPtr.Zero);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }

                numberOrResult = result == (int)EResult.k_EResultOK
                    ? GetLastMessageNumber(message)
                    : -result;
                if (pOutMessageNumberOrResult != IntPtr.Zero)
                {
                    Marshal.WriteInt64(pOutMessageNumberOrResult, i * sizeof(long), numberOrResult);
                }
                SteamNetworkingMessageStore.Release(message);
            }
        }

        public int FlushMessagesOnConnection(HSteamNetConnection hConn)
        {
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out var connection))
                {
                    return (int)EResult.k_EResultInvalidParam;
                }

                return IsTerminal(connection.State)
                    ? (int)EResult.k_EResultNoConnection
                    : connection.State == ConnectionState.Connected ? (int)EResult.k_EResultOK : (int)EResult.k_EResultIgnored;
            }
        }

        public int ReceiveMessagesOnConnection(HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages)
        {
            if (nMaxMessages <= 0 || ppOutMessages == IntPtr.Zero)
            {
                return 0;
            }

            SocketConnection connection;
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out connection))
                {
                    return -1;
                }
            }

            return DequeueMessages(connection, ppOutMessages, nMaxMessages);
        }

        public bool GetConnectionInfo(HSteamNetConnection hConn, IntPtr pInfo)
        {
            if (pInfo == IntPtr.Zero)
            {
                return false;
            }

            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out var connection))
                {
                    return false;
                }

                Marshal.StructureToPtr(CreateInfo(connection), pInfo, false);
                return true;
            }
        }

        public int GetConnectionRealTimeStatus(HSteamNetConnection hConn, IntPtr pStatus, int nLanes, IntPtr pLanes)
        {
            lock (_gate)
            {
                return _connections.ContainsKey(hConn)
                    ? (int)EResult.k_EResultOK
                    : (int)EResult.k_EResultNoConnection;
            }
        }

        public int GetDetailedConnectionStatus(HSteamNetConnection hConn, IntPtr pszBuf, int cbBuf)
        {
            SocketConnection connection;
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out connection))
                {
                    return -1;
                }
            }

            var text = "SKYNET P2P: " + connection.State + " remote=" + connection.RemoteSteamId + " port=" + connection.VirtualPort;
            NativeStringCache.WriteUtf8Buffer(pszBuf, cbBuf, text);
            return text.Length + 1 > cbBuf ? text.Length + 1 : 0;
        }

        public bool GetListenSocketAddress(HSteamListenSocket hSocket, IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return false;
            }

            lock (_gate)
            {
                if (!_listenSockets.TryGetValue(hSocket, out var socket))
                {
                    return false;
                }

                Marshal.StructureToPtr(socket.Address, address, false);
                return true;
            }
        }

        public bool CreateSocketPair(IntPtr pOutConnection1, IntPtr pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
        {
            if (pOutConnection1 == IntPtr.Zero || pOutConnection2 == IntPtr.Zero)
            {
                return false;
            }

            SocketConnection first;
            SocketConnection second;
            lock (_gate)
            {
                first = new SocketConnection { Handle = AllocateHandleLocked(), State = ConnectionState.Connected, RemoteSteamId = (ulong)SteamEmulator.SteamID };
                second = new SocketConnection { Handle = AllocateHandleLocked(), State = ConnectionState.Connected, RemoteSteamId = (ulong)SteamEmulator.SteamID };
                first.LoopbackPeer = second.Handle;
                second.LoopbackPeer = first.Handle;
                _connections.Add(first.Handle, first);
                _connections.Add(second.Handle, second);
            }

            Marshal.WriteInt32(pOutConnection1, unchecked((int)first.Handle));
            Marshal.WriteInt32(pOutConnection2, unchecked((int)second.Handle));
            return true;
        }

        public int ConfigureConnectionLanes(HSteamNetConnection hConn, int nNumLanes, IntPtr pLanePriorities, IntPtr pLaneWeights)
        {
            return nNumLanes < 0 || !HasConnection(hConn)
                ? (int)EResult.k_EResultInvalidParam
                : (int)EResult.k_EResultOK;
        }

        public bool GetIdentity(IntPtr pIdentity)
        {
            if (pIdentity == IntPtr.Zero)
            {
                return false;
            }

            SteamNetworkingIdentityInterop.WriteSteamId(pIdentity, (ulong)SteamEmulator.SteamID);
            return true;
        }

        public int InitAuthentication() => (int)ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;
        public int GetAuthenticationStatus(IntPtr pDetails) => (int)ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;

        public uint CreatePollGroup()
        {
            lock (_gate)
            {
                var handle = AllocateHandleLocked();
                _pollGroups.Add(handle);
                return handle;
            }
        }

        public bool DestroyPollGroup(uint hPollGroup)
        {
            lock (_gate)
            {
                if (!_pollGroups.Remove(hPollGroup))
                {
                    return false;
                }

                foreach (var connection in _connections.Values)
                {
                    if (connection.PollGroup == hPollGroup)
                    {
                        connection.PollGroup = 0;
                    }
                }
                return true;
            }
        }

        public bool SetConnectionPollGroup(HSteamNetConnection hConn, uint hPollGroup)
        {
            lock (_gate)
            {
                if (!_connections.TryGetValue(hConn, out var connection) || (hPollGroup != 0 && !_pollGroups.Contains(hPollGroup)))
                {
                    return false;
                }
                connection.PollGroup = hPollGroup;
                return true;
            }
        }

        public int ReceiveMessagesOnPollGroup(uint hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
        {
            if (nMaxMessages <= 0 || ppOutMessages == IntPtr.Zero)
            {
                return 0;
            }

            List<SocketConnection> connections = new List<SocketConnection>();
            lock (_gate)
            {
                if (!_pollGroups.Contains(hPollGroup))
                {
                    return -1;
                }
                foreach (var connection in _connections.Values)
                {
                    if (connection.PollGroup == hPollGroup)
                    {
                        connections.Add(connection);
                    }
                }
            }

            var written = 0;
            foreach (var connection in connections)
            {
                if (written >= nMaxMessages)
                {
                    break;
                }
                written += DequeueMessages(connection, IntPtr.Add(ppOutMessages, written * IntPtr.Size), nMaxMessages - written);
            }
            return written;
        }

        public bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, IntPtr pOutParsedTicket) => false;
        public int FindRelayAuthTicketForServer(IntPtr identityGameServer, int nVirtualPort, IntPtr pOutParsedTicket) => (int)EResult.k_EResultNoMatch;
        public HSteamNetConnection ConnectToHostedDedicatedServer(IntPtr identityTarget, int nVirtualPort, int nOptions, IntPtr pOptions) => ConnectP2P(identityTarget, nVirtualPort, nOptions, pOptions);
        public ushort GetHostedDedicatedServerPort() => 0;
        public uint GetHostedDedicatedServerPOPID() => 0;
        public int GetHostedDedicatedServerAddress(IntPtr pRouting) => (int)EResult.k_EResultNoMatch;
        public HSteamListenSocket CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, IntPtr pOptions) => CreateListenSocketP2P(nVirtualPort, nOptions, pOptions);
        public int GetGameCoordinatorServerLogin(IntPtr pLoginInfo, IntPtr pcbSignedBlob, IntPtr pBlob) => (int)EResult.k_EResultNoConnection;
        public HSteamNetConnection ConnectP2PCustomSignaling(IntPtr pSignaling, IntPtr pPeerIdentity, int nRemoteVirtualPort, int nOptions, IntPtr pOptions) => ConnectP2P(pPeerIdentity, nRemoteVirtualPort, nOptions, pOptions);
        public bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext) => false;
        public bool GetCertificateRequest(IntPtr pcbBlob, IntPtr pBlob, IntPtr errMsg) => false;
        public bool SetCertificate(IntPtr pCertificate, int cbCertificate, IntPtr errMsg) => false;
        public void RunCallbacks(IntPtr pCallbacks) { }
        public bool SteamDatagramClient_Init(bool bNoSteamSupport, IntPtr errMsg) => true;
        public bool SteamDatagramServer_Init(bool bNoSteamSupport, IntPtr errMsg) => true;
        internal void ResetIdentity(IntPtr pIdentity) => SteamNetworkingIdentityInterop.Clear(pIdentity);
        internal void RunCallbacks() { }
        internal bool BeginAsyncRequestFakeIP(int nNumPorts) => false;
        internal void GetFakeIP(int idxFirstPort, IntPtr pInfo) { }
        internal uint CreateListenSocketP2PFakeIP(int idxFakePort, int nOptions, IntPtr pOptions) => 0;
        internal int GetRemoteFakeIPForConnection(uint hConn, IntPtr pOutAddr) => (int)EResult.k_EResultNoMatch;
        internal IntPtr CreateFakeUDPPort(int idxFakeServerPort) => IntPtr.Zero;

        internal void ProcessRelayPacket(string transport, ulong remoteSteamId, int virtualPort, byte[] payload)
        {
            switch (transport)
            {
                case TransportOpen:
                    ProcessOpen(remoteSteamId, virtualPort);
                    break;
                case TransportAccept:
                    ProcessAccept(remoteSteamId, virtualPort);
                    break;
                case TransportReject:
                    ProcessClosed(remoteSteamId, virtualPort, (int)NetConnectionEnd.Remote_Timeout, "Remote listener rejected the connection");
                    break;
                case TransportClose:
                    ProcessClosed(remoteSteamId, virtualPort, (int)NetConnectionEnd.Remote_Timeout, "Remote closed the connection");
                    break;
                case TransportData:
                    ProcessData(remoteSteamId, virtualPort, payload ?? Array.Empty<byte>());
                    break;
            }
        }

        private void ProcessOpen(ulong remoteSteamId, int virtualPort)
        {
            SocketConnection connection = null;
            lock (_gate)
            {
                foreach (var existing in _connections.Values)
                {
                    if (existing.RemoteSteamId == remoteSteamId && existing.VirtualPort == virtualPort && existing.ListenSocket != 0 && !IsTerminal(existing.State))
                    {
                        return;
                    }
                }

                ListenSocket listener = null;
                foreach (var candidate in _listenSockets.Values)
                {
                    if (candidate.VirtualPort == virtualPort)
                    {
                        listener = candidate;
                        break;
                    }
                }

                if (listener != null)
                {
                    connection = new SocketConnection
                    {
                        Handle = AllocateHandleLocked(),
                        ListenSocket = listener.Handle,
                        RemoteSteamId = remoteSteamId,
                        VirtualPort = virtualPort,
                        State = ConnectionState.Connecting
                    };
                    _connections.Add(connection.Handle, connection);
                }
            }

            if (connection == null)
            {
                SendFrame(remoteSteamId, virtualPort, TransportReject, Array.Empty<byte>());
                return;
            }

            NotifyStateChange(connection, ConnectionState.None);
        }

        private void ProcessAccept(ulong remoteSteamId, int virtualPort)
        {
            SocketConnection connection = null;
            lock (_gate)
            {
                foreach (var candidate in _connections.Values)
                {
                    if (candidate.RemoteSteamId == remoteSteamId && candidate.VirtualPort == virtualPort && candidate.ListenSocket == 0 && candidate.State == ConnectionState.Connecting)
                    {
                        connection = candidate;
                        candidate.State = ConnectionState.Connected;
                        break;
                    }
                }
            }

            if (connection != null)
            {
                NotifyStateChange(connection, ConnectionState.Connecting);
            }
        }

        private void ProcessClosed(ulong remoteSteamId, int virtualPort, int reason, string debug)
        {
            List<SocketConnection> changed = new List<SocketConnection>();
            lock (_gate)
            {
                foreach (var connection in _connections.Values)
                {
                    if (connection.RemoteSteamId == remoteSteamId && connection.VirtualPort == virtualPort && !IsTerminal(connection.State))
                    {
                        var oldState = connection.State;
                        connection.State = ConnectionState.ClosedByPeer;
                        connection.EndReason = reason;
                        connection.Debug = debug;
                        changed.Add(connection);
                        NotifyStateChange(connection, oldState);
                    }
                }
            }
        }

        private void ProcessData(ulong remoteSteamId, int virtualPort, byte[] payload)
        {
            SocketConnection connection = null;
            lock (_gate)
            {
                foreach (var candidate in _connections.Values)
                {
                    if (candidate.RemoteSteamId == remoteSteamId && candidate.VirtualPort == virtualPort && !IsTerminal(candidate.State))
                    {
                        connection = candidate;
                        break;
                    }
                }
            }

            if (connection == null || Interlocked.Increment(ref connection.IncomingCount) > MaxQueuedMessagesPerConnection)
            {
                if (connection != null)
                {
                    Interlocked.Decrement(ref connection.IncomingCount);
                }
                SteamEmulator.Write("SteamNetworkingSockets", "Dropping incoming message because the connection queue is saturated or unknown");
                return;
            }

            connection.Incoming.Enqueue(new IncomingPacket
            {
                Payload = payload,
                MessageNumber = Interlocked.Increment(ref _nextMessageNumber)
            });
        }

        private int DequeueMessages(SocketConnection connection, IntPtr output, int maxMessages)
        {
            // Steam does not expose received data while an incoming connection is
            // waiting for AcceptConnection. Keep it queued until the state change.
            if (connection.State != ConnectionState.Connected)
            {
                return 0;
            }

            var count = 0;
            while (count < maxMessages && connection.Incoming.TryDequeue(out var packet))
            {
                Interlocked.Decrement(ref connection.IncomingCount);
                var message = SteamNetworkingMessageStore.CreateReceived(packet.Payload, connection.RemoteSteamId, connection.Handle, 0, connection.UserData, packet.MessageNumber);
                Marshal.WriteIntPtr(output, count * IntPtr.Size, message);
                count++;
            }
            return count;
        }

        private bool SendFrame(ulong remoteSteamId, int virtualPort, string transport, byte[] payload, int sendFlags = 0)
        {
            return remoteSteamId != 0 && APIClient.SendP2PPacket(remoteSteamId, payload, sendFlags, 0, transport, virtualPort);
        }

        private void QueueLoopback(uint targetHandle, byte[] payload)
        {
            SocketConnection target;
            lock (_gate)
            {
                if (!_connections.TryGetValue(targetHandle, out target) || IsTerminal(target.State))
                {
                    return;
                }
            }
            if (Interlocked.Increment(ref target.IncomingCount) <= MaxQueuedMessagesPerConnection)
            {
                target.Incoming.Enqueue(new IncomingPacket { Payload = payload, MessageNumber = Interlocked.Increment(ref _nextMessageNumber) });
            }
            else
            {
                Interlocked.Decrement(ref target.IncomingCount);
            }
        }

        private void CloseLoopbackPeer(uint targetHandle, int reason, string debug)
        {
            SocketConnection target;
            lock (_gate)
            {
                if (!_connections.TryGetValue(targetHandle, out target) || IsTerminal(target.State))
                {
                    return;
                }
                var oldState = target.State;
                target.State = ConnectionState.ClosedByPeer;
                target.EndReason = reason;
                target.Debug = debug;
                NotifyStateChange(target, oldState);
            }
        }

        private void NotifyStateChange(SocketConnection connection, ConnectionState oldState)
        {
            SteamNetworkingUtils.Instance?.NotifyConnectionStateChange(connection.Handle, CreateInfo(connection), oldState);
        }

        private static SteamNetConnectionInfo_t CreateInfo(SocketConnection connection)
        {
            return SteamNetworkingMessages.CreateConnectionInfo(connection.RemoteSteamId, connection.State, connection.ListenSocket, connection.UserData, connection.EndReason, connection.Debug);
        }

        private bool HasConnection(uint handle)
        {
            lock (_gate)
            {
                return _connections.ContainsKey(handle);
            }
        }

        private uint AllocateHandleLocked()
        {
            do
            {
                _nextHandle = _nextHandle == int.MaxValue ? 1 : _nextHandle + 1;
            }
            while (_listenSockets.ContainsKey((uint)_nextHandle) || _connections.ContainsKey((uint)_nextHandle) || _pollGroups.Contains((uint)_nextHandle));
            return (uint)_nextHandle;
        }

        private static bool IsTerminal(ConnectionState state)
        {
            return state == ConnectionState.ClosedByPeer || state == ConnectionState.ProblemDetectedLocally || state == ConnectionState.Dead;
        }

        private long GetLastMessageNumber(IntPtr message)
        {
            if (!SteamNetworkingMessageStore.TryRead(message, out var native, out var ignored))
            {
                return 0;
            }
            lock (_gate)
            {
                return _connections.TryGetValue(native.m_conn, out var connection) ? connection.NextOutboundMessageNumber : 0;
            }
        }
    }
}
