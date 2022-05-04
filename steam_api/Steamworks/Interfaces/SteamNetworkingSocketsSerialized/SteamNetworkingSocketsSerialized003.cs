using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamNetworkingSocketsSerialized003")]
    public class SteamNetworkingSocketsSerialized003 : ISteamInterface
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
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicketCount();
        }

        public int GetCachedRelayTicket(IntPtr _, uint idxTicket, IntPtr buf, uint cbBuf)
        {
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicket(idxTicket, buf, cbBuf);
        }

        public void PostConnectionStateMsg(IntPtr _, IntPtr pMsg, uint cbMsg)
        {
            SteamEmulator.SteamNetworkingSocketsSerialized.PostConnectionStateMsg(pMsg, cbMsg);
        }

    }
}
