using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamNetworking : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_SendP2PPacket");
            return SteamEmulator.SteamNetworking.SendP2PPacket(steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsP2PPacketAvailable(uint pcubMsgSize, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_IsP2PPacketAvailable");
            return SteamEmulator.SteamNetworking.IsP2PPacketAvailable(pcubMsgSize, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_ReadP2PPacket");
            return SteamEmulator.SteamNetworking.ReadP2PPacket(pubDest, cubDest, pcubMsgSize, psteamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser(IntPtr steamIDRemote)
        {
            Write("SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser");
            return SteamEmulator.SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_CloseP2PSessionWithUser(IntPtr steamIDRemote)
        {
            Write("SteamAPI_ISteamNetworking_CloseP2PSessionWithUser");
            return SteamEmulator.SteamNetworking.CloseP2PSessionWithUser(steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel)
        {
            Write("SteamAPI_ISteamNetworking_CloseP2PChannelWithUser");
            return SteamEmulator.SteamNetworking.CloseP2PChannelWithUser(steamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState)
        {
            Write("SteamAPI_ISteamNetworking_GetP2PSessionState");
            return SteamEmulator.SteamNetworking.GetP2PSessionState(steamIDRemote, pConnectionState);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_AllowP2PPacketRelay(bool bAllow)
        {
            Write("SteamAPI_ISteamNetworking_AllowP2PPacketRelay");
            return SteamEmulator.SteamNetworking.AllowP2PPacketRelay(bAllow);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            Write("SteamAPI_ISteamNetworking_CreateListenSocket");
            return SteamEmulator.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            Write("SteamAPI_ISteamNetworking_CreateP2PConnectionSocket");
            return SteamEmulator.SteamNetworking.CreateP2PConnectionSocket(steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamNetworking_CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec)
        {
            Write("SteamAPI_ISteamNetworking_CreateConnectionSocket");
            return SteamEmulator.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("SteamAPI_ISteamNetworking_DestroySocket");
            return SteamEmulator.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            Write("SteamAPI_ISteamNetworking_DestroyListenSocket");
            return SteamEmulator.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            Write("SteamAPI_ISteamNetworking_SendDataOnSocket");
            return SteamEmulator.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize)
        {
            Write("SteamAPI_ISteamNetworking_IsDataAvailableOnSocket");
            return SteamEmulator.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("SteamAPI_ISteamNetworking_RetrieveDataFromSocket");
            return SteamEmulator.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket)
        {
            Write("SteamAPI_ISteamNetworking_IsDataAvailable");
            return SteamEmulator.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
        {
            Write("SteamAPI_ISteamNetworking_RetrieveData");
            return SteamEmulator.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote)
        {
            Write("SteamAPI_ISteamNetworking_GetSocketInfo");
            return SteamEmulator.SteamNetworking.GetSocketInfo(hSocket, pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworking_GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort)
        {
            Write("SteamAPI_ISteamNetworking_GetListenSocketInfo");
            return SteamEmulator.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESNetSocketConnectionType SteamAPI_ISteamNetworking_GetSocketConnectionType(uint hSocket)
        {
            Write("SteamAPI_ISteamNetworking_GetSocketConnectionType");
            return SteamEmulator.SteamNetworking.GetSocketConnectionType(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworking_GetMaxPacketSize(uint hSocket)
        {
            Write("SteamAPI_ISteamNetworking_GetMaxPacketSize");
            return SteamEmulator.SteamNetworking.GetMaxPacketSize(hSocket);
        }
    }
}
