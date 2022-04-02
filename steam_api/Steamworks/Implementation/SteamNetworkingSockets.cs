using SKYNET;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Runtime.InteropServices;

public class SteamNetworkingSockets : SteamInterface
{
    public uint CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions)
    {
        Write("CreateListenSocketIP");
        return 0;
    }

    public uint ConnectByIPAddress(IntPtr address, int nOptions, IntPtr pOptions)
    {
        Write("ConnectByIPAddress");
        return 0;
    }

    public uint CreateListenSocketP2P(int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("CreateListenSocketP2P");
        return 0;
    }

    public uint ConnectP2P(IntPtr identityRemote, int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("ConnectP2P");
        return 0;
    }

    public EResult AcceptConnection(uint hConn)
    {
        Write("AcceptConnection");
        return default;
    }

    public bool CloseConnection(uint hPeer, int nReason, char pszDebug, bool bEnableLinger)
    {
        Write("CloseConnection");
        return false;
    }

    public bool CloseListenSocket(uint hSocket)
    {
        Write("CloseListenSocket");
        return false;
    }

    public bool SetConnectionUserData(uint hPeer, uint nUserData)
    {
        Write("SetConnectionUserData");
        return false;
    }

    public uint GetConnectionUserData(uint hPeer)
    {
        Write("GetConnectionUserData");
        return 0;
    }

    public void SetConnectionName(uint hPeer, char pszName)
    {
        Write("SetConnectionName");
    }

    public bool GetConnectionName(uint hPeer, char pszName, int nMaxLen)
    {
        Write("GetConnectionName");
        return false;
    }

    public EResult SendMessageToConnection(uint hConn, IntPtr pData, uint cbData, int nSendFlags, uint pOutMessageNumber)
    {
        Write("SendMessageToConnection");
        return default;
    }

    public void SendMessages(int nMessages, IntPtr pMessages, uint pOutMessageNumberOrResult)
    {
        Write("SendMessages");
    }

    public EResult FlushMessagesOnConnection(uint hConn)
    {
        Write("v");
        return default;
    }

    public int ReceiveMessagesOnConnection(uint hConn, IntPtr ppOutMessages, int nMaxMessages)
    {
        Write("ReceiveMessagesOnConnection");
        return 0;
    }

    public bool GetConnectionInfo(uint hConn, IntPtr pInfo)
    {
        Write("GetConnectionInfo");
        return false;
    }

    public bool GetQuickConnectionStatus(uint hConn, IntPtr pStats)
    {
        Write("GetQuickConnectionStatus");
        return false;
    }

    public int GetDetailedConnectionStatus(uint hConn, char pszBuf, int cbBuf)
    {
        Write("GetDetailedConnectionStatus");
        return 0;
    }

    public bool GetListenSocketAddress(uint hSocket, IntPtr address)
    {
        Write("GetListenSocketAddress");
        return false;
    }

    public bool CreateSocketPair(uint pOutConnection1, uint pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
    {
        Write("CreateSocketPair");
        return false;
    }

    public bool GetIdentity(IntPtr pIdentity)
    {
        Write("GetIdentity");
        return false;
    }

    public ESteamNetworkingAvailability InitAuthentication(IntPtr _)
    {
        Write("InitAuthentication");
        return ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;
    }

    public ESteamNetworkingAvailability GetAuthenticationStatus(IntPtr pDetails)
    {
        Write("GetAuthenticationStatus");
        return ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;
    }

    public uint CreatePollGroup(IntPtr _)
    {
        Write("CreatePollGroup");
        return 0;
    }

    public bool DestroyPollGroup(uint hPollGroup)
    {
        Write("DestroyPollGroup");
        return false;
    }

    public bool SetConnectionPollGroup(uint hConn, uint hPollGroup)
    {
        Write("SetConnectionPollGroup");
        return false;
    }

    public int ReceiveMessagesOnPollGroup(uint hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
    {
        Write("ReceiveMessagesOnPollGroup");
        return 0;
    }

    public bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, SteamDatagramRelayAuthTicket pOutParsedTicket)
    {
        Write("ReceivedRelayAuthTicket");
        return false;
    }

    public int FindRelayAuthTicketForServer(IntPtr identityGameServer, int nVirtualPort, SteamDatagramRelayAuthTicket pOutParsedTicket)
    {
        Write("FindRelayAuthTicketForServer");
        return 0;
    }

    public uint ConnectToHostedDedicatedServer(IntPtr identityTarget, int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("ConnectToHostedDedicatedServer");
        return 0;
    }

    public uint GetHostedDedicatedServerPort(IntPtr _)
    {
        Write("GetHostedDedicatedServerPort");
        return 0;
    }

    public uint GetHostedDedicatedServerPOPID(IntPtr _)
    {
        Write("GetHostedDedicatedServerPOPID");
        return 0;
    }

    public EResult GetHostedDedicatedServerAddress(SteamDatagramHostedAddress pRouting)
    {
        Write("GetHostedDedicatedServerAddress");
        return  EResult.k_EResultNone;
    }

    public uint CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("CreateHostedDedicatedServerListenSocket");
        return 0;
    }

    public EResult GetGameCoordinatorServerLogin(SteamDatagramGameCoordinatorServerLogin pLoginInfo, int pcbSignedBlob, IntPtr pBlob)
    {
        Write("GetGameCoordinatorServerLogin");
        return default;
    }

    public uint ConnectP2PCustomSignaling(IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions)
    {
        Write("ConnectP2PCustomSignaling");
        return 0;
    }

    public bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext)
    {
        Write("ReceivedP2PCustomSignal");
        return false;
    }

    public bool GetCertificateRequest(int pcbBlob, IntPtr pBlob, string errMsg)
    {
        Write("GetCertificateRequest");
        return false;
    }

    public bool SetCertificate(IntPtr pCertificate, int cbCertificate, string errMsg)
    {
        Write("SetCertificate");
        return false;
    }

    public void RunCallbacks(IntPtr pCallbacks)
    {
        Write("RunCallbacks");
    }

    public bool SteamDatagramClient_Init(bool bNoSteamSupport, IntPtr errMsg )
    {
        Write("SteamDatagramClient_Init");
        return true;
    }

    public bool SteamDatagramServer_Init(bool bNoSteamSupport, IntPtr errMsg )
    {
        Write("SteamDatagramServer_Init");
        return true;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}