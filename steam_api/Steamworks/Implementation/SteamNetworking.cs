using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    [Map("SteamNetworking")]
    public class SteamNetworking : IBaseInterface, ISteamNetworking
    {
        public bool SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
        {
            return false;
        }

        public bool IsP2PPacketAvailable(uint pcubMsgSize, int nChannel)
        {
            return false;
        }

        public bool ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel)
        {
            return false;
        }

        public bool AcceptP2PSessionWithUser(IntPtr steamIDRemote)
        {
            return false;
        }

        public bool CloseP2PSessionWithUser(IntPtr steamIDRemote)
        {
            return false;
        }

        public bool CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel)
        {
            return false;
        }

        public bool GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState)
        {
            return false;
        }

        public bool AllowP2PPacketRelay(bool bAllow)
        {
            return false;
        }

        public uint CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            return 0;
        }

        public uint CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            return 0;
        }

        public uint CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec)
        {
            return 0;
        }

        public bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            return false;
        }

        public bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
        {
            return false;
        }

        public bool SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            return false;
        }

        public bool IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize)
        {
            return false;
        }

        public bool RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return false;
        }

        public bool IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket)
        {
            return false;
        }

        public bool RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
        {
            return false;
        }

        public bool GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote)
        {
            return false;
        }

        public bool GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort)
        {
            return false;
        }

        public ESNetSocketConnectionType GetSocketConnectionType(uint hSocket)
        {
            return default;
        }

        public int GetMaxPacketSize(uint hSocket)
        {
            return 0;
        }

    }


}