using SKYNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamMatchmakingServers : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestInternetServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestInternetServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestInternetServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestLANServerList(IntPtr iApp, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestLANServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestLANServerList(iApp, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestFriendsServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestFavoritesServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestHistoryServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList(IntPtr iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestSpectatorServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
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
            return SteamEmulator.SteamMatchMakingServers.GetServerDetails(SteamEmulator.SteamMatchMakingServers.MemoryAddress, hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_CancelQuery(IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_CancelQuery");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_RefreshQuery(IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RefreshQuery");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmakingServers_IsRefreshing(IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_IsRefreshing");
            return SteamEmulator.SteamMatchMakingServers.IsRefreshing(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmakingServers_GetServerCount(IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_GetServerCount");
            return SteamEmulator.SteamMatchMakingServers.GetServerCount(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_RefreshServer(IntPtr hRequest, int iServer)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RefreshServer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamMatchmakingServers_PingServer(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_PingServer");
            return SteamEmulator.SteamMatchMakingServers.PingServer(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamMatchmakingServers_PlayerDetails(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_PlayerDetails");
            return SteamEmulator.SteamMatchMakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamMatchmakingServers_ServerRules(uint unIP, uint usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_ServerRules");
            return SteamEmulator.SteamMatchMakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_CancelServerQuery(uint hServerQuery)
        {
            Write("SteamAPI_ISteamMatchmakingServers_CancelServerQuery");
        }
    }
}

