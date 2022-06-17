using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    using SteamAPICall_t = System.UInt64;
    public class SteamAPI_ISteamNetworkingSocketsSerialized
    {
        static SteamAPI_ISteamNetworkingSocketsSerialized()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PRendezvous(IntPtr _, ulong steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PRendezvous");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PConnectionFailure(IntPtr _, IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_SendP2PConnectionFailure");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamNetworkingSocketsSerialized_GetCertAsync(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCertAsync");
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCertAsync();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingSocketsSerialized_GetNetworkConfigJSON(IntPtr _, IntPtr buf, uint cbBuf)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetNetworkConfigJSON");
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetNetworkConfigJSON(buf, cbBuf);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingSocketsSerialized_CacheRelayTicket(IntPtr _, IntPtr pTicket, uint cbTicket)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_CacheRelayTicket");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicketCount(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicketCount");
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicketCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicket(IntPtr _, uint idxTicket, IntPtr buf, uint cbBuf)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_GetCachedRelayTicket");
            return SteamEmulator.SteamNetworkingSocketsSerialized.GetCachedRelayTicket(idxTicket, buf, cbBuf);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingSocketsSerialized_PostConnectionStateMsg(IntPtr _, IntPtr pMsg, uint cbMsg)
        {
            Write("SteamAPI_ISteamNetworkingSocketsSerialized_PostConnectionStateMsg");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

