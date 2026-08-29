using SKYNET.Callback;
using SKYNET.Helpers;
//using SKYNET.IPC.Types;
using SKYNET.Network.Packets;
using SKYNET.Managers;
using SKYNET.Protocol;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

using SNetListenSocket_t = System.UInt32;
using SNetSocket_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworking : ISteamInterface
    {
        public static SteamNetworking Instance;
        private static List<ulong> P2PSession;
        private const int SocketStateInvalid = 0;
        private const int SocketStateConnected = 1;
        private const int SocketStateInitiated = 10;
        private const int SocketStateLocalDisconnect = 22;
        private const int SocketStateTimeoutDuringConnect = 23;
        private const int SocketStateRemoteEndDisconnected = 24;
        private const int MaxQueuedSocketPackets = 2048;

        private readonly object _socketGate = new object();
        private readonly Dictionary<uint, LegacyListenSocket> _listenSockets = new Dictionary<uint, LegacyListenSocket>();
        private readonly Dictionary<uint, LegacySocket> _sockets = new Dictionary<uint, LegacySocket>();
        private int _nextSocketHandle;

        public List<NET_P2PPacket> P2PIncoming;

        private sealed class LegacyListenSocket
        {
            internal uint Handle;
            internal int VirtualPort;
            internal uint IP;
            internal ushort Port;
        }

        private sealed class LegacySocket
        {
            internal uint Handle;
            internal uint ListenSocket;
            internal ulong RemoteSteamId;
            internal int VirtualPort;
            internal uint PeerConnectionId;
            internal uint RemoteIP = 0;
            internal ushort RemotePort = 0;
            internal int State;
            internal readonly ConcurrentQueue<byte[]> Incoming = new ConcurrentQueue<byte[]>();
            internal int IncomingCount;
        }

        public SteamNetworking()
        {
            Instance = this;
            InterfaceName = "SteamNetworking";
            InterfaceVersion = "SteamNetworking006";
            P2PIncoming = new List<NET_P2PPacket>();
            P2PSession = new List<ulong>();
        }

        internal void ProcessP2PPacket(NET_P2PPacket P2PPacket)
        {
            ulong steamIDRemote = (ulong)new CSteamID(P2PPacket.Sender);
            MutexHelper.Wait("P2PPacket", delegate
            {
                if (!P2PSession.Contains(steamIDRemote))
                {
                    P2PSessionRequest_t data = new P2PSessionRequest_t()
                    {
                        m_steamIDRemote = steamIDRemote
                    };
                    CallbackManager.AddCallback(data);
                    P2PSession.Add(steamIDRemote);
                }
            });
            AddP2PPacket(P2PPacket);
        }

        public bool SendP2PPacket(ulong steamIDRemote, IntPtr pubData, uint cubData, int eP2PSendType, int nChannel)
        {
            Write("SendP2PPacket");
            if (pubData == IntPtr.Zero)
            {
                return false;
            }
            byte[] bytes = pubData.GetBytes(cubData);
            if (APIClient.IsEnabled)
            {
                return APIClient.SendP2PPacket(steamIDRemote, bytes, eP2PSendType, nChannel);
            }

            NetworkManager.SendP2PTo(steamIDRemote, bytes, eP2PSendType, nChannel);
            return true;
        }

        public bool IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
        {
            Write("IsP2PPacketAvailable");
            NET_P2PPacket packet = null;
            MutexHelper.Wait("P2PPacket", delegate
            {
                packet = P2PIncoming.Find(p => p.Channel == nChannel);
            });

            if (packet == null)
            {
                pcubMsgSize = 0;
                return false;
            }

            pcubMsgSize = (uint)packet.Buffer.GetBytesFromBase64String().Length;
            return true;
        }

        public bool ReadP2PPacket( IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref ulong psteamIDRemote, int nChannel)
        {
            Write("ReadP2PPacket");
            NET_P2PPacket packet = null;
            MutexHelper.Wait("P2PPacket", delegate
            {
                packet = P2PIncoming.Find(p => p.Channel == nChannel);
            });

            if (packet == null)
            {
                pcubMsgSize = 0;
                psteamIDRemote = 0;
                return false;
            }

            var bytes = packet.Buffer.GetBytesFromBase64String();
            pcubMsgSize = (uint)bytes.Length;
            psteamIDRemote = (ulong)new CSteamID(packet.Sender);

            if (cubDest < bytes.Length || pubDest == IntPtr.Zero)
            {
                return false;
            }

            Marshal.Copy(bytes, 0, pubDest, bytes.Length);
            MutexHelper.Wait("P2PPacket", delegate
            {
                P2PIncoming.Remove(packet);
            });
            return true;
        }

        public bool AcceptP2PSessionWithUser(ulong steamIDRemote)
        {
            Write($"AcceptP2PSessionWithUser (User SteamID = {steamIDRemote})");
            if (!P2PSession.Contains(steamIDRemote))
            {
                P2PSession.Add(steamIDRemote);
            }
            return true;
        }

        public bool CloseP2PSessionWithUser(ulong steamIDRemote)
        {
            Write($"CloseP2PSessionWithUser (User SteamID = {steamIDRemote})");
            P2PSession.Remove(steamIDRemote);
            return true;
        }

        public bool CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel)
        {
            Write($"CloseP2PChannelWithUser (User SteamID = {steamIDRemote})");
            return true;
        }

        public bool GetP2PSessionState(ulong steamIDRemote, IntPtr ptrConnectionState)
        {
            Write($"GetP2PSessionState {steamIDRemote}");

            P2PSessionState_t pConnectionState = Marshal.PtrToStructure<P2PSessionState_t>(ptrConnectionState);
            pConnectionState.m_bConnectionActive = 1;
            pConnectionState.m_bConnecting = 0;
            pConnectionState.m_eP2PSessionError = 0;
            pConnectionState.m_bUsingRelay = 0;
            pConnectionState.m_nBytesQueuedForSend = 0;
            pConnectionState.m_nPacketsQueuedForSend = 0;
            pConnectionState.m_nRemoteIP = NetworkManager.GetIPAddress(NetworkManager.GetIPAddress()); ;
            pConnectionState.m_nRemotePort = 27015;

            Marshal.StructureToPtr(pConnectionState, ptrConnectionState, false);

            return true;
        }

        public bool AllowP2PPacketRelay(bool bAllow)
        {
            Write("AllowP2PPacketRelay");
            return true;
        }

        public SNetListenSocket_t CreateListenSocket(int nVirtualP2PPort, uint nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            return CreateLegacyListenSocket(nVirtualP2PPort, nIP, unchecked((ushort)nPort));
        }

        public SNetListenSocket_t CreateListenSocket(int nVirtualP2PPort, SteamIPAddress_t nIP, ushort nPort, bool bAllowUseOfPacketRelay)
        {
            var address = nIP.ToIPAddress();
            var ipv4 = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                ? NetworkManager.GetIPAddress(address)
                : 0;
            return CreateLegacyListenSocket(nVirtualP2PPort, ipv4, nPort);
        }

        public SNetSocket_t CreateP2PConnectionSocket(ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            if (steamIDTarget == 0 || !APIClient.IsEnabled)
            {
                Write($"CreateP2PConnectionSocket rejected target={steamIDTarget} API={APIClient.IsEnabled}");
                return 0;
            }

            LegacySocket socket;
            lock (_socketGate)
            {
                socket = new LegacySocket
                {
                    Handle = AllocateSocketHandleLocked(),
                    RemoteSteamId = steamIDTarget,
                    VirtualPort = nVirtualPort,
                    State = SocketStateInitiated
                };
                _sockets.Add(socket.Handle, socket);
            }

            Write($"CreateP2PConnectionSocket handle={socket.Handle} target={steamIDTarget} virtualPort={nVirtualPort}");
            if (!SendSocketFrame(socket, P2PTransportKind.LegacySocketsOpen, Array.Empty<byte>()))
            {
                lock (_socketGate)
                {
                    socket.State = SocketStateTimeoutDuringConnect;
                }
                EmitSocketStatus(socket);
            }
            return socket.Handle;
        }

        public SNetSocket_t CreateConnectionSocket(uint nIP, uint nPort, int nTimeoutSec)
        {
            Write($"CreateConnectionSocket unsupported IP={nIP} port={nPort}");
            return 0;
        }

        public SNetSocket_t CreateConnectionSocket(SteamIPAddress_t nIP, ushort nPort, int nTimeoutSec)
        {
            Write($"CreateConnectionSocket unsupported IP={nIP} port={nPort}");
            return 0;
        }

        public bool DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            LegacySocket socket;
            lock (_socketGate)
            {
                if (!_sockets.TryGetValue(hSocket, out socket))
                {
                    return false;
                }
                socket.State = SocketStateLocalDisconnect;
                _sockets.Remove(hSocket);
            }

            Write($"DestroySocket handle={hSocket}");
            if (bNotifyRemoteEnd && socket.RemoteSteamId != 0)
            {
                SendSocketFrame(socket, P2PTransportKind.LegacySocketsClose, Array.Empty<byte>());
            }
            EmitSocketStatus(socket);
            return true;
        }

        public bool DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            var affected = new List<LegacySocket>();
            lock (_socketGate)
            {
                if (!_listenSockets.Remove(hSocket))
                {
                    return false;
                }

                foreach (var pair in new List<KeyValuePair<uint, LegacySocket>>(_sockets))
                {
                    if (pair.Value.ListenSocket == hSocket)
                    {
                        pair.Value.State = SocketStateLocalDisconnect;
                        affected.Add(pair.Value);
                        _sockets.Remove(pair.Key);
                    }
                }
            }

            Write($"DestroyListenSocket handle={hSocket} connections={affected.Count}");
            foreach (var socket in affected)
            {
                if (bNotifyRemoteEnd)
                {
                    SendSocketFrame(socket, P2PTransportKind.LegacySocketsClose, Array.Empty<byte>());
                }
                EmitSocketStatus(socket);
            }
            return true;
        }

        public bool SendDataOnSocket(SNetSocket_t hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            if ((cubData > 0 && pubData == IntPtr.Zero) || cubData > 1024 * 1024)
            {
                return false;
            }

            LegacySocket socket;
            lock (_socketGate)
            {
                if (!_sockets.TryGetValue(hSocket, out socket) || socket.State != SocketStateConnected)
                {
                    return false;
                }
            }

            var payload = new byte[cubData];
            if (payload.Length > 0)
            {
                Marshal.Copy(pubData, payload, 0, payload.Length);
            }
            return SendSocketFrame(socket, P2PTransportKind.LegacySocketsData, payload, bReliable ? 2 : 0);
        }

        public bool IsDataAvailableOnSocket(SNetSocket_t hSocket, uint pcubMsgSize)
        {
            lock (_socketGate)
            {
                return _sockets.TryGetValue(hSocket, out var socket) && socket.Incoming.TryPeek(out _);
            }
        }

        public bool IsDataAvailableOnSocket(SNetSocket_t hSocket, IntPtr pcubMsgSize)
        {
            LegacySocket socket;
            lock (_socketGate)
            {
                if (!_sockets.TryGetValue(hSocket, out socket) || !socket.Incoming.TryPeek(out var payload))
                {
                    WriteUInt32(pcubMsgSize, 0);
                    return false;
                }
                WriteUInt32(pcubMsgSize, (uint)payload.Length);
                return true;
            }
        }

        public bool RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return RetrieveDataFromSocket(hSocket, pubDest, cubDest, IntPtr.Zero);
        }

        public bool RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, IntPtr pcubMsgSize)
        {
            LegacySocket socket;
            lock (_socketGate)
            {
                if (!_sockets.TryGetValue(hSocket, out socket))
                {
                    WriteUInt32(pcubMsgSize, 0);
                    return false;
                }
            }

            if (!socket.Incoming.TryDequeue(out var payload))
            {
                WriteUInt32(pcubMsgSize, 0);
                return false;
            }

            Interlocked.Decrement(ref socket.IncomingCount);
            WriteUInt32(pcubMsgSize, (uint)payload.Length);
            var copyLength = Math.Min(payload.Length, unchecked((int)cubDest));
            if (copyLength > 0 && pubDest != IntPtr.Zero)
            {
                Marshal.Copy(payload, 0, pubDest, copyLength);
            }
            return true;
        }

        public bool IsDataAvailable(SNetListenSocket_t hListenSocket, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            return TryFindSocketWithData(hListenSocket, out _, out _);
        }

        public bool IsDataAvailable(SNetListenSocket_t hListenSocket, IntPtr pcubMsgSize, IntPtr phSocket)
        {
            if (!TryFindSocketWithData(hListenSocket, out var socket, out var payload))
            {
                WriteUInt32(pcubMsgSize, 0);
                WriteUInt32(phSocket, 0);
                return false;
            }
            WriteUInt32(pcubMsgSize, (uint)payload.Length);
            WriteUInt32(phSocket, socket.Handle);
            return true;
        }

        public bool RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            if (!TryFindSocketWithData(hListenSocket, out var socket, out _))
            {
                return false;
            }
            return RetrieveDataFromSocket(socket.Handle, pubDest, cubDest, IntPtr.Zero);
        }

        public bool RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, IntPtr pcubMsgSize, IntPtr phSocket)
        {
            if (!TryFindSocketWithData(hListenSocket, out var socket, out _))
            {
                WriteUInt32(pcubMsgSize, 0);
                WriteUInt32(phSocket, 0);
                return false;
            }
            WriteUInt32(phSocket, socket.Handle);
            return RetrieveDataFromSocket(socket.Handle, pubDest, cubDest, pcubMsgSize);
        }

        public bool GetSocketInfo(SNetSocket_t hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            lock (_socketGate)
            {
                return _sockets.ContainsKey(hSocket);
            }
        }

        public bool GetSocketInfo(SNetSocket_t hSocket, IntPtr pSteamIDRemote, IntPtr peSocketStatus, IntPtr punIPRemote, IntPtr punPortRemote)
        {
            lock (_socketGate)
            {
                if (!_sockets.TryGetValue(hSocket, out var socket))
                {
                    WriteUInt64(pSteamIDRemote, 0);
                    WriteInt32(peSocketStatus, SocketStateInvalid);
                    WriteUInt32(punIPRemote, 0);
                    WriteUInt16(punPortRemote, 0);
                    return false;
                }
                WriteUInt64(pSteamIDRemote, socket.RemoteSteamId);
                WriteInt32(peSocketStatus, socket.State);
                WriteUInt32(punIPRemote, socket.RemoteIP);
                WriteUInt16(punPortRemote, socket.RemotePort);
                return true;
            }
        }

        public bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, uint pnIP, uint pnPort)
        {
            lock (_socketGate)
            {
                return _listenSockets.ContainsKey(hListenSocket);
            }
        }

        public bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, IntPtr pnIP, IntPtr pnPort)
        {
            lock (_socketGate)
            {
                if (!_listenSockets.TryGetValue(hListenSocket, out var listener))
                {
                    WriteUInt32(pnIP, 0);
                    WriteUInt16(pnPort, 0);
                    return false;
                }
                WriteUInt32(pnIP, listener.IP);
                WriteUInt16(pnPort, listener.Port);
                return true;
            }
        }

        public int GetSocketConnectionType(SNetSocket_t hSocket)
        {
            lock (_socketGate)
            {
                return _sockets.TryGetValue(hSocket, out var socket) && socket.State == SocketStateConnected
                    ? (int)ESNetSocketConnectionType.k_ESNetSocketConnectionTypeUDPRelay
                    : (int)ESNetSocketConnectionType.k_ESNetSocketConnectionTypeNotConnected;
            }
        }

        public int GetMaxPacketSize(SNetSocket_t hSocket)
        {
            return 1200;
        }

        internal void ProcessRelaySocketPacket(
            P2PTransportKind transport,
            ulong remoteSteamId,
            int virtualPort,
            uint sourceConnectionId,
            uint targetConnectionId,
            byte[] payload)
        {
            switch (transport)
            {
                case P2PTransportKind.LegacySocketsOpen:
                    ProcessSocketOpen(remoteSteamId, virtualPort, sourceConnectionId);
                    break;
                case P2PTransportKind.LegacySocketsAccept:
                    ProcessSocketAccept(remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId);
                    break;
                case P2PTransportKind.LegacySocketsReject:
                    ProcessSocketClosed(remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId, SocketStateTimeoutDuringConnect);
                    break;
                case P2PTransportKind.LegacySocketsClose:
                    ProcessSocketClosed(remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId, SocketStateRemoteEndDisconnected);
                    break;
                case P2PTransportKind.LegacySocketsData:
                    ProcessSocketData(remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId, payload ?? Array.Empty<byte>());
                    break;
            }
        }

        private SNetListenSocket_t CreateLegacyListenSocket(int virtualPort, uint ip, ushort port)
        {
            lock (_socketGate)
            {
                foreach (var existing in _listenSockets.Values)
                {
                    if (existing.VirtualPort == virtualPort && existing.IP == ip && existing.Port == port)
                    {
                        Write($"CreateListenSocket reused handle={existing.Handle} virtualPort={virtualPort} IP={ip} port={port}");
                        return existing.Handle;
                    }
                }

                var listener = new LegacyListenSocket
                {
                    Handle = AllocateSocketHandleLocked(),
                    VirtualPort = virtualPort,
                    IP = ip,
                    Port = port
                };
                _listenSockets.Add(listener.Handle, listener);
                Write($"CreateListenSocket handle={listener.Handle} virtualPort={virtualPort} IP={ip} port={port}");
                return listener.Handle;
            }
        }

        private void ProcessSocketOpen(ulong remoteSteamId, int virtualPort, uint sourceConnectionId)
        {
            LegacySocket socket = null;
            lock (_socketGate)
            {
                foreach (var existing in _sockets.Values)
                {
                    if (existing.ListenSocket != 0 &&
                        existing.RemoteSteamId == remoteSteamId &&
                        existing.VirtualPort == virtualPort &&
                        existing.PeerConnectionId == sourceConnectionId)
                    {
                        return;
                    }
                }

                LegacyListenSocket listener = null;
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
                    socket = new LegacySocket
                    {
                        Handle = AllocateSocketHandleLocked(),
                        ListenSocket = listener.Handle,
                        RemoteSteamId = remoteSteamId,
                        VirtualPort = virtualPort,
                        PeerConnectionId = sourceConnectionId,
                        State = SocketStateConnected
                    };
                    _sockets.Add(socket.Handle, socket);
                }
            }

            if (socket == null)
            {
                SendSocketFrame(
                    remoteSteamId,
                    virtualPort,
                    P2PTransportKind.LegacySocketsReject,
                    Array.Empty<byte>(),
                    targetConnectionId: sourceConnectionId);
                return;
            }

            Write($"Accepted legacy socket handle={socket.Handle} remote={remoteSteamId} virtualPort={virtualPort}");
            SendSocketFrame(
                remoteSteamId,
                virtualPort,
                P2PTransportKind.LegacySocketsAccept,
                Array.Empty<byte>(),
                sourceConnectionId: socket.Handle,
                targetConnectionId: sourceConnectionId);
            EmitSocketStatus(socket);
        }

        private void ProcessSocketAccept(ulong remoteSteamId, int virtualPort, uint sourceConnectionId, uint targetConnectionId)
        {
            LegacySocket socket = null;
            lock (_socketGate)
            {
                if (_sockets.TryGetValue(targetConnectionId, out var candidate) &&
                    candidate.ListenSocket == 0 &&
                    candidate.RemoteSteamId == remoteSteamId &&
                    candidate.VirtualPort == virtualPort &&
                    candidate.State == SocketStateInitiated)
                {
                    candidate.PeerConnectionId = sourceConnectionId;
                    candidate.State = SocketStateConnected;
                    socket = candidate;
                }
            }

            if (socket != null)
            {
                Write($"Connected legacy socket handle={socket.Handle} remote={remoteSteamId} virtualPort={virtualPort}");
                EmitSocketStatus(socket);
            }
        }

        private void ProcessSocketClosed(
            ulong remoteSteamId,
            int virtualPort,
            uint sourceConnectionId,
            uint targetConnectionId,
            int state)
        {
            LegacySocket socket = null;
            lock (_socketGate)
            {
                foreach (var candidate in _sockets.Values)
                {
                    if (MatchesSocket(candidate, remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId))
                    {
                        candidate.State = state;
                        socket = candidate;
                        break;
                    }
                }
            }

            if (socket != null)
            {
                EmitSocketStatus(socket);
            }
        }

        private void ProcessSocketData(
            ulong remoteSteamId,
            int virtualPort,
            uint sourceConnectionId,
            uint targetConnectionId,
            byte[] payload)
        {
            LegacySocket socket = null;
            lock (_socketGate)
            {
                foreach (var candidate in _sockets.Values)
                {
                    if (MatchesSocket(candidate, remoteSteamId, virtualPort, sourceConnectionId, targetConnectionId) &&
                        candidate.State == SocketStateConnected)
                    {
                        socket = candidate;
                        break;
                    }
                }
            }

            if (socket == null || Interlocked.Increment(ref socket.IncomingCount) > MaxQueuedSocketPackets)
            {
                if (socket != null)
                {
                    Interlocked.Decrement(ref socket.IncomingCount);
                }
                Write("Dropping legacy socket packet because the connection is unknown or its queue is full");
                return;
            }
            socket.Incoming.Enqueue(payload);
        }

        private bool TryFindSocketWithData(uint listenSocket, out LegacySocket socket, out byte[] payload)
        {
            socket = null;
            payload = null;
            lock (_socketGate)
            {
                if (!_listenSockets.ContainsKey(listenSocket))
                {
                    return false;
                }

                foreach (var candidate in _sockets.Values)
                {
                    if (candidate.ListenSocket == listenSocket && candidate.Incoming.TryPeek(out payload))
                    {
                        socket = candidate;
                        return true;
                    }
                }
                return false;
            }
        }

        private bool SendSocketFrame(
            LegacySocket socket,
            P2PTransportKind transport,
            byte[] payload,
            int sendType = 0)
        {
            return SendSocketFrame(
                socket.RemoteSteamId,
                socket.VirtualPort,
                transport,
                payload,
                sendType,
                socket.Handle,
                socket.PeerConnectionId);
        }

        private static bool SendSocketFrame(
            ulong remoteSteamId,
            int virtualPort,
            P2PTransportKind transport,
            byte[] payload,
            int sendType = 0,
            uint sourceConnectionId = 0,
            uint targetConnectionId = 0)
        {
            return remoteSteamId != 0 && APIClient.SendP2PPacket(
                remoteSteamId,
                payload,
                sendType,
                0,
                transport,
                virtualPort,
                sourceConnectionId,
                targetConnectionId);
        }

        private static bool MatchesSocket(
            LegacySocket socket,
            ulong remoteSteamId,
            int virtualPort,
            uint sourceConnectionId,
            uint targetConnectionId)
        {
            if (socket.RemoteSteamId != remoteSteamId || socket.VirtualPort != virtualPort)
            {
                return false;
            }
            if (targetConnectionId != 0 && socket.Handle != targetConnectionId)
            {
                return false;
            }
            return sourceConnectionId == 0 ||
                   socket.PeerConnectionId == 0 ||
                   socket.PeerConnectionId == sourceConnectionId;
        }

        private uint AllocateSocketHandleLocked()
        {
            while (true)
            {
                var handle = unchecked((uint)Interlocked.Increment(ref _nextSocketHandle));
                if (handle != 0 && !_listenSockets.ContainsKey(handle) && !_sockets.ContainsKey(handle))
                {
                    return handle;
                }
            }
        }

        private static void EmitSocketStatus(LegacySocket socket)
        {
            CallbackManager.AddCallback(new SocketStatusCallback_t
            {
                m_hSocket = socket.Handle,
                m_hListenSocket = socket.ListenSocket,
                m_steamIDRemote = socket.RemoteSteamId,
                m_eSNetSocketState = socket.State
            });
        }

        public void AddP2PPacket(NET_P2PPacket p2p)
        {
            MutexHelper.Wait("P2PPacket", delegate
            {
                P2PIncoming.Add(p2p);
            });
        }

        private static void WriteInt32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, value);
            }
        }

        private static void WriteUInt16(IntPtr destination, ushort value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt16(destination, unchecked((short)value));
            }
        }

        private static void WriteUInt32(IntPtr destination, uint value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, unchecked((int)value));
            }
        }

        private static void WriteUInt64(IntPtr destination, ulong value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt64(destination, unchecked((long)value));
            }
        }

        private static void WriteSteamIPAddress(IntPtr destination, SteamIPAddress_t value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.StructureToPtr(value, destination, false);
            }
        }
    }
}
