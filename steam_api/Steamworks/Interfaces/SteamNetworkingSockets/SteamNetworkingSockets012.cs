using SKYNET.Helpers;
using System;
using System.Collections.Generic;

using SteamNetworkingPOPID = System.UInt32;
using HSteamNetConnection = System.UInt32;
using HSteamListenSocket = System.UInt32;
using HSteamNetPollGroup = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamNetworkingSockets001",
        "CreateListenSocket", "ConnectBySteamID", "ConnectByIPv4Address", "AcceptConnection",
        "CloseConnection", "CloseListenSocket__V001", "SetConnectionUserData", "GetConnectionUserData",
        "SetConnectionName", "GetConnectionName", "SendMessageToConnection__V001", "FlushMessagesOnConnection",
        "ReceiveMessagesOnConnection__V001", "ReceiveMessagesOnListenSocket", "GetConnectionInfo__V001", "GetQuickConnectionStatus",
        "GetDetailedConnectionStatus", "GetListenSocketInfo", "CreateSocketPair__V001", "ReceivedRelayAuthTicket",
        "FindRelayAuthTicketForServer__V001", "ConnectToHostedDedicatedServer__V001", "GetHostedDedicatedServerPort", "GetHostedDedicatedServerPOPID",
        "GetHostedDedicatedServerAddress001", "CreateHostedDedicatedServerListenSocket__V001", "GetConnectionDebugText", "GetConfigurationValue",
        "SetConfigurationValue", "GetConfigurationValueName", "GetConfigurationString", "SetConfigurationString",
        "GetConfigurationStringName", "GetConnectionConfigurationValue", "SetConnectionConfigurationValue", "RunCallbacks__V001")]
    [InterfaceLayout("SteamNetworkingSockets002",
        "CreateListenSocketIP__V002", "ConnectByIPAddress__V002", "CreateListenSocketP2P__V002", "ConnectP2P__V002",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection__V001",
        "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "ReceiveMessagesOnListenSocket", "GetConnectionInfo",
        "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "GetIdentity", "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer__V002",
        "GetHostedDedicatedServerPort", "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress001", "CreateHostedDedicatedServerListenSocket__V001")]
    [InterfaceLayout("SteamNetworkingSockets003",
        "CreateListenSocketIP__V002", "ConnectByIPAddress__V002", "CreateListenSocketP2P__V002", "ConnectP2P__V002",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection__V001",
        "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "ReceiveMessagesOnListenSocket", "GetConnectionInfo",
        "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "GetIdentity", "InitAuthentication", "GetAuthenticationStatus", "ReceivedRelayAuthTicket",
        "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer__V002", "GetHostedDedicatedServerPort", "GetHostedDedicatedServerPOPID",
        "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket__V001", "GetGameCoordinatorServerLogin")]
    [InterfaceLayout("SteamNetworkingSockets004",
        "CreateListenSocketIP__V002", "ConnectByIPAddress__V002", "CreateListenSocketP2P__V002", "ConnectP2P__V002",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection__V001",
        "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "ReceiveMessagesOnListenSocket", "GetConnectionInfo",
        "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "GetIdentity", "InitAuthentication", "GetAuthenticationStatus", "ReceivedRelayAuthTicket",
        "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer__V002", "GetHostedDedicatedServerPort", "GetHostedDedicatedServerPOPID",
        "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket__V001", "GetGameCoordinatorServerLogin")]
    [InterfaceLayout("SteamNetworkingSockets006",
        "CreateListenSocketIP", "ConnectByIPAddress", "CreateListenSocketP2P", "ConnectP2P",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection",
        "SendMessages", "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "ReceiveMessagesOnListenSocket",
        "GetConnectionInfo", "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress",
        "CreateSocketPair", "GetIdentity", "InitAuthentication", "GetAuthenticationStatus",
        "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer", "GetHostedDedicatedServerPort",
        "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket", "GetGameCoordinatorServerLogin",
        "ConnectP2PCustomSignaling__V008", "ReceivedP2PCustomSignal")]
    [InterfaceLayout("SteamNetworkingSockets008",
        "CreateListenSocketIP", "ConnectByIPAddress", "CreateListenSocketP2P", "ConnectP2P",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection",
        "SendMessages", "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "GetConnectionInfo",
        "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "GetIdentity", "InitAuthentication", "GetAuthenticationStatus",
        "CreatePollGroup", "DestroyPollGroup", "SetConnectionPollGroup", "ReceiveMessagesOnPollGroup",
        "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer", "GetHostedDedicatedServerPort",
        "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket", "GetGameCoordinatorServerLogin",
        "ConnectP2PCustomSignaling__V008", "ReceivedP2PCustomSignal", "GetCertificateRequest", "SetCertificate")]
    [InterfaceLayout("SteamNetworkingSockets009",
        "CreateListenSocketIP", "ConnectByIPAddress", "CreateListenSocketP2P", "ConnectP2P",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection",
        "SendMessages", "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "GetConnectionInfo",
        "GetQuickConnectionStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "GetIdentity", "InitAuthentication", "GetAuthenticationStatus",
        "CreatePollGroup", "DestroyPollGroup", "SetConnectionPollGroup", "ReceiveMessagesOnPollGroup",
        "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer", "GetHostedDedicatedServerPort",
        "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket", "GetGameCoordinatorServerLogin",
        "ConnectP2PCustomSignaling", "ReceivedP2PCustomSignal", "GetCertificateRequest", "SetCertificate",
        "RunCallbacks")]
    [InterfaceLayout("SteamNetworkingSockets012",
        "CreateListenSocketIP", "ConnectByIPAddress", "CreateListenSocketP2P", "ConnectP2P",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection",
        "SendMessages", "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "GetConnectionInfo",
        "GetConnectionRealTimeStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "ConfigureConnectionLanes", "GetIdentity", "InitAuthentication", "GetAuthenticationStatus",
        "CreatePollGroup", "DestroyPollGroup", "SetConnectionPollGroup", "ReceiveMessagesOnPollGroup",
        "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer", "GetHostedDedicatedServerPort",
        "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket", "GetGameCoordinatorServerLogin",
        "ConnectP2PCustomSignaling", "ReceivedP2PCustomSignal", "GetCertificateRequest", "SetCertificate",
        "ResetIdentity", "RunCallbacks", "BeginAsyncRequestFakeIP", "GetFakeIP",
        "CreateListenSocketP2PFakeIP", "GetRemoteFakeIPForConnection", "CreateFakeUDPPort")]
    [InterfaceLayout("SteamNetworkingSockets013",
        "CreateListenSocketIP", "ConnectByIPAddress", "CreateListenSocketP2P", "ConnectP2P",
        "AcceptConnection", "CloseConnection", "CloseListenSocket", "SetConnectionUserData",
        "GetConnectionUserData", "SetConnectionName", "GetConnectionName", "SendMessageToConnection",
        "SendMessages__V013", "FlushMessagesOnConnection", "ReceiveMessagesOnConnection", "GetConnectionInfo",
        "GetConnectionRealTimeStatus", "GetDetailedConnectionStatus", "GetListenSocketAddress", "CreateSocketPair",
        "ConfigureConnectionLanes", "GetIdentity", "InitAuthentication", "GetAuthenticationStatus",
        "CreatePollGroup", "DestroyPollGroup", "SetConnectionPollGroup", "ReceiveMessagesOnPollGroup",
        "ReceivedRelayAuthTicket", "FindRelayAuthTicketForServer", "ConnectToHostedDedicatedServer", "GetHostedDedicatedServerPort",
        "GetHostedDedicatedServerPOPID", "GetHostedDedicatedServerAddress", "CreateHostedDedicatedServerListenSocket", "GetGameCoordinatorServerLogin",
        "ConnectP2PCustomSignaling", "ReceivedP2PCustomSignal", "GetCertificateRequest", "SetCertificate",
        "ResetIdentity", "RunCallbacks", "BeginAsyncRequestFakeIP", "GetFakeIP",
        "CreateListenSocketP2PFakeIP", "GetRemoteFakeIPForConnection", "CreateFakeUDPPort")]
    public class SteamNetworkingSockets012 : ISteamInterface
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

        public bool CloseConnection(IntPtr _, HSteamNetConnection hPeer, int nReason, string pszDebug, bool bEnableLinger)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
        }

        public bool CloseListenSocket(IntPtr _, HSteamListenSocket hSocket)
        {
            return SteamEmulator.SteamNetworkingSockets.CloseListenSocket(hSocket);
        }

        public bool SetConnectionUserData(IntPtr _, HSteamNetConnection hPeer, long nUserData)
        {
            return SteamEmulator.SteamNetworkingSockets.SetConnectionUserData(hPeer, nUserData);
        }

        public Int64 GetConnectionUserData(IntPtr _, HSteamNetConnection hPeer)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionUserData(hPeer);
        }

        public void SetConnectionName(IntPtr _, HSteamNetConnection hPeer, string pszName)
        {
            SteamEmulator.SteamNetworkingSockets.SetConnectionName(hPeer, pszName);
        }

        public bool GetConnectionName(IntPtr _, HSteamNetConnection hPeer, IntPtr pszName, int nMaxLen)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionName(hPeer, pszName, nMaxLen);
        }

        public int SendMessageToConnection(IntPtr _, HSteamNetConnection hConn, IntPtr pData, UInt32 cbData, int nSendFlags, IntPtr pOutMessageNumber)
        {
            return SteamEmulator.SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, pOutMessageNumber);
        }

        public void SendMessages(IntPtr _, int nMessages, IntPtr pMessages, IntPtr pOutMessageNumberOrResult)
        {
            SteamEmulator.SteamNetworkingSockets.SendMessages(nMessages, pMessages, pOutMessageNumberOrResult);
        }

        // The core always releases every message after sending, which is the bDeleteFailedMessages=true behaviour.
        public void SendMessages__V013(IntPtr _, int nMessages, IntPtr pMessages, IntPtr pOutMessageNumberOrResult, bool bDeleteFailedMessages)
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

        public int GetConnectionRealTimeStatus(IntPtr _, HSteamNetConnection hConn, IntPtr pStatus, int nLanes, IntPtr pLanes)
        {
            return SteamEmulator.SteamNetworkingSockets.GetConnectionRealTimeStatus(hConn, pStatus, nLanes, pLanes);
        }

        public int GetDetailedConnectionStatus(IntPtr _, HSteamNetConnection hConn, IntPtr pszBuf, int cbBuf)
        {
            return SteamEmulator.SteamNetworkingSockets.GetDetailedConnectionStatus(hConn, pszBuf, cbBuf);
        }

        public bool GetListenSocketAddress(IntPtr _, HSteamListenSocket hSocket, IntPtr address)
        {
            return SteamEmulator.SteamNetworkingSockets.GetListenSocketAddress(hSocket, address);
        }

        public bool CreateSocketPair(IntPtr _, IntPtr pOutConnection1, IntPtr pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateSocketPair(pOutConnection1, pOutConnection2, bUseNetworkLoopback, pIdentity1, pIdentity2);
        }

        public int ConfigureConnectionLanes(IntPtr _, HSteamNetConnection hConn, int nNumLanes, IntPtr pLanePriorities, IntPtr pLaneWeights)
        {
            return SteamEmulator.SteamNetworkingSockets.ConfigureConnectionLanes(hConn, nNumLanes, pLanePriorities, pLaneWeights);
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

        public ushort GetHostedDedicatedServerPort(IntPtr _)
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

        public int GetGameCoordinatorServerLogin(IntPtr _, IntPtr pLoginInfo, IntPtr pcbSignedBlob, IntPtr pBlob)
        {
            return SteamEmulator.SteamNetworkingSockets.GetGameCoordinatorServerLogin(pLoginInfo, pcbSignedBlob, pBlob);
        }

        private static bool _quickStatusLogged;

        public bool GetQuickConnectionStatus(IntPtr _, HSteamNetConnection hConn, IntPtr pStats)
        {
            if (!_quickStatusLogged)
            {
                _quickStatusLogged = true;
                SteamEmulator.Write("SteamNetworkingSockets", "GetQuickConnectionStatus not implemented");
            }
            return false;
        }

        public HSteamNetConnection ConnectP2PCustomSignaling__V008(IntPtr _, IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2PCustomSignaling(pSignaling, pPeerIdentity, 0, nOptions, pOptions);
        }

        public HSteamNetConnection ConnectP2PCustomSignaling(IntPtr _, IntPtr pSignaling, IntPtr pPeerIdentity, int nRemoteVirtualPort, int nOptions, IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.ConnectP2PCustomSignaling(pSignaling, pPeerIdentity, nRemoteVirtualPort, nOptions, pOptions);
        }

        public bool ReceivedP2PCustomSignal(IntPtr _, IntPtr pMsg, int cbMsg, IntPtr pContext)
        {
            return SteamEmulator.SteamNetworkingSockets.ReceivedP2PCustomSignal(pMsg, cbMsg, pContext);
        }

        public bool GetCertificateRequest(IntPtr _, IntPtr pcbBlob, IntPtr pBlob, IntPtr errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.GetCertificateRequest(pcbBlob, pBlob, errMsg);
        }

        public bool SetCertificate(IntPtr _, IntPtr pCertificate, int cbCertificate, IntPtr errMsg)
        {
            return SteamEmulator.SteamNetworkingSockets.SetCertificate(pCertificate, cbCertificate, errMsg);
        }

        public void ResetIdentity(IntPtr _, IntPtr pIdentity )
        {
            SteamEmulator.SteamNetworkingSockets.ResetIdentity(pIdentity);
        }

        public void RunCallbacks(IntPtr _)
        {
            SteamEmulator.SteamNetworkingSockets.RunCallbacks();
        }

        public bool BeginAsyncRequestFakeIP(IntPtr _, int nNumPorts)
        {
            return SteamEmulator.SteamNetworkingSockets.BeginAsyncRequestFakeIP(nNumPorts);
        }

        public void GetFakeIP(IntPtr _, int idxFirstPort, IntPtr pInfo)
        {
            SteamEmulator.SteamNetworkingSockets.GetFakeIP(idxFirstPort, pInfo);
        }

        public HSteamListenSocket CreateListenSocketP2PFakeIP(IntPtr _, int idxFakePort, int nOptions,  IntPtr pOptions)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateListenSocketP2PFakeIP(idxFakePort, nOptions, pOptions);
        }

        public int GetRemoteFakeIPForConnection(IntPtr _, HSteamNetConnection hConn, IntPtr pOutAddr)
        {
            return SteamEmulator.SteamNetworkingSockets.GetRemoteFakeIPForConnection(hConn, pOutAddr);
        }

        public IntPtr CreateFakeUDPPort(IntPtr _, int idxFakeServerPort)
        {
            return SteamEmulator.SteamNetworkingSockets.CreateFakeUDPPort(idxFakeServerPort);
        }

        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static void LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (!_stubsLogged.Add(method))
                {
                    return;
                }
            }

            SteamEmulator.Write("SteamNetworkingSockets", method + " not implemented");
        }

        // Pre-009 layouts (001-004): older GameNetworkingSockets signatures, adapted onto the same core calls.
        public HSteamListenSocket CreateListenSocketIP__V002(IntPtr _, IntPtr localAddress) => SteamEmulator.SteamNetworkingSockets.CreateListenSocketIP(localAddress, 0, IntPtr.Zero);
        public HSteamNetConnection ConnectByIPAddress__V002(IntPtr _, IntPtr address) => SteamEmulator.SteamNetworkingSockets.ConnectByIPAddress(address, 0, IntPtr.Zero);
        public HSteamListenSocket CreateListenSocketP2P__V002(IntPtr _, int nVirtualPort) => SteamEmulator.SteamNetworkingSockets.CreateListenSocketP2P(nVirtualPort, 0, IntPtr.Zero);
        public HSteamNetConnection ConnectP2P__V002(IntPtr _, IntPtr identityRemote, int nVirtualPort) => SteamEmulator.SteamNetworkingSockets.ConnectP2P(identityRemote, nVirtualPort, 0, IntPtr.Zero);
        public HSteamNetConnection ConnectToHostedDedicatedServer__V002(IntPtr _, IntPtr identityTarget, int nVirtualPort) => SteamEmulator.SteamNetworkingSockets.ConnectToHostedDedicatedServer(identityTarget, nVirtualPort, 0, IntPtr.Zero);
        public int SendMessageToConnection__V001(IntPtr _, HSteamNetConnection hConn, IntPtr pData, UInt32 cbData, int nSendFlags) => SteamEmulator.SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, IntPtr.Zero);
        public HSteamListenSocket CreateHostedDedicatedServerListenSocket__V001(IntPtr _, int nVirtualPort) => SteamEmulator.SteamNetworkingSockets.CreateHostedDedicatedServerListenSocket(nVirtualPort, 0, IntPtr.Zero);
        public bool GetHostedDedicatedServerAddress001(IntPtr _, IntPtr pRouting) => SteamEmulator.SteamNetworkingSockets.GetHostedDedicatedServerAddress(pRouting) == (int)EResult.k_EResultOK;

        public int ReceiveMessagesOnListenSocket(IntPtr _, HSteamListenSocket hSocket, IntPtr ppOutMessages, int nMaxMessages)
        {
            LogStub("ReceiveMessagesOnListenSocket");
            return 0;
        }

        // SteamNetworkingSockets001 (pre-release): message/connection-info structs differ from the current ones, so those slots are stubs.
        public HSteamListenSocket CreateListenSocket(IntPtr _, int nSteamConnectVirtualPort, uint nIP, ushort nPort)
        {
            LogStub("CreateListenSocket");
            return 0;
        }

        public HSteamNetConnection ConnectBySteamID(IntPtr _, ulong steamIDTarget, int nVirtualPort)
        {
            LogStub("ConnectBySteamID");
            return 0;
        }

        public HSteamNetConnection ConnectByIPv4Address(IntPtr _, uint nIP, ushort nPort)
        {
            LogStub("ConnectByIPv4Address");
            return 0;
        }

        public bool CloseListenSocket__V001(IntPtr _, HSteamListenSocket hSocket, string pszNotifyRemoteReason) => SteamEmulator.SteamNetworkingSockets.CloseListenSocket(hSocket);

        public int ReceiveMessagesOnConnection__V001(IntPtr _, HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages)
        {
            LogStub("ReceiveMessagesOnConnection__V001");
            return 0;
        }

        public bool GetConnectionInfo__V001(IntPtr _, HSteamNetConnection hConn, IntPtr pInfo)
        {
            LogStub("GetConnectionInfo__V001");
            return false;
        }

        public bool GetListenSocketInfo(IntPtr _, HSteamListenSocket hSocket, IntPtr pnIP, IntPtr pnPort)
        {
            LogStub("GetListenSocketInfo");
            return false;
        }

        public bool CreateSocketPair__V001(IntPtr _, IntPtr pOutConnection1, IntPtr pOutConnection2, bool bUseNetworkLoopback) => SteamEmulator.SteamNetworkingSockets.CreateSocketPair(pOutConnection1, pOutConnection2, bUseNetworkLoopback, IntPtr.Zero, IntPtr.Zero);
        public int FindRelayAuthTicketForServer__V001(IntPtr _, ulong steamID, int nVirtualPort, IntPtr pOutParsedTicket) => SteamEmulator.SteamNetworkingSockets.FindRelayAuthTicketForServer(IntPtr.Zero, nVirtualPort, pOutParsedTicket);

        public HSteamNetConnection ConnectToHostedDedicatedServer__V001(IntPtr _, ulong steamIDTarget, int nVirtualPort)
        {
            LogStub("ConnectToHostedDedicatedServer__V001");
            return 0;
        }

        public bool GetConnectionDebugText(IntPtr _, HSteamNetConnection hConn, IntPtr pOut, int nOutCCH)
        {
            LogStub("GetConnectionDebugText");
            return false;
        }

        public int GetConfigurationValue(IntPtr _, int eConfigValue)
        {
            LogStub("GetConfigurationValue");
            return 0;
        }

        public bool SetConfigurationValue(IntPtr _, int eConfigValue, int nValue)
        {
            LogStub("SetConfigurationValue");
            return false;
        }

        public IntPtr GetConfigurationValueName(IntPtr _, int eConfigValue)
        {
            LogStub("GetConfigurationValueName");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int GetConfigurationString(IntPtr _, int eConfigString, IntPtr pDest, int destSize)
        {
            LogStub("GetConfigurationString");
            return 0;
        }

        public bool SetConfigurationString(IntPtr _, int eConfigString, string pString)
        {
            LogStub("SetConfigurationString");
            return false;
        }

        public IntPtr GetConfigurationStringName(IntPtr _, int eConfigString)
        {
            LogStub("GetConfigurationStringName");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int GetConnectionConfigurationValue(IntPtr _, HSteamNetConnection hConn, int eConfigValue)
        {
            LogStub("GetConnectionConfigurationValue");
            return 0;
        }

        public bool SetConnectionConfigurationValue(IntPtr _, HSteamNetConnection hConn, int eConfigValue, int nValue)
        {
            LogStub("SetConnectionConfigurationValue");
            return false;
        }

        public void RunCallbacks__V001(IntPtr _, IntPtr pCallbacks) => SteamEmulator.SteamNetworkingSockets.RunCallbacks(pCallbacks);
    }
}
