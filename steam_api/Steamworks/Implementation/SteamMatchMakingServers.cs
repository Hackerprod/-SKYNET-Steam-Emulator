using SKYNET;
using SKYNET.Helper;
using System;

public class SteamMatchMakingServers : SteamInterface
{
    public IntPtr RequestInternetServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write($"RequestInternetServerList");
        return IntPtr.Zero;
    }

    public IntPtr RequestLANServerList(IntPtr iApp, IntPtr pRequestServersResponse)
    {
        Write($"RequestLANServerList");
        return IntPtr.Zero;
    }

    public IntPtr RequestFriendsServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write($"RequestFriendsServerList");
        return IntPtr.Zero;
    }

    public IntPtr RequestFavoritesServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write($"RequestFavoritesServerList");
        return IntPtr.Zero;
    }

    public IntPtr RequestHistoryServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write($"RequestHistoryServerList");
        return IntPtr.Zero;
    }

    public IntPtr RequestSpectatorServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write($"RequestSpectatorServerList");
        return IntPtr.Zero;
    }

    public void ReleaseRequest(IntPtr hServerListRequest)
    {
        Write($"ReleaseRequest");
    }

    public IntPtr GetServerDetails(IntPtr _, IntPtr hRequest, int iServer)
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

    public void CancelServerQuery(uint hServerQuery)
    {
        Write($"CancelServerQuery");
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}