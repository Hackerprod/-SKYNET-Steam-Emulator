using Core.Interface;
using SKYNET;

using System;

//[Map("SteamMatchMakingServers")]
public class SteamMatchMakingServers : IBaseInterface
{
    public IntPtr RequestInternetServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public IntPtr RequestLANServerList(IntPtr iApp, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public IntPtr RequestFriendsServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public IntPtr RequestFavoritesServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public IntPtr RequestHistoryServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public IntPtr RequestSpectatorServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        return IntPtr.Zero;
    }

    public void ReleaseRequest(IntPtr hServerListRequest)
    {
        //
    }

    public IntPtr GetServerDetails(IntPtr _, IntPtr hRequest, int iServer)
    {
        return IntPtr.Zero;
    }

    public void CancelQuery(IntPtr hRequest)
    {
        //
    }

    public void RefreshQuery(IntPtr hRequest)
    {
        //
    }

    public bool IsRefreshing(IntPtr hRequest)
    {
        return false;
    }

    public int GetServerCount(IntPtr hRequest)
    {
        return 0;
    }

    public void RefreshServer(IntPtr hRequest, int iServer)
    {
        //
    }

    public uint PingServer(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        return 0;
    }

    public uint PlayerDetails(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        return 0;
    }

    public uint ServerRules(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        return 0;
    }

    public void CancelServerQuery(uint hServerQuery)
    {
        //
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}