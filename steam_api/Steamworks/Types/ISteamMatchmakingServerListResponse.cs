using System;
using System.Runtime.InteropServices;

using HServerListRequest = System.IntPtr;

namespace SKYNET.Steamworks.Types
{
    public struct ISteamMatchmakingServerListResponse
    {
        public ServerRespondedDelegate ServerResponded;
        public ServerFailedToRespondDelegate ServerFailedToRespond;
        public RefreshCompleteDelegate RefreshComplete;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ServerRespondedDelegate(IntPtr _, HServerListRequest hRequest, int iServer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ServerFailedToRespondDelegate(IntPtr _, HServerListRequest hRequest, int iServer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RefreshCompleteDelegate(IntPtr _, HServerListRequest hRequest, EMatchMakingServerResponse response);
    };
}
