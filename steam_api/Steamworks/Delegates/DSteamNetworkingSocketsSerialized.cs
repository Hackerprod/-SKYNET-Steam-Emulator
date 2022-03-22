using Core.Interface;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamNetworkingSocketsSerialized")]
    public class DSteamNetworkingSocketsSerialized 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SendP2PRendezvous(IntPtr steamIDRemote, uint unConnectionIDSrc, IntPtr pMsgRendezvous, uint cbRendezvous);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SendP2PConnectionFailure(IntPtr steamIDRemote, uint unConnectionIDDest, uint nReason, char pszReason);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetCertAsync(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNetworkConfigJSON(IntPtr buf, uint cbBuf);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CacheRelayTicket(IntPtr pTicket, uint cbTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetCachedRelayTicketCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetCachedRelayTicket(uint idxTicket, IntPtr buf, uint cbBuf);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PostConnectionStateMsg(IntPtr pMsg, uint cbMsg);
    }
}