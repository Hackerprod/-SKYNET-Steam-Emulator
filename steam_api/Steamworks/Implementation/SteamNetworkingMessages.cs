using SKYNET;
using SKYNET.Helper;
using System;

public class SteamNetworkingMessages : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


    public int SendMessageToUser(IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
    {
        Write("SendMessageToUser");
        return 0;
    }

    public int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages)
    {
        Write("ReceiveMessagesOnChannel");
        return 0;
    }

    public bool AcceptSessionWithUser(IntPtr identityRemote)
    {
        Write("AcceptSessionWithUser");
        return true;
    }

    public bool CloseSessionWithUser(IntPtr identityRemote)
    {
        Write("CloseSessionWithUser");
        return false;
    }

    public bool CloseChannelWithUser(IntPtr identityRemote, int nLocalChannel)
    {
        Write("CloseChannelWithUser");
        return false;
    }

    public IntPtr GetSessionConnectionInfo(IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus)
    {
        Write("GetSessionConnectionInfo");
        return IntPtr.Zero;
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}