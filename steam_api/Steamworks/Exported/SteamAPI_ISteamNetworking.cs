using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamNetworking
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_SendP2PPacket(IntPtr _, ulong steamIDRemote, IntPtr pubData, uint cubData, int eP2PSendType, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_SendP2PPacket");
            return SteamEmulator.SteamNetworking.SendP2PPacket(steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsP2PPacketAvailable(IntPtr _, ref uint pcubMsgSize, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_IsP2PPacketAvailable");
            return SteamEmulator.SteamNetworking.IsP2PPacketAvailable(ref pcubMsgSize, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_ReadP2PPacket(IntPtr _, IntPtr pubDest, ref uint cubDest, ref uint pcubMsgSize, ulong psteamIDRemote, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_ReadP2PPacket");
            return SteamEmulator.SteamNetworking.ReadP2PPacket(pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser(IntPtr _, ulong steamIDRemote)
        {
            Write("SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser");
            return SteamEmulator.SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_CloseP2PSessionWithUser(IntPtr _, ulong steamIDRemote)
        {
            Write("SteamAPI_ISteamNetworking_CloseP2PSessionWithUser");
            return SteamEmulator.SteamNetworking.CloseP2PSessionWithUser(steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_CloseP2PChannelWithUser(IntPtr _, ulong steamIDRemote, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_CloseP2PChannelWithUser");
            return SteamEmulator.SteamNetworking.CloseP2PChannelWithUser(steamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetP2PSessionState(IntPtr _, ulong steamIDRemote, IntPtr pConnectionState)
        {
            Write("SteamAPI_ISteamNetworking_GetP2PSessionState");
            return SteamEmulator.SteamNetworking.GetP2PSessionState(steamIDRemote, pConnectionState);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_AllowP2PPacketRelay(IntPtr _, bool bAllow)
        {
            Write("SteamAPI_ISteamNetworking_AllowP2PPacketRelay");
            return SteamEmulator.SteamNetworking.AllowP2PPacketRelay(bAllow);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateListenSocket(IntPtr _, int nVirtualP2PPort, uint nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            Write("SteamAPI_ISteamNetworking_CreateListenSocket");
            return SteamEmulator.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateP2PConnectionSocket(IntPtr _, ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            Write("SteamAPI_ISteamNetworking_CreateP2PConnectionSocket");
            return SteamEmulator.SteamNetworking.CreateP2PConnectionSocket(steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateConnectionSocket(IntPtr _, uint nIP, uint nPort, int nTimeoutSec)
        {
            Write("SteamAPI_ISteamNetworking_CreateConnectionSocket");
            return SteamEmulator.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_DestroySocket(IntPtr _, uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("SteamAPI_ISteamNetworking_DestroySocket");
            return SteamEmulator.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_DestroyListenSocket(IntPtr _, uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("SteamAPI_ISteamNetworking_DestroyListenSocket");
            return SteamEmulator.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_SendDataOnSocket(IntPtr _, uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            Write("SteamAPI_ISteamNetworking_SendDataOnSocket");
            return SteamEmulator.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsDataAvailableOnSocket(IntPtr _, uint hSocket, uint pcubMsgSize)
        {
            Write("SteamAPI_ISteamNetworking_IsDataAvailableOnSocket");
            return SteamEmulator.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_RetrieveDataFromSocket(IntPtr _, uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("SteamAPI_ISteamNetworking_RetrieveDataFromSocket");
            return SteamEmulator.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsDataAvailable(IntPtr _, uint hListenSocket, uint pcubMsgSize, uint phSocket)
        {
            Write("SteamAPI_ISteamNetworking_IsDataAvailable");
            return SteamEmulator.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_RetrieveData(IntPtr _, uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
        {
            Write("SteamAPI_ISteamNetworking_RetrieveData");
            return SteamEmulator.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetSocketInfo(IntPtr _, uint hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            Write("SteamAPI_ISteamNetworking_GetSocketInfo");
            return SteamEmulator.SteamNetworking.GetSocketInfo(hSocket, pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetListenSocketInfo(IntPtr _, uint hListenSocket, uint pnIP, uint pnPort)
        {
            Write("SteamAPI_ISteamNetworking_GetListenSocketInfo");
            return SteamEmulator.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworking_GetSocketConnectionType(IntPtr _, uint hSocket)
        {
            Write("SteamAPI_ISteamNetworking_GetSocketConnectionType");
            return SteamEmulator.SteamNetworking.GetSocketConnectionType(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworking_GetMaxPacketSize(IntPtr _, uint hSocket)
        {
            Write("SteamAPI_ISteamNetworking_GetMaxPacketSize");
            return SteamEmulator.SteamNetworking.GetMaxPacketSize(hSocket);
        }

        #region ISteamNetworkingFakeUDPPort

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingFakeUDPPort_DestroyFakeUDPPort(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingFakeUDPPort_DestroyFakeUDPPort");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EResult SteamAPI_ISteamNetworkingFakeUDPPort_SendMessageToFakeIP(IntPtr _, IntPtr remoteAddress, IntPtr pData, uint cbData, int nSendFlags)
        {
            Write("SteamAPI_ISteamNetworkingFakeUDPPort_SendMessageToFakeIP");
            return EResult.k_EResultFail;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingFakeUDPPort_ReceiveMessages(IntPtr _, IntPtr ppOutMessages, int nMaxMessages)
        {
            Write("SteamAPI_ISteamNetworkingFakeUDPPort_ReceiveMessages");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingFakeUDPPort_ScheduleCleanup(IntPtr _, IntPtr remoteAddress )
        {
            Write("SteamAPI_ISteamNetworkingFakeUDPPort_ScheduleCleanup");
        }

        #endregion

        #region SteamIPAddress_t

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamIPAddress_t_IsSet(IntPtr _)
        {
            Write("SteamAPI_SteamIPAddress_t_IsSet");
            return false;
        }

        #endregion

        #region servernetadr_t

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_servernetadr_t_Construct(IntPtr _)
        {
            Write("SteamAPI_MatchMakingKeyValuePair_t_Construct");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_servernetadr_t_Init(IntPtr _, int ip, int usQueryPort, int usConnectionPort)
        {
            Write("SteamAPI_servernetadr_t_Init");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_servernetadr_t_GetQueryPort(IntPtr _)
        {
            Write("SteamAPI_servernetadr_t_GetQueryPort");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_servernetadr_t_SetQueryPort(IntPtr _, int usPort)
        {
            Write("SteamAPI_servernetadr_t_SetQueryPort");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_servernetadr_t_GetConnectionPort(IntPtr _)
        {
            Write("SteamAPI_servernetadr_t_GetConnectionPort");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_servernetadr_t_SetConnectionPort(IntPtr _, int usPort)
        {
            Write("SteamAPI_MatchMakingKeyValuePair_t_Construct");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_servernetadr_t_GetIP(IntPtr _)
        {
            Write("SteamAPI_servernetadr_t_GetIP");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_servernetadr_t_SetIP(IntPtr _, uint unIP)
        {
            Write("SteamAPI_servernetadr_t_SetIP");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_servernetadr_t_GetConnectionAddressString(IntPtr _)
        {
            Write("SteamAPI_servernetadr_t_GetConnectionAddressString");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_servernetadr_t_GetQueryAddressString(IntPtr _)
        {
            Write("SteamAPI_servernetadr_t_GetQueryAddressString");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_servernetadr_t_IsLessThan(IntPtr _, IntPtr netadr)
        {
            Write("SteamAPI_servernetadr_t_IsLessThan");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_servernetadr_t_Assign(IntPtr _, IntPtr netadr)
        {
            Write("SteamAPI_servernetadr_t_Assign");
            return "";
        }

        #endregion

        #region gameserveritem_t

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_gameserveritem_t_Construct(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_Construct");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_gameserveritem_t_GetName(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_GetName");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_gameserveritem_t_SetName(IntPtr self, IntPtr pName)
        {
            Write($"SteamAPI_gameserveritem_t_SetName");
        }

        #endregion

        #region SteamNetworkingMessage_t

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingMessage_t_Release(IntPtr _)
        {
            Write("SteamAPI_SteamNetworkingMessage_t_Release");
        }

        #endregion

        #region SteamDatagramHostedAddress

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamDatagramHostedAddress_Clear(IntPtr _)
        {
            Write("SteamAPI_SteamDatagramHostedAddress_Clear");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamDatagramHostedAddress_GetPopID(IntPtr _)
        {
            Write("SteamAPI_SteamDatagramHostedAddress_GetPopID");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamDatagramHostedAddress_SetDevAddress(IntPtr _, uint nIP, int nPort, IntPtr popid)
        {
            Write("SteamAPI_SteamDatagramHostedAddress_SetDevAddress");
        }

        #endregion

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
