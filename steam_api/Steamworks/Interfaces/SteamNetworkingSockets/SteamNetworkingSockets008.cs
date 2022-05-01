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
        public HSteamListenSocket CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateListenSocketIP(localAddress, nOptions, pOptions);
        }

        public HSteamNetConnection ConnectByIPAddress(  IntPtr address, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectByIPAddress(address, nOptions, pOptions);
        }

        public HSteamListenSocket CreateListenSocketP2P(int npublicPort, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateListenSocketP2P(npublicPort, nOptions, pOptions);
        }

        public HSteamNetConnection ConnectP2P(  IntPtr identityRemote, int npublicPort, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2P(identityRemote, npublicPort, nOptions, pOptions);
        }

        public int AcceptConnection(HSteamNetConnection hConn)
        {
            return SteamEmulator.SteamNetworkingSockets.AcceptConnection(hConn);
        }

        public bool CloseConnection(HSteamNetConnection hPeer, int nReason,  char pszDebug, bool bEnableLinger)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
        }

        public bool CloseListenSocket(HSteamListenSocket hSocket)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseListenSocket(hSocket);
        }

        public bool SetConnectionUserData(HSteamNetConnection hPeer, ulong nUserData)
        {
            return SteamEmulator.SteamNetworkingSockets.SetConnectionUserData(hPeer, nUserData);
        }

        public Int64 GetConnectionUserData(HSteamNetConnection hPeer)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionUserData(hPeer);
        }

        public void SetConnectionName(HSteamNetConnection hPeer,  char pszName)
        {
            SteamEmulator.SteamNetworkingSockets.SetConnectionName(hPeer, pszName);
        }

        public bool GetConnectionName(HSteamNetConnection hPeer, char pszName, int nMaxLen)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionName(hPeer, pszName, nMaxLen);
        }

        public int SendMessageToConnection(HSteamNetConnection hConn, IntPtr pData, UInt32 cbData, int nSendFlags, Int64 pOutMessageNumber)
        {
            return SteamEmulator.SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, pOutMessageNumber);
        }

        public void SendMessages(int nMessages, IntPtr  pMessages, Int64 pOutMessageNumberOrResult)
        {
            SteamEmulator.SteamNetworkingSockets.SendMessages(nMessages, pMessages, pOutMessageNumberOrResult);
        }

        public int FlushMessagesOnConnection(HSteamNetConnection hConn)
        {
            return SteamEmulator.SteamNetworkingSockets.FlushMessagesOnConnection(hConn);
        }
        public int ReceiveMessagesOnConnection(HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceiveMessagesOnConnection(hConn, ppOutMessages, nMaxMessages);
        }

        public bool GetConnectionInfo(HSteamNetConnection hConn, IntPtr pInfo)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionInfo(hConn, pInfo);
        }

        public bool GetQuickConnectionStatus(HSteamNetConnection hConn, IntPtr pStats)
        {
            return SteamEmulator.SteamNetworkingSockets.GetQuickConnectionStatus(hConn, pStats);
        }

        public int GetDetailedConnectionStatus(HSteamNetConnection hConn, char pszBuf, int cbBuf)
        {
            return SteamEmulator.SteamNetworkingSockets.GetDetailedConnectionStatus(hConn, pszBuf, cbBuf);
        }

        public bool GetListenSocketAddress(HSteamListenSocket hSocket, IntPtr address)
        {
            return SteamEmulator.SteamNetworkingSockets.GetListenSocketAddress(hSocket, address);
        }

        public bool CreateSocketPair(HSteamNetConnection pOutConnection1, HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback,  IntPtr pIdentity1,  IntPtr pIdentity2)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateSocketPair(pOutConnection1, pOutConnection2, bUseNetworkLoopback, pIdentity1, pIdentity2);
        }

        public bool GetIdentity(IntPtr pIdentity)
        {
            return SteamEmulator.SteamNetworkingSockets.GetIdentity(pIdentity);
        }

        public int InitAuthentication()
        {
            return SteamEmulator.SteamNetworkingSockets.InitAuthentication();
        }

        public int GetAuthenticationStatus(IntPtr pDetails)
        {
            return SteamEmulator.SteamNetworkingSockets.GetAuthenticationStatus(pDetails);
        }

        public HSteamNetPollGroup CreatePollGroup()
        {
            return SteamEmulator.SteamNetworkingSockets.CreatePollGroup();
        }

        public bool DestroyPollGroup(HSteamNetPollGroup hPollGroup)
        {
            return SteamEmulator.SteamNetworkingSockets.DestroyPollGroup(hPollGroup);
        }

        public bool SetConnectionPollGroup(HSteamNetConnection hConn, HSteamNetPollGroup hPollGroup)
        {
            return SteamEmulator.SteamNetworkingSockets.SetConnectionPollGroup(hConn, hPollGroup);
        }

        public int ReceiveMessagesOnPollGroup(HSteamNetPollGroup hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceiveMessagesOnPollGroup(hPollGroup, ppOutMessages, nMaxMessages);
        }

        public bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, IntPtr pOutParsedTicket)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceivedRelayAuthTicket(pvTicket, cbTicket, pOutParsedTicket);
        }

        public int FindRelayAuthTicketForServer(  IntPtr identityGameServer, int npublicPort, IntPtr pOutParsedTicket)
        {
            return SteamEmulator.SteamNetworkingSockets.FindRelayAuthTicketForServer(identityGameServer, npublicPort, pOutParsedTicket);
        }


        public HSteamNetConnection ConnectToHostedDedicatedServer(  IntPtr identityTarget, int npublicPort, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectToHostedDedicatedServer(identityTarget, npublicPort, nOptions, pOptions);
        }


        public int GetHostedDedicatedServerPort()
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerPort();
        }


        public SteamNetworkingPOPID GetHostedDedicatedServerPOPID()
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerPOPID();
        }

        public int GetHostedDedicatedServerAddress(IntPtr pRouting)
        {
            return SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerAddress(pRouting);
        }

        public HSteamListenSocket CreateHostedDedicatedServerListenSocket(int npublicPort, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateHostedDedicatedServerListenSocket(npublicPort, nOptions, pOptions);
        }

        public int GetGameCoordinatorServerLogin(IntPtr pLoginInfo, int pcbSignedBlob, IntPtr pBlob)
        {
            return SteamEmulator.SteamNetworkingSockets.GetGameCoordinatorServerLogin(pLoginInfo, pcbSignedBlob, pBlob);
        }

        public HSteamNetConnection ConnectP2PCustomSignaling(IntPtr pSignaling,  IntPtr pPeerIdentity, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2PCustomSignaling(pSignaling, pPeerIdentity, nOptions, pOptions);
        }

        public bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceivedP2PCustomSignal(pMsg, cbMsg, pContext);
        }

        public bool GetCertificateRequest(int pcbBlob, IntPtr pBlob, string errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.GetCertificateRequest(pcbBlob, pBlob, errMsg);
        }

        public bool SetCertificate(  IntPtr pCertificate, int cbCertificate, string errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.SetCertificate(pCertificate, cbCertificate, errMsg);
        }
    }
}
