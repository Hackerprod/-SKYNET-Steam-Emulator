using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamNetworking : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
    {
        Write("SteamAPI_ISteamNetworking_SendP2PPacket");
        return SteamClient.SteamNetworking.SendP2PPacket(steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_IsP2PPacketAvailable(uint pcubMsgSize, int nChannel)
    {
        Write("SteamAPI_ISteamNetworking_IsP2PPacketAvailable");
        return SteamClient.SteamNetworking.IsP2PPacketAvailable(pcubMsgSize, nChannel);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel)
    {
        Write("SteamAPI_ISteamNetworking_ReadP2PPacket");
        return SteamClient.SteamNetworking.ReadP2PPacket(pubDest, cubDest, pcubMsgSize, psteamIDRemote, nChannel);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser(IntPtr steamIDRemote)
    {
        Write("SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser");
        return SteamClient.SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_CloseP2PSessionWithUser(IntPtr steamIDRemote)
    {
        Write("SteamAPI_ISteamNetworking_CloseP2PSessionWithUser");
        return SteamClient.SteamNetworking.CloseP2PSessionWithUser(steamIDRemote);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel)
    {
        Write("SteamAPI_ISteamNetworking_CloseP2PChannelWithUser");
        return SteamClient.SteamNetworking.CloseP2PChannelWithUser(steamIDRemote, nChannel);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState)
    {
        Write("SteamAPI_ISteamNetworking_GetP2PSessionState");
        return SteamClient.SteamNetworking.GetP2PSessionState(steamIDRemote, pConnectionState);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_AllowP2PPacketRelay(bool bAllow)
    {
        Write("SteamAPI_ISteamNetworking_AllowP2PPacketRelay");
        return SteamClient.SteamNetworking.AllowP2PPacketRelay(bAllow);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworking_CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay)
    {
        Write("SteamAPI_ISteamNetworking_CreateListenSocket");
        return SteamClient.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworking_CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
    {
        Write("SteamAPI_ISteamNetworking_CreateP2PConnectionSocket");
        return SteamClient.SteamNetworking.CreateP2PConnectionSocket(steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworking_CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec)
    {
        Write("SteamAPI_ISteamNetworking_CreateConnectionSocket");
        return SteamClient.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
    {
        Write("SteamAPI_ISteamNetworking_DestroySocket");
        return SteamClient.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
    {
        Write("SteamAPI_ISteamNetworking_DestroyListenSocket");
        return SteamClient.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
    {
        Write("SteamAPI_ISteamNetworking_SendDataOnSocket");
        return SteamClient.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize)
    {
        Write("SteamAPI_ISteamNetworking_IsDataAvailableOnSocket");
        return SteamClient.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
    {
        Write("SteamAPI_ISteamNetworking_RetrieveDataFromSocket");
        return SteamClient.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket)
    {
        Write("SteamAPI_ISteamNetworking_IsDataAvailable");
        return SteamClient.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
    {
        Write("SteamAPI_ISteamNetworking_RetrieveData");
        return SteamClient.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote)
    {
        Write("SteamAPI_ISteamNetworking_GetSocketInfo");
        return SteamClient.SteamNetworking.GetSocketInfo(hSocket, pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworking_GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort)
    {
        Write("SteamAPI_ISteamNetworking_GetListenSocketInfo");
        return SteamClient.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESNetSocketConnectionType SteamAPI_ISteamNetworking_GetSocketConnectionType(uint hSocket)
    {
        Write("SteamAPI_ISteamNetworking_GetSocketConnectionType");
        return SteamClient.SteamNetworking.GetSocketConnectionType(hSocket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworking_GetMaxPacketSize(uint hSocket)
    {
        Write("SteamAPI_ISteamNetworking_GetMaxPacketSize");
        return SteamClient.SteamNetworking.GetMaxPacketSize(hSocket);
    }

}
