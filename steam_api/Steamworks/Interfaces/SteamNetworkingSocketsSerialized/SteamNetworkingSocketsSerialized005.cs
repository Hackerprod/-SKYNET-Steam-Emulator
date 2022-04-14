using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamNetworkingSocketsSerialized005")]
    public class SteamNetworkingSocketsSerialized005 : ISteamInterface
    {
        public void SendP2PRendezvous(IntPtr _, ulong steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
        {
            SteamEmulator.SteamNetworkingSocketsSerialized.SendP2PRendezvous(steamIDRemote, unConnectionIDSrc, pMsgRendezvous, cbRendezvous);
        }

        public void SendP2PConnectionFailure(IntPtr _, ulong steamIDRemote, uint unConnectionIDDest, uint nReason, string pszReason)
        {
            SteamEmulator.SteamNetworkingSocketsSerialized.SendP2PConnectionFailure(steamIDRemote, unConnectionIDDest, nReason, pszReason);
        }

        public ulong GetCertAsync(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCertAsync();
        }

        public int GetNetworkConfigJSON(IntPtr _, IntPtr buf, uint cbBuf, string pszLauncherPartner)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetNetworkConfigJSON(buf, cbBuf, pszLauncherPartner);
        }

        public void CacheRelayTicket(IntPtr _, IntPtr pTicket, uint cbTicket)
        {
            SteamEmulator.SteamNetworkingSocketsSerialized.CacheRelayTicket(pTicket, cbTicket);
        }

        public uint GetCachedRelayTicketCount(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicketCount(_);
        }

        public int GetCachedRelayTicket(IntPtr _, uint idxTicket, IntPtr buf, uint cbBuf)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicket(idxTicket, buf, cbBuf);
        }

        public void PostConnectionStateMsg(IntPtr _, IntPtr pMsg, uint cbMsg)
        {
            SteamEmulator.SteamNetworkingSocketsSerialized.PostConnectionStateMsg(pMsg, cbMsg);
        }

        public bool GetSTUNServer(IntPtr _, int dont_know, IntPtr buf, int len)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetSTUNServer(dont_know, buf, len);
        }

        public bool BAllowDirectConnectToPeer(IntPtr _, IntPtr identity)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.BAllowDirectConnectToPeer(identity);
        }

        public bool BeginAsyncRequestFakeIP(IntPtr _, int nNumPorts)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.BeginAsyncRequestFakeIP(nNumPorts);
        }
    }
}
