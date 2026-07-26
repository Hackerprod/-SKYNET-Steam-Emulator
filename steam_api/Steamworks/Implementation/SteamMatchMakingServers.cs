using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Steamworks.Types;
using SKYNET.Types;

using HServerListRequest = System.IntPtr;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchMakingServers : ISteamInterface
    {
        private const int MatchmakingFilterSize = 512;
        private const int MatchmakingFilterFieldSize = 256;
        private const uint MaxFilterCount = 256;

        private readonly ConcurrentDictionary<IntPtr, ListRequestState> _listRequests =
            new ConcurrentDictionary<IntPtr, ListRequestState>();
        private readonly ConcurrentDictionary<int, ServerQueryState> _serverQueries =
            new ConcurrentDictionary<int, ServerQueryState>();
        private long _nextListRequest;
        private int _nextServerQuery;

        public static SteamMatchMakingServers Instance;

        public SteamMatchMakingServers()
        {
            Instance = this;
            InterfaceName = "SteamMatchMakingServers";
            InterfaceVersion = "SteamMatchMakingServers002";
        }

        public HServerListRequest RequestInternetServerList(
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            Write($"RequestInternetServerList (AppId={appId}, Filters={filterCount})");
            return RequestServerList(ServerListKind.Internet, appId, filters, filterCount, response);
        }

        public HServerListRequest RequestLANServerList(uint appId, IntPtr response)
        {
            Write($"RequestLANServerList (AppId={appId})");
            return RequestServerList(ServerListKind.Lan, appId, IntPtr.Zero, 0, response);
        }

        public HServerListRequest RequestFriendsServerList(
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            Write($"RequestFriendsServerList (AppId={appId}, Filters={filterCount})");
            return RequestServerList(ServerListKind.Friends, appId, filters, filterCount, response);
        }

        public HServerListRequest RequestFavoritesServerList(
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            Write($"RequestFavoritesServerList (AppId={appId}, Filters={filterCount})");
            return RequestServerList(ServerListKind.Favorites, appId, filters, filterCount, response);
        }

        public HServerListRequest RequestHistoryServerList(
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            Write($"RequestHistoryServerList (AppId={appId}, Filters={filterCount})");
            return RequestServerList(ServerListKind.History, appId, filters, filterCount, response);
        }

        public HServerListRequest RequestSpectatorServerList(
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            Write($"RequestSpectatorServerList (AppId={appId}, Filters={filterCount})");
            return RequestServerList(ServerListKind.Spectator, appId, filters, filterCount, response);
        }

        public void ReleaseRequest(IntPtr request)
        {
            Write($"ReleaseRequest (Request=0x{request.ToInt64():X})");
            if (!_listRequests.TryRemove(request, out var state))
            {
                return;
            }

            lock (state.Gate)
            {
                state.Released = true;
                state.Canceled = true;
                state.Refreshing = false;
                state.Generation++;
                foreach (var item in state.AllocatedItems)
                {
                    Marshal.FreeHGlobal(item);
                }

                state.AllocatedItems.Clear();
                state.CurrentItems.Clear();
            }
        }

        public IntPtr GetServerDetails(HServerListRequest request, int serverIndex)
        {
            if (!_listRequests.TryGetValue(request, out var state))
            {
                return IntPtr.Zero;
            }

            lock (state.Gate)
            {
                return serverIndex >= 0 && serverIndex < state.CurrentItems.Count
                    ? state.CurrentItems[serverIndex]
                    : IntPtr.Zero;
            }
        }

        public void CancelQuery(HServerListRequest request)
        {
            Write($"CancelQuery (Request=0x{request.ToInt64():X})");
            if (!_listRequests.TryGetValue(request, out var state))
            {
                return;
            }

            lock (state.Gate)
            {
                state.Canceled = true;
                state.Refreshing = false;
                state.Generation++;
            }
        }

        public void RefreshQuery(HServerListRequest request)
        {
            Write($"RefreshQuery (Request=0x{request.ToInt64():X})");
            if (_listRequests.TryGetValue(request, out var state))
            {
                BeginRefresh(state);
            }
        }

        public bool IsRefreshing(HServerListRequest request)
        {
            if (!_listRequests.TryGetValue(request, out var state))
            {
                return false;
            }

            lock (state.Gate)
            {
                return state.Refreshing && !state.Canceled && !state.Released;
            }
        }

        public int GetServerCount(HServerListRequest request)
        {
            if (!_listRequests.TryGetValue(request, out var state))
            {
                return 0;
            }

            lock (state.Gate)
            {
                return state.CurrentItems.Count;
            }
        }

        public void RefreshServer(HServerListRequest request, int serverIndex)
        {
            Write($"RefreshServer (Request=0x{request.ToInt64():X}, Index={serverIndex})");
            if (!_listRequests.TryGetValue(request, out var state))
            {
                return;
            }

            GameServerData previous;
            int generation;
            lock (state.Gate)
            {
                if (state.Released || state.Canceled ||
                    serverIndex < 0 || serverIndex >= state.Servers.Count)
                {
                    return;
                }

                previous = state.Servers[serverIndex];
                generation = state.Generation;
            }

            WorkQueue.Enqueue(
                "Refresh game server",
                () => CompleteServerRefresh(state, generation, serverIndex, previous),
                $"server-browser:refresh:{request.ToInt64()}:{serverIndex}:{generation}",
                true);
        }

        public int PingServer(uint ip, ushort port, IntPtr response)
        {
            Write($"PingServer (IP={FormatIp(ip)}, Port={port})");
            return BeginServerQuery(
                ServerQueryKind.Ping,
                ip,
                port,
                response,
                (query, server) =>
                {
                    if (server == null)
                    {
                        NativeMatchmakingCallbacks.PingFailed(query.Response);
                        return;
                    }

                    var native = NativeGameServerItem.Allocate(server, 1);
                    try
                    {
                        NativeMatchmakingCallbacks.PingResponded(query.Response, native);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(native);
                    }
                });
        }

        public int PlayerDetails(uint ip, ushort port, IntPtr response)
        {
            Write($"PlayerDetails (IP={FormatIp(ip)}, Port={port})");
            return BeginServerQuery(
                ServerQueryKind.Players,
                ip,
                port,
                response,
                (query, server) =>
                {
                    if (server == null)
                    {
                        NativeMatchmakingCallbacks.PlayersFailed(query.Response);
                        return;
                    }

                    foreach (var player in server.Players.Values.OrderBy(player => player.SteamId))
                    {
                        NativeMatchmakingCallbacks.AddPlayer(
                            query.Response,
                            player.Name,
                            player.Score,
                            player.GetTimePlayedSeconds());
                    }

                    NativeMatchmakingCallbacks.PlayersComplete(query.Response);
                });
        }

        public int ServerRules(uint ip, ushort port, IntPtr response)
        {
            Write($"ServerRules (IP={FormatIp(ip)}, Port={port})");
            return BeginServerQuery(
                ServerQueryKind.Rules,
                ip,
                port,
                response,
                (query, server) =>
                {
                    if (server == null)
                    {
                        NativeMatchmakingCallbacks.RulesFailed(query.Response);
                        return;
                    }

                    foreach (var pair in BuildRules(server).OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        NativeMatchmakingCallbacks.RuleResponded(query.Response, pair.Key, pair.Value);
                    }

                    NativeMatchmakingCallbacks.RulesComplete(query.Response);
                });
        }

        public void CancelServerQuery(int queryHandle)
        {
            Write($"CancelServerQuery (Query={queryHandle})");
            if (_serverQueries.TryRemove(queryHandle, out var query))
            {
                query.Canceled = true;
            }
        }

        private IntPtr RequestServerList(
            ServerListKind kind,
            uint appId,
            IntPtr filters,
            uint filterCount,
            IntPtr response)
        {
            if (response == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var requestValue = Interlocked.Increment(ref _nextListRequest);
            if (requestValue == 0)
            {
                requestValue = Interlocked.Increment(ref _nextListRequest);
            }

            var request = new IntPtr(requestValue);
            var state = new ListRequestState
            {
                Handle = request,
                Kind = kind,
                AppId = appId,
                Response = response,
                Filters = ReadFilters(filters, filterCount)
            };

            if (!_listRequests.TryAdd(request, state))
            {
                return IntPtr.Zero;
            }

            BeginRefresh(state);
            return request;
        }

        private void BeginRefresh(ListRequestState state)
        {
            int generation;
            lock (state.Gate)
            {
                if (state.Released)
                {
                    return;
                }

                state.Canceled = false;
                state.Refreshing = true;
                state.Generation++;
                generation = state.Generation;
                state.CurrentItems.Clear();
                state.Servers.Clear();
            }

            WorkQueue.Enqueue(
                "Query game servers",
                () =>
                {
                    List<GameServerData> servers;
                    try
                    {
                        servers = LoadServers(state.AppId)
                            .Where(server => MatchesListKind(state.Kind, server))
                            .Where(server => ServerFilter.Matches(state.Filters, server))
                            .OrderBy(server => server.ServerName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                            .ThenBy(server => server.IP)
                            .ThenBy(server => server.QueryPort)
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        SteamEmulator.Write("Server browser", ex);
                        servers = new List<GameServerData>();
                    }

                    CompleteRefresh(state, generation, servers);
                },
                $"server-browser:list:{state.Handle.ToInt64()}:{generation}",
                true);
        }

        private void CompleteRefresh(ListRequestState state, int generation, List<GameServerData> servers)
        {
            var indexes = new List<int>();
            EMatchMakingServerResponse result;
            lock (state.Gate)
            {
                if (state.Released || state.Canceled || state.Generation != generation ||
                    !_listRequests.ContainsKey(state.Handle))
                {
                    return;
                }

                foreach (var server in servers)
                {
                    var native = NativeGameServerItem.Allocate(server, 1);
                    state.AllocatedItems.Add(native);
                    state.CurrentItems.Add(native);
                    state.Servers.Add(server);
                    indexes.Add(state.CurrentItems.Count - 1);
                }

                state.Refreshing = false;
                result = indexes.Count == 0 && state.Kind == ServerListKind.Internet
                    ? EMatchMakingServerResponse.eNoServersListedOnMasterServer
                    : EMatchMakingServerResponse.eServerResponded;
            }

            foreach (var index in indexes)
            {
                NativeCallbackQueue.Enqueue(() =>
                {
                    if (IsListCallbackValid(state, generation))
                    {
                        NativeMatchmakingCallbacks.ServerResponded(state.Response, state.Handle, index);
                    }
                });
            }

            NativeCallbackQueue.Enqueue(() =>
            {
                if (IsListCallbackValid(state, generation))
                {
                    NativeMatchmakingCallbacks.RefreshComplete(state.Response, state.Handle, result);
                }
            });
        }

        private void CompleteServerRefresh(
            ListRequestState state,
            int generation,
            int serverIndex,
            GameServerData previous)
        {
            var updated = LoadServers(state.AppId).FirstOrDefault(server => SameServer(server, previous));
            var responded = false;
            lock (state.Gate)
            {
                if (state.Released || state.Canceled || state.Generation != generation ||
                    serverIndex < 0 || serverIndex >= state.CurrentItems.Count)
                {
                    return;
                }

                if (updated != null)
                {
                    var native = NativeGameServerItem.Allocate(updated, 1);
                    state.AllocatedItems.Add(native);
                    state.CurrentItems[serverIndex] = native;
                    state.Servers[serverIndex] = updated;
                    responded = true;
                }
            }

            NativeCallbackQueue.Enqueue(() =>
            {
                if (!IsListCallbackValid(state, generation))
                {
                    return;
                }

                if (responded)
                {
                    NativeMatchmakingCallbacks.ServerResponded(state.Response, state.Handle, serverIndex);
                }
                else
                {
                    NativeMatchmakingCallbacks.ServerFailedToRespond(state.Response, state.Handle, serverIndex);
                }
            });
        }

        private int BeginServerQuery(
            ServerQueryKind kind,
            uint ip,
            ushort port,
            IntPtr response,
            Action<ServerQueryState, GameServerData> deliver)
        {
            if (response == IntPtr.Zero)
            {
                return -1;
            }

            var handle = Interlocked.Increment(ref _nextServerQuery);
            if (handle <= 0)
            {
                Interlocked.Exchange(ref _nextServerQuery, 1);
                handle = 1;
            }

            var query = new ServerQueryState
            {
                Handle = handle,
                Kind = kind,
                IP = ip,
                Port = port,
                Response = response
            };

            if (!_serverQueries.TryAdd(handle, query))
            {
                return -1;
            }

            WorkQueue.Enqueue(
                "Query individual game server",
                () =>
                {
                    GameServerData server = null;
                    try
                    {
                        server = LoadServers(0).FirstOrDefault(candidate =>
                            candidate.IP == query.IP &&
                            (candidate.QueryPort == query.Port || candidate.Port == query.Port));
                    }
                    catch (Exception ex)
                    {
                        SteamEmulator.Write("Server query", ex);
                    }

                    NativeCallbackQueue.Enqueue(() =>
                    {
                        if (!_serverQueries.TryGetValue(query.Handle, out var active) ||
                            !ReferenceEquals(active, query) ||
                            query.Canceled)
                        {
                            return;
                        }

                        try
                        {
                            deliver(query, server);
                        }
                        finally
                        {
                            _serverQueries.TryRemove(query.Handle, out _);
                        }
                    });
                },
                $"server-browser:query:{handle}",
                true);

            return handle;
        }

        private List<GameServerData> LoadServers(uint appId)
        {
            var servers = APIClient.IsEnabled
                ? APIClient.ListGameServers(appId)
                : new List<GameServerData>();

            var local = SteamEmulator.SteamGameServer?.ServerData;
            if (local != null &&
                local.LoggedOn &&
                local.AdvertiseActive &&
                (appId == 0 || local.AppId == appId) &&
                !servers.Any(server => SameServer(server, local)))
            {
                servers.Add(local);
            }

            return servers
                .Where(server => server != null && server.LoggedOn && server.AdvertiseActive)
                .GroupBy(ServerIdentity)
                .Select(group => group.First())
                .ToList();
        }

        private bool IsListCallbackValid(ListRequestState state, int generation)
        {
            return _listRequests.TryGetValue(state.Handle, out var active) &&
                ReferenceEquals(active, state) &&
                !state.Released &&
                !state.Canceled &&
                state.Generation == generation;
        }

        private static bool MatchesListKind(ServerListKind kind, GameServerData server)
        {
            return kind != ServerListKind.Spectator || server.SpectatorPort != 0;
        }

        private static bool SameServer(GameServerData left, GameServerData right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            return left.SteamId != 0 && right.SteamId != 0
                ? left.SteamId == right.SteamId
                : left.IP == right.IP &&
                  left.QueryPort == right.QueryPort &&
                  left.Port == right.Port;
        }

        private static string ServerIdentity(GameServerData server)
        {
            return server.SteamId != 0
                ? "steam:" + server.SteamId
                : $"address:{server.IP}:{server.QueryPort}:{server.Port}";
        }

        private static Dictionary<string, string> BuildRules(GameServerData server)
        {
            var rules = server.KeyValues == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(server.KeyValues, StringComparer.OrdinalIgnoreCase);

            AddRule(rules, "appid", server.AppId.ToString());
            AddRule(rules, "gamedir", server.ModDir);
            AddRule(rules, "gamename", server.Description);
            AddRule(rules, "map", server.MapName);
            AddRule(rules, "maxplayers", server.MaxPlayers.ToString());
            AddRule(rules, "players", ((server.Players?.Count ?? 0) + server.BotPlayers).ToString());
            AddRule(rules, "version", server.VersionString);
            return rules;
        }

        private static void AddRule(Dictionary<string, string> rules, string key, string value)
        {
            if (!rules.ContainsKey(key) && !string.IsNullOrWhiteSpace(value))
            {
                rules[key] = value;
            }
        }

        private static List<ServerFilterTerm> ReadFilters(IntPtr filters, uint filterCount)
        {
            var result = new List<ServerFilterTerm>();
            if (filters == IntPtr.Zero || filterCount == 0 || filterCount > MaxFilterCount)
            {
                return result;
            }

            var firstFilter = Marshal.ReadIntPtr(filters);
            if (firstFilter == IntPtr.Zero)
            {
                return result;
            }

            for (var index = 0; index < filterCount; index++)
            {
                var filter = IntPtr.Add(firstFilter, checked((int)index * MatchmakingFilterSize));
                result.Add(new ServerFilterTerm
                {
                    Key = ReadFixedUtf8(filter, MatchmakingFilterFieldSize),
                    Value = ReadFixedUtf8(
                        IntPtr.Add(filter, MatchmakingFilterFieldSize),
                        MatchmakingFilterFieldSize)
                });
            }

            return result;
        }

        private static string ReadFixedUtf8(IntPtr pointer, int capacity)
        {
            var bytes = new byte[capacity];
            Marshal.Copy(pointer, bytes, 0, bytes.Length);
            var length = Array.IndexOf(bytes, (byte)0);
            if (length < 0)
            {
                length = bytes.Length;
            }

            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        private static string FormatIp(uint ip)
        {
            return $"{(byte)(ip >> 24)}.{(byte)(ip >> 16)}.{(byte)(ip >> 8)}.{(byte)ip}";
        }

        private sealed class ListRequestState
        {
            public object Gate { get; } = new object();
            public IntPtr Handle { get; set; }
            public ServerListKind Kind { get; set; }
            public uint AppId { get; set; }
            public IntPtr Response { get; set; }
            public List<ServerFilterTerm> Filters { get; set; } = new List<ServerFilterTerm>();
            public List<GameServerData> Servers { get; } = new List<GameServerData>();
            public List<IntPtr> CurrentItems { get; } = new List<IntPtr>();
            public List<IntPtr> AllocatedItems { get; } = new List<IntPtr>();
            public int Generation { get; set; }
            public bool Refreshing { get; set; }
            public bool Canceled { get; set; }
            public bool Released { get; set; }
        }

        private sealed class ServerQueryState
        {
            public int Handle { get; set; }
            public ServerQueryKind Kind { get; set; }
            public uint IP { get; set; }
            public ushort Port { get; set; }
            public IntPtr Response { get; set; }
            public volatile bool Canceled;
        }

        private enum ServerListKind
        {
            Internet,
            Lan,
            Friends,
            Favorites,
            History,
            Spectator
        }

        private enum ServerQueryKind
        {
            Ping,
            Players,
            Rules
        }
    }

    internal sealed class ServerFilterTerm
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal static class ServerFilter
    {
        public static bool Matches(IReadOnlyList<ServerFilterTerm> filters, GameServerData server)
        {
            if (filters == null || filters.Count == 0)
            {
                return true;
            }

            var position = 0;
            while (position < filters.Count)
            {
                if (!Evaluate(filters, server, ref position, filters.Count))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Evaluate(
            IReadOnlyList<ServerFilterTerm> filters,
            GameServerData server,
            ref int position,
            int limit)
        {
            if (position >= limit)
            {
                return false;
            }

            var term = filters[position++];
            var key = (term.Key ?? string.Empty).Trim().ToLowerInvariant();
            if (!IsLogical(key))
            {
                return EvaluateAtomic(key, term.Value, server);
            }

            if (!int.TryParse(term.Value, out var length) || length < 0)
            {
                return false;
            }

            var expressionEnd = Math.Min(limit, position + length);
            var values = new List<bool>();
            while (position < expressionEnd)
            {
                values.Add(Evaluate(filters, server, ref position, expressionEnd));
            }

            if (values.Count == 0)
            {
                return false;
            }

            return key switch
            {
                "and" => values.All(value => value),
                "or" => values.Any(value => value),
                "nand" => !values.All(value => value),
                "nor" => !values.Any(value => value),
                _ => false
            };
        }

        private static bool EvaluateAtomic(string key, string value, GameServerData server)
        {
            value ??= string.Empty;
            var players = (server.Players?.Count ?? 0) + Math.Max(0, server.BotPlayers);
            switch (key)
            {
                case "map":
                    return EqualsIgnoreCase(server.MapName, value);
                case "gamedir":
                    return EqualsIgnoreCase(server.ModDir, value);
                case "gametype":
                    return EqualsIgnoreCase(server.Product, value) ||
                           EqualsIgnoreCase(server.Description, value);
                case "gamedataand":
                    return ContainsAll(server.GameData, value);
                case "gamedataor":
                    return ContainsAny(server.GameData, value);
                case "gamedatanor":
                    return !ContainsAny(server.GameData, value);
                case "gametagsand":
                    return ContainsAll(server.GameTags, value);
                case "gametagsnor":
                    return !ContainsAny(server.GameTags, value);
                case "addr":
                    return MatchesAddress(server.IP, server.QueryPort, value);
                case "gameaddr":
                    return MatchesAddress(server.IP, server.Port, value);
                case "dedicated":
                    return server.Dedicated;
                case "secure":
                    return server.Secure != 0;
                case "notfull":
                    return server.MaxPlayers > 0 && players < server.MaxPlayers;
                case "hasplayers":
                    return players > 0;
                case "noplayers":
                    return players == 0;
                case "linux":
                    return server.KeyValues != null &&
                           server.KeyValues.TryGetValue("os", out var os) &&
                           (EqualsIgnoreCase(os, "l") || EqualsIgnoreCase(os, "linux"));
                default:
                    return server.KeyValues != null &&
                           server.KeyValues.TryGetValue(key, out var actual) &&
                           EqualsIgnoreCase(actual, value);
            }
        }

        private static bool IsLogical(string key)
        {
            return key == "and" || key == "or" || key == "nand" || key == "nor";
        }

        private static bool ContainsAll(string source, string values)
        {
            var operands = SplitValues(values);
            return operands.Count > 0 &&
                   operands.All(value => ContainsIgnoreCase(source, value));
        }

        private static bool ContainsAny(string source, string values)
        {
            var operands = SplitValues(values);
            return operands.Any(value => ContainsIgnoreCase(source, value));
        }

        private static List<string> SplitValues(string values)
        {
            return (values ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => value.Length != 0)
                .ToList();
        }

        private static bool MatchesAddress(uint ip, int port, string expected)
        {
            var address = $"{(byte)(ip >> 24)}.{(byte)(ip >> 16)}.{(byte)(ip >> 8)}.{(byte)ip}";
            var normalized = (expected ?? string.Empty).Trim();
            return EqualsIgnoreCase(normalized, address) ||
                   EqualsIgnoreCase(normalized, $"{address}:{port}");
        }

        private static bool ContainsIgnoreCase(string source, string value)
        {
            return (source ?? string.Empty).IndexOf(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EqualsIgnoreCase(string left, string right)
        {
            return string.Equals(
                (left ?? string.Empty).Trim(),
                (right ?? string.Empty).Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
