using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks.Types;
using System;
using System.Runtime.InteropServices;

using HServerListRequest = System.IntPtr;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchMakingServers : ISteamInterface
    {
        public static SteamMatchMakingServers Instance;

        public SteamMatchMakingServers()
        {
            Instance = this;
            InterfaceName = "SteamMatchMakingServers";
            InterfaceVersion = "SteamMatchMakingServers002";
        }

        public HServerListRequest RequestInternetServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestInternetServerList querying LAN list");
            return RequestLANServerList(iApp, pRequestServersResponse);
        }

        public HServerListRequest RequestLANServerList(uint iApp, IntPtr pRequestServersResponse)
        {
            Write($"RequestLANServerList");
            var response = pRequestServersResponse.ToType<ISteamMatchmakingServerListResponse>();
            //response.RefreshComplete(IntPtr.Zero, EMatchMakingServerResponse.eServerResponded);
            //response.m_RefreshComplete(IntPtr.Zero, EMatchMakingServerResponse.eServerResponded);
            //response.m_ServerResponded(IntPtr.Zero, 225);
            return IntPtr.Zero;
        }

        public HServerListRequest RequestFriendsServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestFriendsServerList");
            return IntPtr.Zero;
        }

        public HServerListRequest RequestFavoritesServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestFavoritesServerList");
            return IntPtr.Zero;
        }

        public HServerListRequest RequestHistoryServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestHistoryServerList");
            return IntPtr.Zero;
        }

        public HServerListRequest RequestSpectatorServerList(uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write($"RequestSpectatorServerList");
            return IntPtr.Zero;
        }

        public void ReleaseRequest(IntPtr hServerListRequest)
        {
            Write($"ReleaseRequest");
        }

        public IntPtr GetServerDetails(HServerListRequest hRequest, int iServer)
        {
            Write($"GetServerDetails");
            return IntPtr.Zero;
        }

        public void CancelQuery(HServerListRequest hRequest)
        {
            Write($"CancelQuery");
        }

        public void RefreshQuery(HServerListRequest hReques)
        {
            Write($"RefreshQuery");
        }

        public bool IsRefreshing(HServerListRequest hReques)
        {
            Write($"IsRefreshing");
            return false;
        }

        public int GetServerCount(HServerListRequest hRequest)
        {
            Write($"GetServerCount");
            return 10;
        }

        public void RefreshServer(HServerListRequest hReques, int iServer)
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