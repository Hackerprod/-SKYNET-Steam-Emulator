using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;

public class SteamNetworking : SteamInterface
{
    public bool SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
    {
        Write("SendP2PPacket");
        return false;
    }

    public bool IsP2PPacketAvailable(uint pcubMsgSize, int nChannel)
    {
        Write("IsP2PPacketAvailable");
        return false;
    }

    public bool ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel)
    {
        Write("ReadP2PPacket");
        return false;
    }

    public bool AcceptP2PSessionWithUser(IntPtr steamIDRemote)
    {
        Write("AcceptP2PSessionWithUser");
        return false;
    }

    public bool CloseP2PSessionWithUser(IntPtr steamIDRemote)
    {
        Write("CloseP2PSessionWithUser");
        return false;
    }

    public bool CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel)
    {
        Write("CloseP2PChannelWithUser");
        return false;
    }

    public bool GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState)
    {
        Write("GetP2PSessionState");
        return false;
    }

    public bool AllowP2PPacketRelay(bool bAllow)
    {
        Write("AllowP2PPacketRelay");
        return false;
    }

    public uint CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay)
    {
        Write("CreateListenSocket");
        return 0;
    }

    public uint CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
    {
        Write("CreateP2PConnectionSocket");
        return 0;
    }

    public uint CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec)
    {
        Write("CreateConnectionSocket");
        return 0;
    }

    public bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd)
    {
        Write("DestroySocket");
        return false;
    }

    public bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd)
    {
        Write("DestroyListenSocket");
        return false;
    }

    public bool SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable)
    {
        Write("SendDataOnSocket");
        return false;
    }

    public bool IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize)
    {
        Write("IsDataAvailableOnSocket");
        return false;
    }

    public bool RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
    {
        Write("RetrieveDataFromSocket");
        return false;
    }

    public bool IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket)
    {
        Write("IsDataAvailable");
        return false;
    }

    public bool RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket)
    {
        Write("RetrieveData");
        return false;
    }

    public bool GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote)
    {
        Write("GetSocketInfo");
        return false;
    }

    public bool GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort)
    {
        Write("GetListenSocketInfo");
        return false;
    }

    public ESNetSocketConnectionType GetSocketConnectionType(uint hSocket)
    {
        Write("GetSocketConnectionType");
        return default;
    }

    public int GetMaxPacketSize(uint hSocket)
    {
        Write("GetMaxPacketSize");
        return 1500;
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}