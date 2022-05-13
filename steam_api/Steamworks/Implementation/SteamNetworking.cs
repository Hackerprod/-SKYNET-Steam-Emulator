using SKYNET;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworking : ISteamInterface
    {
        public List<NET_P2PPacket> P2PIncoming { get; set; }

        public SteamNetworking()
        {
            InterfaceName = "SteamNetworking";
            InterfaceVersion = "SteamNetworking005";
            P2PIncoming = new List<NET_P2PPacket>();
        }

        public bool SendP2PPacket(ulong steamIDRemote, IntPtr pubData, uint cubData, int eP2PSendType, int nChannel)
        {
            Write("SendP2PPacket");
            if (pubData == IntPtr.Zero)
            {
                return false;
            }
            byte[] bytes = pubData.GetBytes(cubData);
            NetworkManager.SendP2PTo(steamIDRemote, bytes, eP2PSendType, nChannel);

            return true;
        }

        public bool IsP2PPacketAvailable(uint pcubMsgSize, int nChannel)
        {
            Write("IsP2PPacketAvailable");
            return P2PIncoming.Any();
        }

        public bool ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, ulong psteamIDRemote, int nChannel)
        {
            Write("ReadP2PPacket");
            if (P2PIncoming.Any())
            {
                var packet = P2PIncoming[0];
                Marshal.Copy(packet.Buffer.GetBytesFromBase64String(), 0, pubDest, packet.Buffer.GetBytesFromBase64String().Length);
                P2PIncoming.RemoveAt(0);
            }
            return false;
        }

        public bool AcceptP2PSessionWithUser(ulong steamIDRemote)
        {
            Write("AcceptP2PSessionWithUser");
            return true;
        }

        public bool CloseP2PSessionWithUser(ulong steamIDRemote)
        {
            Write("CloseP2PSessionWithUser");
            return false;
        }

        public bool CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel)
        {
            Write("CloseP2PChannelWithUser");
            return false;
        }

        public bool GetP2PSessionState(ulong steamIDRemote, IntPtr ptrConnectionState)
        {
            Write($"GetP2PSessionState {steamIDRemote}");

            P2PSessionState_t pConnectionState = Marshal.PtrToStructure<P2PSessionState_t>(ptrConnectionState);
            pConnectionState.m_bConnectionActive = true;
            pConnectionState.m_bConnecting = false;
            pConnectionState.m_eP2PSessionError = 0;
            pConnectionState.m_bUsingRelay = false;
            pConnectionState.m_nBytesQueuedForSend = 0;
            pConnectionState.m_nPacketsQueuedForSend = 0;
            pConnectionState.m_nRemoteIP = NetworkManager.GetIPAddress(NetworkManager.GetIPAddress());
            pConnectionState.m_nRemotePort = 208802;

            Marshal.StructureToPtr(pConnectionState, ptrConnectionState, false);

            return true;
        }

        public bool AllowP2PPacketRelay(bool bAllow)
        {
            Write("AllowP2PPacketRelay");
            return false;
        }

        public uint CreateListenSocket(int nVirtualP2PPort, uint nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            Write("CreateListenSocket");
            return 0;
        }

        public uint CreateP2PConnectionSocket(ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            Write("CreateP2PConnectionSocket");
            return 0;
        }

        public uint CreateConnectionSocket(uint nIP, uint nPort, int nTimeoutSec)
        {
            Write("CreateConnectionSocket");
            return 0;
        }

        public bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("DestroySocket");
            return false;
        }

        public bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("DestroyListenSocket");
            return false;
        }

        public bool SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            Write("SendDataOnSocket");
            return false;
        }

        public bool IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize)
        {
            Write("IsDataAvailableOnSocket");
            return false;
        }

        public bool RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("RetrieveDataFromSocket");
            return false;
        }

        public bool IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket)
        {
            Write("IsDataAvailable");
            return false;
        }

        public bool RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
        {
            Write("RetrieveData");
            return false;
        }

        public bool GetSocketInfo(uint hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            Write("GetSocketInfo");
            return false;
        }

        public bool GetListenSocketInfo(uint hListenSocket, uint pnIP, uint pnPort)
        {
            Write("GetListenSocketInfo");
            return false;
        }

        public int GetSocketConnectionType(uint hSocket)
        {
            Write("GetSocketConnectionType");
            return default;
        }

        public int GetMaxPacketSize(uint hSocket)
        {
            Write("GetMaxPacketSize");
            return 1500;
        }
    }
}