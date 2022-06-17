using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamNetworkingMessages
    {
        static SteamAPI_ISteamNetworkingMessages()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingMessages_SendMessageToUser(IntPtr _, IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
        {
            Write("SteamAPI_ISteamNetworkingMessages_SendMessageToUser");
            return SteamEmulator.SteamNetworkingMessages.SendMessageToUser(identityRemote, pubData, cubData, nSendFlags, nRemoteChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingMessages_ReceiveMessagesOnChannel(IntPtr _, int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages)
        {
            Write("SteamAPI_ISteamNetworkingMessages_ReceiveMessagesOnChannel");
            return SteamEmulator.SteamNetworkingMessages.ReceiveMessagesOnChannel(nLocalChannel, ppOutMessages, nMaxMessages);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingMessages_AcceptSessionWithUser(IntPtr _, IntPtr identityRemote)
        {
            Write("SteamAPI_ISteamNetworkingMessages_AcceptSessionWithUser");
            return SteamEmulator.SteamNetworkingMessages.AcceptSessionWithUser(identityRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingMessages_CloseSessionWithUser(IntPtr _, IntPtr identityRemote)
        {
            Write("SteamAPI_ISteamNetworkingMessages_CloseSessionWithUser");
            return SteamEmulator.SteamNetworkingMessages.CloseSessionWithUser(identityRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingMessages_CloseChannelWithUser(IntPtr _, IntPtr identityRemote, int nLocalChannel)
        {
            Write("SteamAPI_ISteamNetworkingMessages_CloseChannelWithUser");
            return SteamEmulator.SteamNetworkingMessages.CloseChannelWithUser(identityRemote, nLocalChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamNetworkingMessages_GetSessionConnectionInfo(IntPtr _, IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus)
        {
            Write("SteamAPI_ISteamNetworkingMessages_GetSessionConnectionInfo");
            return SteamEmulator.SteamNetworkingMessages.GetSessionConnectionInfo(identityRemote, pConnectionInfo, pQuickStatus);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

