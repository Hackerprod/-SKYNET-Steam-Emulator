using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.IPC.Types;
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

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworking : ISteamInterface
    {
        public static SteamNetworking Instance;
        private static List<ulong> P2PSession;

        public List<IPC_P2PPacket> P2PIncoming;
        public Dictionary<SNetSocket_t, Socket> P2PSocket;
        public Dictionary<SNetListenSocket_t, Socket> P2PListenSocket;

        public SteamNetworking()
        {
            Instance = this;
            InterfaceName = "SteamNetworking";
            InterfaceVersion = "SteamNetworking005";
            P2PIncoming = new List<IPC_P2PPacket>();
            P2PSocket = new Dictionary<SNetListenSocket_t, Socket>();
            P2PListenSocket = new Dictionary<SNetListenSocket_t, Socket>();
            P2PSession = new List<ulong>();
        }

        internal void ProcessP2PPacket(IPC_P2PPacket P2PPacket)
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
                    CallbackManager.AddCallbackResult(data);
                    P2PSession.Add(steamIDRemote);
                }
            });
            AddP2PPacket(P2PPacket);
        }

        public bool SendP2PPacket(ulong steamIDRemote, IntPtr pubData, uint cubData, int eP2PSendType, int nChannel)
        {
            bool Result = false;
            if (pubData != IntPtr.Zero)
            {
                byte[] bytes = pubData.GetBytes(cubData);
                Write(Encoding.Default.GetString(bytes));

                var packet = new IPC_P2PPacket()
                {
                    Sender = SteamEmulator.SteamID.AccountID,
                    AccountID = steamIDRemote.GetAccountID(),
                    Buffer = bytes,
                    Channel = nChannel,
                    P2PSendType = eP2PSendType
                };

                if (steamIDRemote == SteamEmulator.SteamID)
                {
                    MutexHelper.Wait("P2PPacket", delegate
                    {
                        P2PIncoming.Add(packet);
                        Result = true;
                    });
                }
                else
                {
                    IPCManager.SendP2PTo(steamIDRemote, packet);
                }
            }
            //Write($"SendP2PPacket (SteamID = {steamIDRemote}, Length = {cubData}, {(EP2PSend)eP2PSendType}, Channel = {nChannel}) = {Result}");
            return true;
        }

        public bool IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
        {
            bool Result = false;
            var MsgSize = 0;
            MutexHelper.Wait("P2PPacket", delegate
            {
                var Packet = P2PIncoming.Find(p => p.Channel == nChannel);
                if (Packet != null)
                {
                    byte[] bytes = Packet.Buffer;
                    MsgSize = bytes.Length;
                    Result = true;
                }
            });
            pcubMsgSize = (uint)MsgSize;
            //Write($"IsP2PPacketAvailable (MsgSize = {pcubMsgSize}, Channel = {nChannel}) = {Result}");
            return Result;
        }

        public bool ReadP2PPacket(IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref ulong psteamIDRemote, int nChannel)
        {
            bool Result = false;
            ulong IDRemote = 0;
            int MsgSize = 0;
            var Packet = P2PIncoming.Find(p => p.Channel == nChannel);
            if (Packet != null)
            {
                byte[] bytes = Packet.Buffer;
                IDRemote = (ulong)new CSteamID(Packet.Sender);
                MsgSize = bytes.Length;
                Marshal.Copy(bytes, 0, pubDest, bytes.Length); 
                P2PIncoming.Remove(Packet);
                Result = true;
            }
            psteamIDRemote = IDRemote;
            pcubMsgSize = (uint)MsgSize;
            return Result;
        }

        public bool AcceptP2PSessionWithUser(ulong steamIDRemote)
        {
            Write($"AcceptP2PSessionWithUser (User SteamID = {steamIDRemote})");
            return true;
        }

        public bool CloseP2PSessionWithUser(ulong steamIDRemote)
        {
            Write($"CloseP2PSessionWithUser (User SteamID = {steamIDRemote})");
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

            uint RemoteIP = NetworkHelper.ConvertFromIPAddress(NetworkHelper.GetIPAddress()); 
            var user = IPCManager.GetUser(steamIDRemote);
            if (user != null)
            {
                if (IPAddress.TryParse(user.IPAddress, out var Address))
                {
                    RemoteIP = NetworkHelper.ConvertFromIPAddress(Address);
                }
            }

            P2PSessionState_t pConnectionState = Marshal.PtrToStructure<P2PSessionState_t>(ptrConnectionState);
            pConnectionState.m_bConnectionActive = 1;
            pConnectionState.m_bConnecting = 0;
            pConnectionState.m_eP2PSessionError = 0;
            pConnectionState.m_bUsingRelay = false;
            pConnectionState.m_nBytesQueuedForSend = 0;
            pConnectionState.m_nPacketsQueuedForSend = 0;
            pConnectionState.m_nRemoteIP = RemoteIP;
            pConnectionState.m_nRemotePort = 208802;

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

        public bool RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("RetrieveDataFromSocket");
            return false;
        }

        public bool IsDataAvailable(SNetListenSocket_t hListenSocket, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            Write("IsDataAvailable");
            return false;
        }

        public bool RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, SNetSocket_t phSocket)
        {
            Write("RetrieveData");
            return false;
        }

        public bool GetSocketInfo(SNetSocket_t hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            Write("GetSocketInfo");
            return false;
        }

        public bool GetListenSocketInfo(SNetListenSocket_t hListenSocket, uint pnIP, uint pnPort)
        {
            Write("GetListenSocketInfo");
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

        public void AddP2PPacket(IPC_P2PPacket p2p)
        {
            MutexHelper.Wait("P2PPacket", delegate
            {
                P2PIncoming.Add(p2p);
            });
        }
    }
}