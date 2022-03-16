using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamMatchmakingServers")]
    public class DSteamMatchmakingServers : IBaseInterfaceMap
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestInternetServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestLANServerList(IntPtr iApp, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestFriendsServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestFavoritesServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestHistoryServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr RequestSpectatorServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ReleaseRequest(IntPtr hServerListRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CancelQuery(IntPtr hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RefreshQuery(IntPtr hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsRefreshing(IntPtr hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetServerCount(IntPtr hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RefreshServer(IntPtr hRequest, int iServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint PingServer(uint unIP, uint usPort, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint PlayerDetails(uint unIP, uint usPort, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint ServerRules(uint unIP, uint usPort, IntPtr pRequestServersResponse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CancelServerQuery(uint hServerQuery);
    }
}
