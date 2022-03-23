using Core.Interface;
using SKYNET.Interface;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamNetworkingSockets")]
    public class DSteamNetworkingSockets 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint ConnectByIPAddress(IntPtr address, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateListenSocketP2P(int nVirtualPort, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint ConnectP2P(IntPtr identityRemote, int nVirtualPort, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EResult AcceptConnection(uint hConn);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseConnection(uint hPeer, int nReason, char pszDebug, bool bEnableLinger);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseListenSocket(uint hSocket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConnectionUserData(uint hPeer, uint nUserData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetConnectionUserData(uint hPeer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetConnectionName(uint hPeer, char pszName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetConnectionName(uint hPeer, char pszName, int nMaxLen);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EResult SendMessageToConnection(uint hConn, IntPtr pData, uint cbData, int nSendFlags, uint pOutMessageNumber);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SendMessages(int nMessages, IntPtr pMessages, uint pOutMessageNumberOrResult);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EResult FlushMessagesOnConnection(uint hConn);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int ReceiveMessagesOnConnection(uint hConn, IntPtr ppOutMessages, int nMaxMessages);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetConnectionInfo(uint hConn, IntPtr pInfo);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQuickConnectionStatus(uint hConn, IntPtr pStats);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDetailedConnectionStatus(uint hConn, char pszBuf, int cbBuf);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetListenSocketAddress(uint hSocket, IntPtr address);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CreateSocketPair(uint pOutConnection1, uint pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetIdentity(IntPtr pIdentity);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamNetworkingAvailability InitAuthentication(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamNetworkingAvailability GetAuthenticationStatus(IntPtr pDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreatePollGroup(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DestroyPollGroup(uint hPollGroup);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConnectionPollGroup(uint hConn, uint hPollGroup);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int ReceiveMessagesOnPollGroup(uint hPollGroup, IntPtr ppOutMessages, int nMaxMessages);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, SteamDatagramRelayAuthTicket pOutParsedTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int FindRelayAuthTicketForServer(IntPtr identityGameServer, int nVirtualPort, SteamDatagramRelayAuthTicket pOutParsedTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint ConnectToHostedDedicatedServer(IntPtr identityTarget, int nVirtualPort, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetHostedDedicatedServerPort(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetHostedDedicatedServerPOPID(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EResult GetHostedDedicatedServerAddress(SteamDatagramHostedAddress pRouting);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EResult GetGameCoordinatorServerLogin(SteamDatagramGameCoordinatorServerLogin pLoginInfo, int pcbSignedBlob, IntPtr pBlob);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint ConnectP2PCustomSignaling(IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetCertificateRequest(int pcbBlob, IntPtr pBlob, string errMsg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCertificate(IntPtr pCertificate, int cbCertificate, string errMsg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunCallbacks(IntPtr pCallbacks);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SteamDatagramClient_Init(bool bNoSteamSupport, IntPtr errMsg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SteamDatagramServer_Init(bool bNoSteamSupport, IntPtr errMsg);
    }
}
