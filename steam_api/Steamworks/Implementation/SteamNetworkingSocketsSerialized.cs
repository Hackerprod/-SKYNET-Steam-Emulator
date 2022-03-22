using Core.Interface;
using SKYNET.Interface;
using Steamworks;
using System;

//[Map("SteamNetworkingSocketsSerialized")]
public class SteamNetworkingSocketsSerialized : IBaseInterface, ISteamNetworkingSocketsSerialized
{
    public void SendP2PRendezvous(IntPtr steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
    {
        //
    }

    public void SendP2PConnectionFailure(IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason)
    {
        //
    }

    public SteamAPICall_t GetCertAsync()
    {
        return default;
    }

    public int GetNetworkConfigJSON(IntPtr buf, uint cbBuf)
    {
        return 0;
    }

    public void CacheRelayTicket(IntPtr pTicket, uint cbTicket)
    {
        //
    }

    public uint GetCachedRelayTicketCount()
    {
        return 0;
    }

    public int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf)
    {
        return 0;
    }

    public void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg)
    {
        //
    }

}