using SKYNET;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamNetworkingSocketsSerialized : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PRendezvous(IntPtr steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PRendezvous");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PConnectionFailure(IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PConnectionFailure");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamNetworkingSocketsSerialized_GetCertAsync()
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCertAsync");
        return SteamEmulator.SteamNetworkingSocketsSerialized.GetCertAsync();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSocketsSerialized_GetNetworkConfigJSON(IntPtr buf, uint cbBuf)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetNetworkConfigJSON");
        return SteamEmulator.SteamNetworkingSocketsSerialized.GetNetworkConfigJSON(buf, cbBuf);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSocketsSerialized_CacheRelayTicket(IntPtr pTicket, uint cbTicket)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_CacheRelayTicket");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicketCount()
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicketCount");
        return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicketCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicket");
        return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicket(idxTicket, buf, cbBuf);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSocketsSerialized_PostConnectionStateMsg(IntPtr pMsg, uint cbMsg)
    {
        Write("SteamAPI_ISteamNetworkingSocketsSerialized_PostConnectionStateMsg");
        //
    }

}

