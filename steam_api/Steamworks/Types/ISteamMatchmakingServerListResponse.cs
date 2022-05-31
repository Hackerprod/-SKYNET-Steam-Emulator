using System;
using System.Runtime.InteropServices;
using HServerListRequest = System.IntPtr;

namespace SKYNET.Steamworks.Types
{
    public struct ISteamMatchmakingServerListResponse
    {
        // Server has responded ok with updated data
        public ServerRespondedDelegate ServerResponded;

        // Server has failed to respond
        public ServerFailedToRespondDelegate ServerFailedToRespond;

        // A list refresh you had initiated is now 100% completed
        public RefreshCompleteDelegate RefreshComplete;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ServerRespondedDelegate(IntPtr _, HServerListRequest hRequest, int iServer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ServerFailedToRespondDelegate(IntPtr _, HServerListRequest hRequest, int iServer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RefreshCompleteDelegate(IntPtr _, HServerListRequest hRequest, EMatchMakingServerResponse response);
    };
}
