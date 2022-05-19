using System;


namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamNetworking006")]
    public class SteamNetworking006 : ISteamInterface
    {
        public bool SendP2PPacket(IntPtr _, ulong steamIDRemote, IntPtr pubData, uint cubData, int eP2PSendType, int nChannel = 0)
        {
            return SteamEmulator.SteamNetworking.SendP2PPacket(steamIDRemote, pubData, cubData, eP2PSendType, 0);
        }

        public bool IsP2PPacketAvailable(IntPtr _, ref uint pcubMsgSize, int nChannel = 0)
        {
            return SteamEmulator.SteamNetworking.IsP2PPacketAvailable(ref pcubMsgSize, 0);
        }

        public bool ReadP2PPacket(IntPtr _, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref ulong psteamIDRemote, int nChannel = 0)
        {
            return SteamEmulator.SteamNetworking.ReadP2PPacket(pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, 0);
        }

        public bool AcceptP2PSessionWithUser(IntPtr _, ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
        }

        public bool CloseP2PSessionWithUser(IntPtr _, ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.CloseP2PSessionWithUser(steamIDRemote);
        }

        public bool CloseP2PChannelWithUser(IntPtr _, ulong steamIDRemote, int nChannel)
        {
            return SteamEmulator.SteamNetworking.CloseP2PChannelWithUser(steamIDRemote, nChannel);
        }

        public bool GetP2PSessionState(IntPtr _, ulong steamIDRemote, IntPtr pConnectionState)
        {
            return SteamEmulator.SteamNetworking.GetP2PSessionState(steamIDRemote, pConnectionState);
        }

        public bool AllowP2PPacketRelay(IntPtr _, bool bAllow)
        {
            return SteamEmulator.SteamNetworking.AllowP2PPacketRelay(bAllow);
        }

        public uint CreateListenSocket(IntPtr _, int nVirtualP2PPort, uint nIP, uint nPort, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
        }

        public uint CreateP2PConnectionSocket(IntPtr _, ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateP2PConnectionSocket(steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
        }

        public uint CreateConnectionSocket(IntPtr _, uint nIP, uint nPort, int nTimeoutSec)
        {
            return SteamEmulator.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
        }

        public bool DestroySocket(IntPtr _, uint hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
        }

        public bool DestroyListenSocket(IntPtr _, uint hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
        }

        public bool SendDataOnSocket(IntPtr _, uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            return SteamEmulator.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
        }

        public bool IsDataAvailableOnSocket(IntPtr _, uint hSocket, uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
        }

        public bool RetrieveDataFromSocket(IntPtr _, uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
        }

        public bool IsDataAvailable(IntPtr _, uint hListenSocket, uint pcubMsgSize, uint phSocket)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
        }

        public bool RetrieveData(IntPtr _, uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
        {
            return SteamEmulator.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
        }

        public bool GetSocketInfo(IntPtr _, uint hSocket, ulong pSteamIDRemote, int peSocketStatus, uint punIPRemote, uint punPortRemote)
        {
            return SteamEmulator.SteamNetworking.GetSocketInfo(hSocket, pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
        }

        public bool GetListenSocketInfo(IntPtr _, uint hListenSocket, uint pnIP, uint pnPort)
        {
            return SteamEmulator.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
        }

        public int GetSocketConnectionType(IntPtr _, uint hSocket)
        {
            return SteamEmulator.SteamNetworking.GetSocketConnectionType(hSocket);
        }

        public int GetMaxPacketSize(IntPtr _, uint hSocket)
        {
            return SteamEmulator.SteamNetworking.GetMaxPacketSize(hSocket);
        }
    }
}
