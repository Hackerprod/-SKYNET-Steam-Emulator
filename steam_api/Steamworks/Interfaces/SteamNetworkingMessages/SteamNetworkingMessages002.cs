using System;

using SteamNetworkingPOPID = System.UInt32;
using HSteamNetConnection = System.UInt32;
using HSteamListenSocket = System.UInt32;
using HSteamNetPollGroup = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamNetworkingMessages002")]
    public class SteamNetworkingMessages002 : ISteamInterface
    {
        public int SendMessageToUser(IntPtr _, IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
        {
            return SteamEmulator.SteamNetworkingMessages.SendMessageToUser(identityRemote, pubData, cubData, nSendFlags, nRemoteChannel);
        }

        public int ReceiveMessagesOnChannel(IntPtr _, int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages)
        {
            return SteamEmulator.SteamNetworkingMessages.ReceiveMessagesOnChannel(nLocalChannel, ppOutMessages, nMaxMessages);
        }

        public bool AcceptSessionWithUser(IntPtr _, IntPtr identityRemote)
        {
            return SteamEmulator.SteamNetworkingMessages.AcceptSessionWithUser(identityRemote);
        }

        public bool CloseSessionWithUser(IntPtr _, IntPtr identityRemote)
        {
            return SteamEmulator.SteamNetworkingMessages.CloseSessionWithUser(identityRemote);
        }

        public bool CloseChannelWithUser(IntPtr _, IntPtr identityRemote, int nLocalChannel)
        {
            return SteamEmulator.SteamNetworkingMessages.CloseChannelWithUser(identityRemote, nLocalChannel);
        }

        public IntPtr GetSessionConnectionInfo(IntPtr _, IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus)
        {
            return SteamEmulator.SteamNetworkingMessages.GetSessionConnectionInfo(identityRemote, pConnectionInfo, pQuickStatus);
        }
    }
}
