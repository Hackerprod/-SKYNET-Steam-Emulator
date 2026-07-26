using System;
using System.Runtime.InteropServices;
using SKYNET.Steamworks.Types;

namespace SKYNET.Steamworks.Exported
{
    using HServerListRequest = System.IntPtr;
    public class SteamAPI_ISteamMatchmakingServers
    {
        static SteamAPI_ISteamMatchmakingServers()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestInternetServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestInternetServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestInternetServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestLANServerList(IntPtr _, uint iApp, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestLANServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestLANServerList(iApp, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestFriendsServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestFavoritesServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestHistoryServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList(IntPtr _, uint iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList");
            return SteamEmulator.SteamMatchMakingServers.RequestSpectatorServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_ReleaseRequest(IntPtr _, IntPtr hServerListRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_ReleaseRequest");
            SteamEmulator.SteamMatchMakingServers.ReleaseRequest(hServerListRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamMatchmakingServers_GetServerDetails(IntPtr _, IntPtr hRequest, int iServer)
        {
            Write("SteamAPI_ISteamMatchmakingServers_GetServerDetails");
            return SteamEmulator.SteamMatchMakingServers.GetServerDetails(hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_CancelQuery(IntPtr _, IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_CancelQuery");
            SteamEmulator.SteamMatchMakingServers.CancelQuery(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_RefreshQuery(IntPtr _, IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RefreshQuery");
            SteamEmulator.SteamMatchMakingServers.RefreshQuery(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmakingServers_IsRefreshing(IntPtr _, IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_IsRefreshing");
            return SteamEmulator.SteamMatchMakingServers.IsRefreshing(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmakingServers_GetServerCount(IntPtr _, IntPtr hRequest)
        {
            Write("SteamAPI_ISteamMatchmakingServers_GetServerCount");
            return SteamEmulator.SteamMatchMakingServers.GetServerCount(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_RefreshServer(IntPtr _, IntPtr hRequest, int iServer)
        {
            Write("SteamAPI_ISteamMatchmakingServers_RefreshServer");
            SteamEmulator.SteamMatchMakingServers.RefreshServer(hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmakingServers_PingServer(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_PingServer");
            return SteamEmulator.SteamMatchMakingServers.PingServer(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmakingServers_PlayerDetails(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_PlayerDetails");
            return SteamEmulator.SteamMatchMakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmakingServers_ServerRules(IntPtr _, uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            Write("SteamAPI_ISteamMatchmakingServers_ServerRules");
            return SteamEmulator.SteamMatchMakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServers_CancelServerQuery(IntPtr _, int hServerQuery)
        {
            Write("SteamAPI_ISteamMatchmakingServers_CancelServerQuery");
            SteamEmulator.SteamMatchMakingServers.CancelServerQuery(hServerQuery);
        }

        // ISteamMatchmakingServerListResponse
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServerListResponse_ServerResponded(IntPtr _, HServerListRequest hRequest, int iServer)
        {
            Write("SteamAPI_ISteamMatchmakingServerListResponse_ServerResponded");
            NativeMatchmakingCallbacks.ServerResponded(_, hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServerListResponse_ServerFailedToRespond(IntPtr _, HServerListRequest hRequest, int iServer)
        {
            Write("SteamAPI_ISteamMatchmakingServerListResponse_ServerFailedToRespond");
            NativeMatchmakingCallbacks.ServerFailedToRespond(_, hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingServerListResponse_RefreshComplete(IntPtr _, HServerListRequest hRequest, EMatchMakingServerResponse response)
        {
            Write("SteamAPI_ISteamMatchmakingServerListResponse_RefreshComplete");
            NativeMatchmakingCallbacks.RefreshComplete(_, hRequest, response);
        }

        // ISteamMatchmakingPingResponse
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingPingResponse_ServerResponded(IntPtr _, IntPtr server )
        {
            Write("SteamAPI_ISteamMatchmakingPingResponse_ServerResponded");
            NativeMatchmakingCallbacks.PingResponded(_, server);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingPingResponse_ServerFailedToRespond(IntPtr _)
        {
            Write("SteamAPI_ISteamMatchmakingPingResponse_ServerFailedToRespond");
            NativeMatchmakingCallbacks.PingFailed(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingPlayersResponse_AddPlayerToList(
            IntPtr _,
            string pchName,
            int nScore,
            float flTimePlayed)
        {
            NativeMatchmakingCallbacks.AddPlayer(_, pchName, nScore, flTimePlayed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingPlayersResponse_PlayersFailedToRespond(IntPtr _)
        {
            NativeMatchmakingCallbacks.PlayersFailed(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingPlayersResponse_PlayersRefreshComplete(IntPtr _)
        {
            NativeMatchmakingCallbacks.PlayersComplete(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingRulesResponse_RulesResponded(
            IntPtr _,
            string pchRule,
            string pchValue)
        {
            NativeMatchmakingCallbacks.RuleResponded(_, pchRule, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingRulesResponse_RulesFailedToRespond(IntPtr _)
        {
            NativeMatchmakingCallbacks.RulesFailed(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmakingRulesResponse_RulesRefreshComplete(IntPtr _)
        {
            NativeMatchmakingCallbacks.RulesComplete(_);
        }


        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
