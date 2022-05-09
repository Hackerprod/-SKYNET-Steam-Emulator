using System;


namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamMatchMakingServers002")]
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

        public int PingServer(IntPtr _, uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PingServer(unIP, usPort, pRequestServersResponse);
        }

        public int PlayerDetails(IntPtr _, uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
        }

        public int ServerRules(IntPtr _, uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
        }

        public void CancelServerQuery(IntPtr _, int hServerQuery)
        {
            SteamEmulator.SteamMatchMakingServers.CancelServerQuery(hServerQuery);
        }


    }
}
