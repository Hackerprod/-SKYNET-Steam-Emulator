using System;
using System.Collections.Generic;


namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamMatchMakingServers001",
        "RequestInternetServerList__V001", "RequestLANServerList__V001", "RequestFriendsServerList__V001", "RequestFavoritesServerList__V001",
        "RequestHistoryServerList__V001", "RequestSpectatorServerList__V001", "GetServerDetails__V001", "CancelQuery__V001",
        "RefreshQuery__V001", "IsRefreshing__V001", "GetServerCount__V001", "RefreshServer__V001",
        "PingServer", "PlayerDetails", "ServerRules", "CancelServerQuery")]
    [InterfaceLayout("SteamMatchMakingServers002",
        "RequestInternetServerList", "RequestLANServerList", "RequestFriendsServerList", "RequestFavoritesServerList",
        "RequestHistoryServerList", "RequestSpectatorServerList", "ReleaseRequest", "GetServerDetails",
        "CancelQuery", "RefreshQuery", "IsRefreshing", "GetServerCount",
        "RefreshServer", "PingServer", "PlayerDetails", "ServerRules",
        "CancelServerQuery")]
    [InterfaceLayout("SteamMatchMakingServers003",
        "RequestInternetServerList", "RequestLANServerList", "RequestFriendsServerList", "RequestFavoritesServerList",
        "RequestHistoryServerList", "RequestSpectatorServerList", "ReleaseRequest", "GetServerDetails",
        "CancelQuery", "RefreshQuery", "IsRefreshing", "GetServerCount",
        "RefreshServer", "PingServer", "PlayerDetails", "ServerRules",
        "ServerFriends", "CancelServerQuery")]
    public class SteamMatchMakingServers002 : ISteamInterface
    {
        public IntPtr RequestInternetServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestInternetServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        public IntPtr RequestLANServerList(IntPtr _, uint iApp, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestLANServerList(iApp, pRequestServersResponse);
        }

        public IntPtr RequestFriendsServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestFriendsServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        public IntPtr RequestFavoritesServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestFavoritesServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        public IntPtr RequestHistoryServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestHistoryServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        public IntPtr RequestSpectatorServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestSpectatorServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        public void ReleaseRequest(IntPtr _, IntPtr IntPtr)
        {
            SteamEmulator.SteamMatchMakingServers.ReleaseRequest(IntPtr);
        }

        public IntPtr GetServerDetails(IntPtr _, IntPtr hRequest, int iServer)
        {
            return SteamEmulator.SteamMatchMakingServers.GetServerDetails(hRequest, iServer);
        }

        public void CancelQuery(IntPtr _, IntPtr hRequest)
        {
            SteamEmulator.SteamMatchMakingServers.CancelQuery(hRequest);
        }

        public void RefreshQuery(IntPtr _, IntPtr hRequest)
        {
            SteamEmulator.SteamMatchMakingServers.RefreshQuery(hRequest);
        }

        public bool IsRefreshing(IntPtr _, IntPtr hRequest)
        {
            return SteamEmulator.SteamMatchMakingServers.IsRefreshing(hRequest);
        }

        public int GetServerCount(IntPtr _, IntPtr hRequest)
        {
            return SteamEmulator.SteamMatchMakingServers.GetServerCount(hRequest);
        }

        public void RefreshServer(IntPtr _, IntPtr hRequest, int iServer)
        {
            SteamEmulator.SteamMatchMakingServers.RefreshServer(hRequest, iServer);
        }

        public int PingServer(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PingServer(unIP, usPort, pRequestServersResponse);
        }

        public int PlayerDetails(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
        }

        public int ServerRules(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
        }

        public int ServerFriends(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            LogStub("ServerFriends");
            return -1;
        }

        public void CancelServerQuery(IntPtr _, int hServerQuery)
        {
            SteamEmulator.SteamMatchMakingServers.CancelServerQuery(hServerQuery);
        }

        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static void LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (!_stubsLogged.Add(method))
                {
                    return;
                }
            }

            SteamEmulator.Write("SteamMatchMakingServers", method + " not implemented");
        }

        // SteamMatchMakingServers001 lists are keyed by EMatchMakingType and use the 001 response vtable, which the emulator core cannot call back.
        public void RequestInternetServerList__V001(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            LogStub("RequestInternetServerList");
        }

        public void RequestLANServerList__V001(IntPtr _, uint iApp, IntPtr pRequestServersResponse)
        {
            LogStub("RequestLANServerList");
        }

        public void RequestFriendsServerList__V001(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            LogStub("RequestFriendsServerList");
        }

        public void RequestFavoritesServerList__V001(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            LogStub("RequestFavoritesServerList");
        }

        public void RequestHistoryServerList__V001(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            LogStub("RequestHistoryServerList");
        }

        public void RequestSpectatorServerList__V001(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            LogStub("RequestSpectatorServerList");
        }

        public IntPtr GetServerDetails__V001(IntPtr _, int eType, int iServer)
        {
            LogStub("GetServerDetails");
            return IntPtr.Zero;
        }

        public void CancelQuery__V001(IntPtr _, int eType)
        {
            LogStub("CancelQuery");
        }

        public void RefreshQuery__V001(IntPtr _, int eType)
        {
            LogStub("RefreshQuery");
        }

        public bool IsRefreshing__V001(IntPtr _, int eType)
        {
            LogStub("IsRefreshing");
            return false;
        }

        public int GetServerCount__V001(IntPtr _, int eType)
        {
            LogStub("GetServerCount");
            return 0;
        }

        public void RefreshServer__V001(IntPtr _, int eType, int iServer)
        {
            LogStub("RefreshServer");
        }


    }
}
