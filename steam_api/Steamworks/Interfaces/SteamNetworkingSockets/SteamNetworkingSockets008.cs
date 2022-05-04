using System;

using SteamNetworkingPOPID = System.UInt32;
using HSteamNetConnection = System.UInt32;
using HSteamListenSocket = System.UInt32;
using HSteamNetPollGroup = System.UInt32;

namespace SKYNET.Interface
{
    [Interface("SteamNetworkingSockets008")]
    public class SteamNetworkingSockets008 : ISteamInterface
    {
        public HSteamListenSocket CreateListenSocketIP(IntPtr _, IntPtr localAddress, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateListenSocketIP(localAddress, nOptions, pOptions);
        }

        public HSteamNetConnection ConnectByIPAddress(IntPtr _, IntPtr address, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectByIPAddress(address, nOptions, pOptions);
        }

        public HSteamListenSocket CreateListenSocketP2P(IntPtr _, int npublicPort, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateListenSocketP2P(npublicPort, nOptions, pOptions);
        }

        public HSteamNetConnection ConnectP2P(IntPtr _, IntPtr identityRemote, int npublicPort, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2P(identityRemote, npublicPort, nOptions, pOptions);
        }

        public int AcceptConnection(IntPtr _, HSteamNetConnection hConn)
        {
            return SteamEmulator.SteamNetworkingSockets.AcceptConnection(hConn);
        }

        public bool CloseConnection(IntPtr _, HSteamNetConnection hPeer, int nReason, char pszDebug, bool bEnableLinger)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
        }

        public bool CloseListenSocket(IntPtr _, HSteamListenSocket hSocket)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseListenSocket(hSocket);
        }

        public bool SetConnectionUserData(IntPtr _, HSteamNetConnection hPeer, ulong nUserData)
        {
            return SteamEmulator.SteamNetworkingSockets.SetConnectionUserData(hPeer, nUserData);
        }

        public Int64 GetConnectionUserData(IntPtr _, HSteamNetConnection hPeer)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionUserData(hPeer);
        }

        public void SetConnectionName(IntPtr _, HSteamNetConnection hPeer, char pszName)
        {
            SteamEmulator.SteamNetworkingSockets.SetConnectionName(hPeer, pszName);
        }

        public bool GetConnectionName(IntPtr _, HSteamNetConnection hPeer, char pszName, int nMaxLen)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionName(hPeer, pszName, nMaxLen);
        }

        public int SendMessageToConnection(IntPtr _, HSteamNetConnection hConn, IntPtr pData, UInt32 cbData, int nSendFlags, Int64 pOutMessageNumber)
        {
            return SteamEmulator.SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, pOutMessageNumber);
        }

        public void SendMessages(IntPtr _, int nMessages, IntPtr pMessages, Int64 pOutMessageNumberOrResult)
        {
            SteamEmulator.SteamNetworkingSockets.SendMessages(nMessages, pMessages, pOutMessageNumberOrResult);
        }

        public int FlushMessagesOnConnection(IntPtr _, HSteamNetConnection hConn)
        {
            return SteamEmulator.SteamNetworkingSockets.FlushMessagesOnConnection(hConn);
        }
        public int ReceiveMessagesOnConnection(IntPtr _, HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceiveMessagesOnConnection(hConn, ppOutMessages, nMaxMessages);
        }

        public bool GetConnectionInfo(IntPtr _, HSteamNetConnection hConn, IntPtr pInfo)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionInfo(hConn, pInfo);
        }

        public bool GetQuickConnectionStatus(IntPtr _, HSteamNetConnection hConn, IntPtr pStats)
        {
            return SteamEmulator.SteamNetworkingSockets.GetQuickConnectionStatus(hConn, pStats);
        }

        public int GetDetailedConnectionStatus(IntPtr _, HSteamNetConnection hConn, char pszBuf, int cbBuf)
        {
            return SteamEmulator.SteamNetworkingSockets.GetDetailedConnectionStatus(hConn, pszBuf, cbBuf);
        }

        public bool GetListenSocketAddress(IntPtr _, HSteamListenSocket hSocket, IntPtr address)
        {
            return SteamEmulator.SteamNetworkingSockets.GetListenSocketAddress(hSocket, address);
        }

        public bool CreateSocketPair(IntPtr _, HSteamNetConnection pOutConnection1, HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateSocketPair(pOutConnection1, pOutConnection2, bUseNetworkLoopback, pIdentity1, pIdentity2);
        }

        public bool GetIdentity(IntPtr _, IntPtr pIdentity)
        {
            return SteamEmulator.SteamNetworkingSockets.GetIdentity(pIdentity);
        }

        public int InitAuthentication(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSockets.InitAuthentication();
        }

        public int GetAuthenticationStatus(IntPtr _, IntPtr pDetails)
        {
            return SteamEmulator.SteamNetworkingSockets.GetAuthenticationStatus(pDetails);
        }

        public HSteamNetPollGroup CreatePollGroup(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSockets.CreatePollGroup();
        }

        public bool DestroyPollGroup(IntPtr _, HSteamNetPollGroup hPollGroup)
        {
            return SteamEmulator.SteamNetworkingSockets.DestroyPollGroup(hPollGroup);
        }

        public bool SetConnectionPollGroup(IntPtr _, HSteamNetConnection hConn, HSteamNetPollGroup hPollGroup)
        {
            return SteamEmulator.SteamNetworkingSockets.SetConnectionPollGroup(hConn, hPollGroup);
        }

        public int ReceiveMessagesOnPollGroup(IntPtr _, HSteamNetPollGroup hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceiveMessagesOnPollGroup(hPollGroup, ppOutMessages, nMaxMessages);
        }

        public bool ReceivedRelayAuthTicket(IntPtr _, IntPtr pvTicket, int cbTicket, IntPtr pOutParsedTicket)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceivedRelayAuthTicket(pvTicket, cbTicket, pOutParsedTicket);
        }

        public int FindRelayAuthTicketForServer(IntPtr _, IntPtr identityGameServer, int npublicPort, IntPtr pOutParsedTicket)
        {
            return SteamEmulator.SteamNetworkingSockets.FindRelayAuthTicketForServer(identityGameServer, npublicPort, pOutParsedTicket);
        }

        public HSteamNetConnection ConnectToHostedDedicatedServer(IntPtr _, IntPtr identityTarget, int npublicPort, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectToHostedDedicatedServer(identityTarget, npublicPort, nOptions, pOptions);
        }

        public int GetHostedDedicatedServerPort(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerPort();
        }

        public SteamNetworkingPOPID GetHostedDedicatedServerPOPID(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerPOPID();
        }

        public int GetHostedDedicatedServerAddress(IntPtr _, IntPtr pRouting)
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerAddress(pRouting);
        }

        public HSteamListenSocket CreateHostedDedicatedServerListenSocket(IntPtr _, int npublicPort, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateHostedDedicatedServerListenSocket(npublicPort, nOptions, pOptions);
        }

        public int GetGameCoordinatorServerLogin(IntPtr _, IntPtr pLoginInfo, int pcbSignedBlob, IntPtr pBlob)
        {
            return SteamEmulator.SteamNetworkingSockets.GetGameCoordinatorServerLogin(pLoginInfo, pcbSignedBlob, pBlob);
        }

        public HSteamNetConnection ConnectP2PCustomSignaling(IntPtr _, IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2PCustomSignaling(pSignaling, pPeerIdentity, nOptions, pOptions);
        }

        public bool ReceivedP2PCustomSignal(IntPtr _, IntPtr pMsg, int cbMsg, IntPtr pContext)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceivedP2PCustomSignal(pMsg, cbMsg, pContext);
        }

        public bool GetCertificateRequest(IntPtr _, int pcbBlob, IntPtr pBlob, string errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.GetCertificateRequest(pcbBlob, pBlob, errMsg);
        }

        public bool SetCertificate(IntPtr _, IntPtr pCertificate, int cbCertificate, string errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.SetCertificate(pCertificate, cbCertificate, errMsg);
        }
    }
}
