using Core.Interface;
using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamNetworking")]
    public class DSteamNetworking
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsP2PPacketAvailable(uint pcubMsgSize, int nChannel);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AcceptP2PSessionWithUser(IntPtr steamIDRemote);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseP2PSessionWithUser(IntPtr steamIDRemote);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AllowP2PPacketRelay(bool bAllow);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESNetSocketConnectionType GetSocketConnectionType(uint hSocket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetMaxPacketSize(uint hSocket);

    }
}
