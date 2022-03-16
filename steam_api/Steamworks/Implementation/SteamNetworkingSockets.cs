using SKYNET.Interface;
using SKYNET.Types;
using System;

namespace SKYNET.Managers
{
    [Interface("SteamNetworkingSockets")]
    public class SteamNetworkingSockets : SteamInterface, ISteamNetworkingSockets
    {
        public uint CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public uint ConnectByIPAddress(IntPtr address, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public uint CreateListenSocketP2P(int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public uint ConnectP2P(IntPtr identityRemote, int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public EResult AcceptConnection(uint hConn)
        {
            return default;
        }

        public bool CloseConnection(uint hPeer, int nReason, char pszDebug, bool bEnableLinger)
        {
            return false;
        }

        public bool CloseListenSocket(uint hSocket)
        {
            return false;
        }

        public bool SetConnectionUserData(uint hPeer, uint nUserData)
        {
            return false;
        }

        public uint GetConnectionUserData(uint hPeer)
        {
            return 0;
        }

        public void SetConnectionName(uint hPeer, char pszName)
        {
            //
        }

        public bool GetConnectionName(uint hPeer, char pszName, int nMaxLen)
        {
            return false;
        }

        public EResult SendMessageToConnection(uint hConn, IntPtr pData, uint cbData, int nSendFlags, uint pOutMessageNumber)
        {
            return default;
        }

        public void SendMessages(int nMessages, IntPtr pMessages, uint pOutMessageNumberOrResult)
        {
            //
        }

        public EResult FlushMessagesOnConnection(uint hConn)
        {
            return default;
        }

        public int ReceiveMessagesOnConnection(uint hConn, IntPtr ppOutMessages, int nMaxMessages)
        {
            return 0;
        }

        public bool GetConnectionInfo(uint hConn, IntPtr pInfo)
        {
            return false;
        }

        public bool GetQuickConnectionStatus(uint hConn, IntPtr pStats)
        {
            return false;
        }

        public int GetDetailedConnectionStatus(uint hConn, char pszBuf, int cbBuf)
        {
            return 0;
        }

        public bool GetListenSocketAddress(uint hSocket, IntPtr address)
        {
            return false;
        }

        public bool CreateSocketPair(uint pOutConnection1, uint pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
        {
            return false;
        }

        public bool GetIdentity(IntPtr pIdentity)
        {
            return false;
        }

        public ESteamNetworkingAvailability InitAuthentication()
        {
            return default;
        }

        public ESteamNetworkingAvailability GetAuthenticationStatus(IntPtr pDetails)
        {
            return default;
        }

        public uint CreatePollGroup()
        {
            return 0;
        }

        public bool DestroyPollGroup(uint hPollGroup)
        {
            return false;
        }

        public bool SetConnectionPollGroup(uint hConn, uint hPollGroup)
        {
            return false;
        }

        public int ReceiveMessagesOnPollGroup(uint hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
        {
            return 0;
        }

        public bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, SteamDatagramRelayAuthTicket pOutParsedTicket)
        {
            return false;
        }

        public int FindRelayAuthTicketForServer(IntPtr identityGameServer, int nVirtualPort, SteamDatagramRelayAuthTicket pOutParsedTicket)
        {
            return 0;
        }

        public uint ConnectToHostedDedicatedServer(IntPtr identityTarget, int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public uint GetHostedDedicatedServerPort()
        {
            return 0;
        }

        public uint GetHostedDedicatedServerPOPID()
        {
            return 0;
        }

        public EResult GetHostedDedicatedServerAddress(SteamDatagramHostedAddress pRouting)
        {
            return default;
        }

        public uint CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public EResult GetGameCoordinatorServerLogin(SteamDatagramGameCoordinatorServerLogin pLoginInfo, int pcbSignedBlob, IntPtr pBlob)
        {
            return default;
        }

        public uint ConnectP2PCustomSignaling(IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions)
        {
            return 0;
        }

        public bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext)
        {
            return false;
        }

        public bool GetCertificateRequest(int pcbBlob, IntPtr pBlob, string errMsg)
        {
            return false;
        }

        public bool SetCertificate(IntPtr pCertificate, int cbCertificate, string errMsg)
        {
            return false;
        }

        public void RunCallbacks(IntPtr pCallbacks)
        {
            //
        }

    }

}