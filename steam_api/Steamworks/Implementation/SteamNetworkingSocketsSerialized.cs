using SKYNET;
using SKYNET.Helpers;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamNetworkingSocketsSerialized : ISteamInterface
    {
        public void SendP2PRendezvous(IntPtr steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
        {
            Write("SendP2PRendezvous");
        }

        public void SendP2PConnectionFailure(IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason)
        {
            Write("SendP2PConnectionFailure");
        }

        public SteamAPICall_t GetCertAsync(IntPtr _)
        {
            Write("GetCertAsync");
            return default;
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

        public int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf)
        {
            Write("GetCachedRelayTicket");
            return 0;
        }

        public void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg)
        {
            Write("PostConnectionStateMsg");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamNetworkingSocketsSerialized()
        {
            InterfaceVersion = "SteamNetworkingSocketsSerialized";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}