using Core.Interface;
using SKYNET.Interface;
using System;

public class SteamNetworkingMessages : IBaseInterface, ISteamNetworkingMessages
{
    public int SendMessageToUser(IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
    {
        return 0;
    }

    public int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages)
    {
        return 0;
    }

    public bool AcceptSessionWithUser(IntPtr identityRemote)
    {
        return default;
    }

    public bool CloseSessionWithUser(IntPtr identityRemote)
    {
        return default;
    }

    public bool CloseChannelWithUser(IntPtr identityRemote, int nLocalChannel)
    {
        return default;
    }

    public IntPtr GetSessionConnectionInfo(IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus)
    {
        return IntPtr.Zero;
    }

}