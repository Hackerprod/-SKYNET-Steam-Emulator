using SKYNET;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchMakingServers : ISteamInterface
    {
        public SteamMatchMakingServers()
        {
            InterfaceName = "SteamMatchMakingServers";
        }

        public IntPtr RequestInternetServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestInternetServerList");
            return IntPtr.Zero;
        }

        public IntPtr RequestLANServerList(uint iApp, IntPtr pRequestServersResponse)
        {
            Write($"RequestLANServerList");
            return IntPtr.Zero;
        }

        public IntPtr RequestFriendsServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestFriendsServerList");
            return IntPtr.Zero;
        }

        public IntPtr RequestFavoritesServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestFavoritesServerList");
            return IntPtr.Zero;
        }

        public IntPtr RequestHistoryServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestHistoryServerList");
            return IntPtr.Zero;
        }

        public IntPtr RequestSpectatorServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestSpectatorServerList");
            return IntPtr.Zero;
        }

        public void ReleaseRequest(IntPtr hServerListRequest)
        {
            Write($"ReleaseRequest");
        }

        public IntPtr GetServerDetails(IntPtr hRequest, int iServer)
        {
            Write($"GetServerDetails");
            return IntPtr.Zero;
        }

        public void CancelQuery(IntPtr hRequest)
        {
            Write($"CancelQuery");
        }

        public void RefreshQuery(IntPtr hRequest)
        {
            Write($"RefreshQuery");
        }

        public bool IsRefreshing(IntPtr hRequest)
        {
            Write($"IsRefreshing");
            return false;
        }

        public int GetServerCount(IntPtr hRequest)
        {
            Write($"GetServerCount");
            return 0;
        }

        public void RefreshServer(IntPtr hRequest, int iServer)
        {
            Write($"RefreshServer");
        }

        public uint PingServer(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write($"PingServer");
            return 0;
        }

        public uint PlayerDetails(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write($"PlayerDetails");
            return 0;
        }

        public uint ServerRules(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write($"ServerRules");
            return 0;
        }

        public void CancelServerQuery(int hServerQuery)
        {
            Write($"CancelServerQuery");
        }
    }
}