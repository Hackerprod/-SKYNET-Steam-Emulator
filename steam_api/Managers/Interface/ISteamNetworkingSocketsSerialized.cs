using Steamworks;
using System;

namespace SKYNET.Interface
{
    public interface ISteamNetworkingSocketsSerialized
    {
        void SendP2PRendezvous(IntPtr steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous);
        void SendP2PConnectionFailure(IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason);
        //SteamNetworkingSocketsCert_t
        SteamAPICall_t GetCertAsync();
        int GetNetworkConfigJSON(IntPtr buf, uint cbBuf);
        void CacheRelayTicket(IntPtr pTicket, uint cbTicket);
        uint GetCachedRelayTicketCount();
        int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf);
        void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg);
    }
}