using SKYNET.Callback;
using SKYNET.Helpers;
//using SKYNET.IPC.Types;
using SKYNET.Network.Packets;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

using SNetListenSocket_t = System.UInt32;
using SNetSocket_t = System.UInt32;
using System.Linq;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworking : ISteamInterface
    {
        public static SteamNetworking Instance;
        private static List<ulong> P2PSession;

        public List<NET_P2PPacket> P2PIncoming;
        public Dictionary<SNetSocket_t, Socket> P2PSocket;
        public Dictionary<SNetListenSocket_t, Socket> P2PListenSocket;

        public SteamNetworking()
        {
            Instance = this;
            InterfaceName = "SteamNetworking";
            InterfaceVersion = "SteamNetworking006";
            P2PIncoming = new List<NET_P2PPacket>();
            P2PSocket = new Dictionary<SNetListenSocket_t, Socket>();
            P2PListenSocket = new Dictionary<SNetListenSocket_t, Socket>();
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
            Write("CreateListenSocket");
            return 0;
        }

        public SNetListenSocket_t CreateListenSocket(int nVirtualP2PPort, SteamIPAddress_t nIP, ushort nPort, bool bAllowUseOfPacketRelay)
        {
            Write("CreateListenSocket");
            return 0;
        }

        public SNetSocket_t CreateP2PConnectionSocket(ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            Write("CreateP2PConnectionSocket");
            return 0;
        }

        public SNetSocket_t CreateConnectionSocket(uint nIP, uint nPort, int nTimeoutSec)
        {
            Write("CreateConnectionSocket");
            return 0;
        }

        public SNetSocket_t CreateConnectionSocket(SteamIPAddress_t nIP, ushort nPort, int nTimeoutSec)
        {
            Write("CreateConnectionSocket");
            return 0;
        }

        public bool DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            Write("DestroySocket");
            return false;
        }

        public bool DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            Write("DestroyListenSocket");
            return false;
        }

        public bool SendDataOnSocket(SNetSocket_t hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            Write("SendDataOnSocket");
            return false;
        }

        public bool IsDataAvailableOnSocket(SNetSocket_t hSocket, uint pcubMsgSize)
        {
            Write("IsDataAvailableOnSocket");
            return false;
        }

        public bool IsDataAvailableOnSocket(SNetSocket_t hSocket, IntPtr pcubMsgSize)
        {
            Write("IsDataAvailableOnSocket");
            WriteUInt32(pcubMsgSize, 0);
            return false;
        }

        public bool RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("RetrieveDataFromSocket");
            return false;
        }

        public bool RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, IntPtr pcubMsgSize)
        {
            Write("RetrieveDataFromSocket");
            WriteUInt32(pcubMsgSize, 0);
            return false;
        }

        public bool IsDataAvailable(SNetListenSocket_t hListenSocket, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            Write("IsDataAvailable");
            return false;
        }

        public bool IsDataAvailable(SNetListenSocket_t hListenSocket, IntPtr pcubMsgSize, IntPtr phSocket)
        {
            Write("IsDataAvailable");
            WriteUInt32(pcubMsgSize, 0);
            WriteUInt32(phSocket, 0);
            return false;
        }

        public bool RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            Write("RetrieveData");
            return false;
        }

        public bool RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, IntPtr pcubMsgSize, IntPtr phSocket)
        {
            Write("RetrieveData");
            WriteUInt32(pcubMsgSize, 0);
            WriteUInt32(phSocket, 0);
            return false;
        }

        public bool GetSocketInfo(SNetSocket_t hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            Write("GetSocketInfo");
            return false;
        }

        public bool GetSocketInfo(SNetSocket_t hSocket, IntPtr pSteamIDRemote, IntPtr peSocketStatus, IntPtr punIPRemote, IntPtr punPortRemote)
        {
            Write("GetSocketInfo");
            WriteUInt64(pSteamIDRemote, 0);
            WriteInt32(peSocketStatus, 0);
            WriteSteamIPAddress(punIPRemote, default);
            WriteUInt16(punPortRemote, 0);
            return false;
        }

        public bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, uint pnIP, uint pnPort)
        {
            Write("GetListenSocketInfo");
            return false;
        }

        public bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, IntPtr pnIP, IntPtr pnPort)
        {
            Write("GetListenSocketInfo");
            WriteSteamIPAddress(pnIP, default);
            WriteUInt16(pnPort, 0);
            return false;
        }

        public int GetSocketConnectionType(SNetSocket_t hSocket)
        {
            Write("GetSocketConnectionType");
            return default;
        }

        public int GetMaxPacketSize(SNetSocket_t hSocket)
        {
            Write("GetMaxPacketSize");
            return 1500;
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
