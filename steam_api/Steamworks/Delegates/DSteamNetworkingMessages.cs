using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamNetworkingMessages")]
    public class DSteamNetworkingMessages : SteamDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int SendMessageToUser( IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AcceptSessionWithUser( IntPtr identityRemote );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseSessionWithUser( IntPtr identityRemote );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseChannelWithUser( IntPtr identityRemote, int nLocalChannel );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetSessionConnectionInfo( IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus );

    }
}
