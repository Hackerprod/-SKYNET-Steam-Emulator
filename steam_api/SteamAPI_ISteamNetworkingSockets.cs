using SKYNET.Interface;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamNetworkingSockets : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr localAddress, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP");
        return SteamClient.SteamNetworkingSockets.CreateListenSocketIP(localAddress, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr address, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress");
        return SteamClient.SteamNetworkingSockets.ConnectByIPAddress(address, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketP2P(int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CreateListenSocketP2P");
        return SteamClient.SteamNetworkingSockets.CreateListenSocketP2P(nVirtualPort, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_ConnectP2P(IntPtr identityRemote, int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ConnectP2P");
        return SteamClient.SteamNetworkingSockets.ConnectP2P(identityRemote, nVirtualPort, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EResult SteamAPI_ISteamNetworkingSockets_AcceptConnection(uint hConn)
    {
        Write("SteamAPI_ISteamNetworkingSockets_AcceptConnection");
        return SteamClient.SteamNetworkingSockets.AcceptConnection(hConn);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_CloseConnection(uint hPeer, int nReason, char pszDebug, bool bEnableLinger)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CloseConnection");
        return SteamClient.SteamNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(uint hSocket)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CloseListenSocket");
        return SteamClient.SteamNetworkingSockets.CloseListenSocket(hSocket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(uint hPeer, uint nUserData)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SetConnectionUserData");
        return SteamClient.SteamNetworkingSockets.SetConnectionUserData(hPeer, nUserData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(uint hPeer)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetConnectionUserData");
        return SteamClient.SteamNetworkingSockets.GetConnectionUserData(hPeer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSockets_SetConnectionName(uint hPeer, char pszName)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SetConnectionName");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(uint hPeer, char pszName, int nMaxLen)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetConnectionName");
        return SteamClient.SteamNetworkingSockets.GetConnectionName(hPeer, pszName, nMaxLen);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EResult SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(uint hConn, IntPtr pData, uint cbData, int nSendFlags, uint pOutMessageNumber)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SendMessageToConnection");
        return SteamClient.SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, pOutMessageNumber);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSockets_SendMessages(int nMessages, IntPtr pMessages, uint pOutMessageNumberOrResult)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SendMessages");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EResult SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(uint hConn)
    {
        Write("SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection");
        return SteamClient.SteamNetworkingSockets.FlushMessagesOnConnection(hConn);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(uint hConn, IntPtr ppOutMessages, int nMaxMessages)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection");
        return SteamClient.SteamNetworkingSockets.ReceiveMessagesOnConnection(hConn, ppOutMessages, nMaxMessages);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(uint hConn, IntPtr pInfo)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetConnectionInfo");
        return SteamClient.SteamNetworkingSockets.GetConnectionInfo(hConn, pInfo);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(uint hConn, IntPtr pStats)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus");
        return SteamClient.SteamNetworkingSockets.GetQuickConnectionStatus(hConn, pStats);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(uint hConn, char pszBuf, int cbBuf)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus");
        return SteamClient.SteamNetworkingSockets.GetDetailedConnectionStatus(hConn, pszBuf, cbBuf);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(uint hSocket, IntPtr address)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress");
        return SteamClient.SteamNetworkingSockets.GetListenSocketAddress(hSocket, address);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(uint pOutConnection1, uint pOutConnection2, bool bUseNetworkLoopback, IntPtr pIdentity1, IntPtr pIdentity2)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CreateSocketPair");
        return SteamClient.SteamNetworkingSockets.CreateSocketPair(pOutConnection1, pOutConnection2, bUseNetworkLoopback, pIdentity1, pIdentity2);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetIdentity(IntPtr pIdentity)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetIdentity");
        return SteamClient.SteamNetworkingSockets.GetIdentity(pIdentity);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamNetworkingAvailability SteamAPI_ISteamNetworkingSockets_InitAuthentication()
    {
        Write("SteamAPI_ISteamNetworkingSockets_InitAuthentication");
        return SteamClient.SteamNetworkingSockets.InitAuthentication();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamNetworkingAvailability SteamAPI_ISteamNetworkingSockets_GetAuthenticationStatus(IntPtr pDetails)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetAuthenticationStatus");
        return SteamClient.SteamNetworkingSockets.GetAuthenticationStatus(pDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_CreatePollGroup()
    {
        Write("SteamAPI_ISteamNetworkingSockets_CreatePollGroup");
        return SteamClient.SteamNetworkingSockets.CreatePollGroup();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(uint hPollGroup)
    {
        Write("SteamAPI_ISteamNetworkingSockets_DestroyPollGroup");
        return SteamClient.SteamNetworkingSockets.DestroyPollGroup(hPollGroup);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(uint hConn, uint hPollGroup)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup");
        return SteamClient.SteamNetworkingSockets.SetConnectionPollGroup(hConn, hPollGroup);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(uint hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup");
        return SteamClient.SteamNetworkingSockets.ReceiveMessagesOnPollGroup(hPollGroup, ppOutMessages, nMaxMessages);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, SteamDatagramRelayAuthTicket pOutParsedTicket)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ReceivedRelayAuthTicket");
        return SteamClient.SteamNetworkingSockets.ReceivedRelayAuthTicket(pvTicket, cbTicket, pOutParsedTicket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingSockets_FindRelayAuthTicketForServer(IntPtr identityGameServer, int nVirtualPort, SteamDatagramRelayAuthTicket pOutParsedTicket)
    {
        Write("SteamAPI_ISteamNetworkingSockets_FindRelayAuthTicketForServer");
        return SteamClient.SteamNetworkingSockets.FindRelayAuthTicketForServer(identityGameServer, nVirtualPort, pOutParsedTicket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_ConnectToHostedDedicatedServer(IntPtr identityTarget, int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ConnectToHostedDedicatedServer");
        return SteamClient.SteamNetworkingSockets.ConnectToHostedDedicatedServer(identityTarget, nVirtualPort, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort()
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort");
        return SteamClient.SteamNetworkingSockets.GetHostedDedicatedServerPort();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID()
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID");
        return SteamClient.SteamNetworkingSockets.GetHostedDedicatedServerPOPID();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EResult SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerAddress(SteamDatagramHostedAddress pRouting)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerAddress");
        return SteamClient.SteamNetworkingSockets.GetHostedDedicatedServerAddress(pRouting);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_CreateHostedDedicatedServerListenSocket");
        return SteamClient.SteamNetworkingSockets.CreateHostedDedicatedServerListenSocket(nVirtualPort, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EResult SteamAPI_ISteamNetworkingSockets_GetGameCoordinatorServerLogin(SteamDatagramGameCoordinatorServerLogin pLoginInfo, int pcbSignedBlob, IntPtr pBlob)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetGameCoordinatorServerLogin");
        return SteamClient.SteamNetworkingSockets.GetGameCoordinatorServerLogin(pLoginInfo, pcbSignedBlob, pBlob);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingSockets_ConnectP2PCustomSignaling(IntPtr pSignaling, IntPtr pPeerIdentity, int nOptions, IntPtr pOptions)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ConnectP2PCustomSignaling");
        return SteamClient.SteamNetworkingSockets.ConnectP2PCustomSignaling(pSignaling, pPeerIdentity, nOptions, pOptions);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext)
    {
        Write("SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal");
        return SteamClient.SteamNetworkingSockets.ReceivedP2PCustomSignal(pMsg, cbMsg, pContext);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_GetCertificateRequest(int pcbBlob, IntPtr pBlob, string errMsg)
    {
        Write("SteamAPI_ISteamNetworkingSockets_GetCertificateRequest");
        return SteamClient.SteamNetworkingSockets.GetCertificateRequest(pcbBlob, pBlob, errMsg);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingSockets_SetCertificate(IntPtr pCertificate, int cbCertificate, string errMsg)
    {
        Write("SteamAPI_ISteamNetworkingSockets_SetCertificate");
        return SteamClient.SteamNetworkingSockets.SetCertificate(pCertificate, cbCertificate, errMsg);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingSockets_RunCallbacks(IntPtr pCallbacks)
    {
        Write("SteamAPI_ISteamNetworkingSockets_RunCallbacks");
        //
    }

}

