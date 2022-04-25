using SKYNET;
using SKYNET.Helpers;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworkingSocketsSerialized : ISteamInterface
    {
        public SteamNetworkingSocketsSerialized()
        {
            InterfaceVersion = "SteamNetworkingSocketsSerialized";
        }

        public void SendP2PRendezvous(ulong steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
        {
            Write("SendP2PRendezvous");
        }

        public void SendP2PConnectionFailure(ulong steamIDRemote, uint unConnectionIDDest, uint nReason, string pszReason)
        {
            Write("SendP2PConnectionFailure");
        }

        public ulong GetCertAsync()
        {
            Write("GetCertAsync");
            return 0;
        }

        public int GetNetworkConfigJSON(IntPtr buf, uint cbBuf)
        {
            Write("GetNetworkConfigJSON");
            return 0;
        }

        public void CacheRelayTicket(IntPtr pTicket, uint cbTicket)
        {
            Write("CacheRelayTicket");
        }

        public uint GetCachedRelayTicketCount(IntPtr _)
        {
            Write("GetCachedRelayTicketCount");
            return 0;
        }

        public int GetNetworkConfigJSON(IntPtr buf, uint cbBuf, string pszLauncherPartner)
        {
            Write("GetNetworkConfigJSON");
            return 0;
        }

        public int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf)
        {
            Write("GetCachedRelayTicket");
            return 0;
        }

        public void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg)
        {
            Write("PostConnectionStateMsg");
        }

        public bool GetSTUNServer(int dont_know, IntPtr buf, int len)
        {
            Write("GetSTUNServer");
            return false;
        }

        public bool BAllowDirectConnectToPeer(IntPtr identity)
        {
            Write("BAllowDirectConnectToPeer");
            return false;
        }

        public bool BeginAsyncRequestFakeIP(int nNumPorts)
        {

            Write("BAllowDirectConnectToPeer");
            return false;
        }
    }
}