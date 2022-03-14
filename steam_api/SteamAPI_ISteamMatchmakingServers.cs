using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamMatchmakingServers : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestInternetServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestInternetServerList");
        return SteamClient.SteamMatchmakingServers.RequestInternetServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestLANServerList(IntPtr iApp, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestLANServerList");
        return SteamClient.SteamMatchmakingServers.RequestLANServerList(iApp, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList");
        return SteamClient.SteamMatchmakingServers.RequestFriendsServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList");
        return SteamClient.SteamMatchmakingServers.RequestFavoritesServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList");
        return SteamClient.SteamMatchmakingServers.RequestHistoryServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList");
        return SteamClient.SteamMatchmakingServers.RequestSpectatorServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMatchmakingServers_ReleaseRequest(IntPtr hServerListRequest)
    {
        Write("SteamAPI_ISteamMatchmakingServers_ReleaseRequest");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamMatchmakingServers_GetServerDetails(IntPtr hRequest, int iServer)
    {
        Write("SteamAPI_ISteamMatchmakingServers_GetServerDetails");
        return SteamClient.SteamMatchmakingServers.GetServerDetails(hRequest, iServer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMatchmakingServers_CancelQuery(IntPtr hRequest)
    {
        Write("SteamAPI_ISteamMatchmakingServers_CancelQuery");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMatchmakingServers_RefreshQuery(IntPtr hRequest)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RefreshQuery");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMatchmakingServers_IsRefreshing(IntPtr hRequest)
    {
        Write("SteamAPI_ISteamMatchmakingServers_IsRefreshing");
        return SteamClient.SteamMatchmakingServers.IsRefreshing(hRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamMatchmakingServers_GetServerCount(IntPtr hRequest)
    {
        Write("SteamAPI_ISteamMatchmakingServers_GetServerCount");
        return SteamClient.SteamMatchmakingServers.GetServerCount(hRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMatchmakingServers_RefreshServer(IntPtr hRequest, int iServer)
    {
        Write("SteamAPI_ISteamMatchmakingServers_RefreshServer");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamMatchmakingServers_PingServer(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_PingServer");
        return SteamClient.SteamMatchmakingServers.PingServer(unIP, usPort, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamMatchmakingServers_PlayerDetails(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_PlayerDetails");
        return SteamClient.SteamMatchmakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamMatchmakingServers_ServerRules(uint unIP, uint usPort, IntPtr pRequestServersResponse)
    {
        Write("SteamAPI_ISteamMatchmakingServers_ServerRules");
        return SteamClient.SteamMatchmakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMatchmakingServers_CancelServerQuery(uint hServerQuery)
    {
        Write("SteamAPI_ISteamMatchmakingServers_CancelServerQuery");
        //
    }

}

