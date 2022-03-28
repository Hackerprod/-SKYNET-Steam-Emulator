using SKYNET;
using Steamworks;
using System;

public class SteamNetworkingSocketsSerialized : SteamInterface
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

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}