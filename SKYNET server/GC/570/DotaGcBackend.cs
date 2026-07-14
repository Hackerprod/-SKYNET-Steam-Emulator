using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.GC.Dota2;

public sealed partial class DotaGcBackend : ILuaGameCoordinatorBackend
{
    private static readonly object PracticeLobbySync = new();
    private static readonly Dictionary<ulong, PracticeLobbyState> PracticeLobbies = new();
    private static readonly Dictionary<ulong, ulong> PracticeLobbyBySteamId = new();
    private static readonly Dictionary<ulong, ulong> PracticeLobbyByServerSteamId = new();
    private static readonly Dictionary<ulong, Queue<ApiGCMessage>> PendingGcMessages = new();
    private static readonly object ObservedFixtureSync = new();
    private static readonly Dictionary<string, int> ObservedFixtureIndexes = new(StringComparer.Ordinal);
    private static long SteamObjectCounter = Environment.TickCount64 & 0xFFFFFF;
    private static ulong MatchIdCounter = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 5UL;
    private static readonly TimeSpan PracticeLobbyTimeout = TimeSpan.FromSeconds(45);

    public static Action<ulong, ApiGCMessage>? PendingMessageQueued { get; set; }
    public static Func<ulong, ApiDotaRuntimeInventory>? InventoryProvider { get; set; }
    public static Func<ulong, ulong, uint, uint, uint, List<ApiDotaEquipment>>? EquipItemSink { get; set; }
    public static Func<ulong, ulong, uint, List<ApiDotaEquipment>>? SetItemStyleSink { get; set; }
    public static Action<ApiDotaMatch>? MatchSnapshotSink { get; set; }
    public static Func<ulong, string>? MatchSnapshotJsonProvider { get; set; }
    public static Func<ulong, string>? MatchSnapshotByLobbyJsonProvider { get; set; }
    public static Func<ulong, bool>? MatchSnapshotDeleteSink { get; set; }
    public static Func<ulong, bool>? UserExistsProvider { get; set; }
    public static Func<ulong, bool>? UserOnlineProvider { get; set; }
    public static Func<string, string, string, string, string>? GameServerConnectIpResolver { get; set; }
    public static Func<string, string, string, string, string>? GameServerConnectIpsResolver { get; set; }
    public static Func<ulong, string, DotaDedicatedServerSupervisor.DedicatedLaunchResult>? DedicatedServerStart { get; set; }
    public static Func<ulong, uint, string>? DedicatedServerClaim { get; set; }
    public static Func<uint, bool>? DedicatedServerPortReserved { get; set; }
    public static Func<ulong, string>? DedicatedServerStatus { get; set; }
    public static Func<ulong, string, bool>? DedicatedServerRelease { get; set; }
    public static DotaStatsStore? StatsStore { get; set; }
    public static DotaPartyStore? PartyStore { get; set; }
    public static DotaLobbyInviteStore? LobbyInviteStore { get; set; }

    // Dumb-storage primitives for the Lua-owned equip logic: resolve item ids,
    // read/replace the persisted equipment list and query the catalog. Rules
    // (slot matching, replacement, unequip) live in GC/570/dota_items.lua.
    public static Func<ulong, ulong, uint>? ItemDefResolver { get; set; }
    public static Func<ulong, string>? EquipmentJsonProvider { get; set; }
    public static Func<ulong, string, bool>? EquipmentJsonSink { get; set; }
    public static Func<uint, string>? CatalogItemJsonProvider { get; set; }

    private const uint EMsgGCClientWelcome = 4004;
    private const uint EMsgGCGameServerWelcome = 4005;
    private const uint EMsgGCClientHello = 4006;
    private const uint EMsgGCGameServerHello = 4007;
    private const uint EMsgGCClientConnectionStatus = 4009;
    private const uint EMsgSOSingleObject = 21;
    private const uint EMsgSOSingleObjectDestroyed = 23;
    private const uint EMsgSOCacheSubscribed = 24;
    private const uint EMsgSOCacheUnsubscribed = 25;
    private const uint EMsgSOMultipleObjects = 26;
    private const uint EMsgSOCacheSubscribedUpToDate = 29;
    private const uint EMsgGCRequestStoreSalesData = 2536;
    private const uint EMsgGCRequestStoreSalesDataResponse = 2537;
    private const uint EMsgClientToGCEquipItems = 2569;
    private const uint EMsgClientToGCEquipItemsResponse = 2570;
    private const uint EMsgClientToGCSetItemStyle = 2577;
    private const uint EMsgClientToGCSetItemStyleResponse = 2578;
    private const uint EMsgGCGenericResult = 2579;
    private const uint EMsgClientToGCCancelUnfinalizedTransactions = 2617;
    private const uint EMsgClientToGCAggregateMetrics = 4523;
    private const uint EMsgGCServerAvailable = 4506;
    private const uint EMsgGCGameServerInfo = 4508;
    private const uint EMsgGCLANServerAvailable = 4511;
    private const uint EMsgGCPracticeLobbyResponse = 7055;
    private const uint EMsgGCPracticeLobbyJoinResponse = 7113;
    private const uint EMsgGCToServerRealtimeStatsStartStop = 8042;
    private const uint EMsgServerToGCRequestStatusResponse = 7546;
    private const uint EMsgServerToGCRequestPlayerRecentAccomplishmentsResponse = 8331;
    private const uint EMsgGCToServerRequestBatchPlayerResourcesResponse = 7451;
    private const uint EMsgGameMatchSignOutPermissionResponse = 7382;
    private const uint EMsgGameMatchSignOutResponse = 7005;
    private const uint EMsgDOTASetMatchHistoryAccessResponse = 7201;
    private const uint EMsgLeaverDetectedResponse = 7087;
    private const uint EMsgGCToClientMatchSignedOut = 8081;
    private const uint EventIdPlusSubscription = 19;
    private const uint EventIdInitialProfile = 29;
    private const uint EMsgGCMatchmakingStatsRequest = 7197;
    private const uint EMsgGCMatchmakingStatsResponse = 7198;
    private const uint EMsgDOTAGetEventPoints = 7387;
    private const uint EMsgDOTAGetEventPointsResponse = 7388;
    private const uint EMsgGCNotificationsRequest = 7427;
    private const uint EMsgGCNotificationsResponse = 7428;
    private const uint EMsgClientToGCGetProfileCard = 7534;
    private const uint EMsgClientToGCGetProfileCardResponse = 7535;
    private const uint EMsgGCToClientProfileCardUpdated = 7539;
    private const uint EMsgClientToGCGetProfileCardStats = 8034;
    private const uint EMsgClientToGCGetProfileCardStatsResponse = 8035;
    private const uint EMsgClientToGCGetProfileTickets = 8073;
    private const uint EMsgClientToGCGetProfileTicketsResponse = 8074;
    private const uint EMsgClientToGCGetQuestProgress = 8078;
    private const uint EMsgClientToGCGetQuestProgressResponse = 8079;
    private const uint EMsgClientToGCFindTopSourceTvGames = 8009;
    private const uint EMsgGCToClientFindTopSourceTvGamesResponse = 8010;
    private const uint EMsgGCGetHeroStandings = 7274;
    private const uint EMsgGCGetHeroStandingsResponse = 7275;
    private const uint EMsgGCGetHeroStatsHistory = 8082;
    private const uint EMsgGCGetHeroStatsHistoryResponse = 8083;
    private const uint EMsgProfileRequest = 8268;
    private const uint EMsgProfileResponse = 8269;
    private const uint EMsgClientToGCRequestGuildData = 8673;
    private const uint EMsgClientToGCRequestGuildDataResponse = 8674;
    private const uint EMsgClientToGCRequestGuildMembership = 8676;
    private const uint EMsgClientToGCRequestGuildMembershipResponse = 8677;
    private const uint EMsgClientToGCUnknown8716 = 8716;
    private const uint EMsgClientToGCRequestAccountGuildPersonaInfo = 8727;
    private const uint EMsgClientToGCRequestAccountGuildPersonaInfoResponse = 8728;
    private const uint EMsgClientToGCRequestAccountGuildPersonaInfoBatch = 8729;
    private const uint EMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse = 8730;
    private const uint EMsgClientToGCGetCurrentPrivateCoachingSession = 8793;
    private const uint EMsgClientToGCGetCurrentPrivateCoachingSessionResponse = 8794;
    private const uint EMsgClientToGCGetAvailablePrivateCoachingSessions = 8798;
    private const uint EMsgClientToGCGetAvailablePrivateCoachingSessionsResponse = 8799;
    private const uint EMsgClientToGCGetAvailablePrivateCoachingSessionsSummary = 8800;
    private const uint EMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse = 8801;
    private const uint EMsgClientToGCRankRequest = 8879;
    private const uint EMsgGCToClientRankResponse = 8880;
    private const uint EMsgClientToGCShowcaseGetUserData = 8886;
    private const uint EMsgClientToGCShowcaseGetUserDataResponse = 8887;
    private const uint EMsgClientToGCOverworldGetUserData = 8944;
    private const uint EMsgClientToGCOverworldGetUserDataResponse = 8945;
    private const uint EMsgClientToGCMonsterHunterGetUserData = 9023;
    private const uint EMsgClientToGCMonsterHunterGetUserDataResponse = 9024;
    private const ulong InvalidJobId = ulong.MaxValue;

    private const uint ClientWelcomeVersion = 20;
    private const uint WelcomeVersion = 20;
    private const uint ActiveEventId = 57;
    private const int LobbyObjectTypeId = 2004;
    private const int LobbyInviteObjectTypeId = 2013;
    private const int LobbyPersonaObjectTypeId = 2014;
    private const int LobbyBroadcastObjectTypeId = 2015;
    private const int LobbyMemberObjectTypeId = 2016;
    private const uint TeamGoodGuys = 0;
    private const uint TeamBadGuys = 1;
    private const uint TeamSpectator = 3;
    private const uint TeamPlayerPool = 4;
    private const uint TeamNoTeam = 5;
    private const uint LobbyStateUi = 0;
    private const uint LobbyStateServerSetup = 1;
    private const uint LobbyStateRun = 2;
    private const uint GameStateInit = 0;
    private const uint GameStateHeroSelection = 2;
    private const uint ConnectedPlayersReasonGameState = 2;
    private const uint ConnectedPlayersReasonPlayerHero = 5;
    private const uint LobbyTypePractice = 1;
    private const uint LeaverNone = 0;
    private const uint LeaverDisconnected = 1;
    private const uint LobbyDotaTv120 = 1;
    private const uint JoinResultSuccess = 0;
    private const uint JoinResultAlreadyInGame = 1;
    private const uint JoinResultInvalidLobby = 2;
    private const uint JoinResultIncorrectPassword = 3;
    private const uint JoinResultLobbyFull = 9;
    private const int MaxPracticeLobbyMembers = 20;

    private readonly GameCoordinatorContext _context;
    private readonly ApiGCExchangeRequest _request;
    private readonly byte[] _requestBody;
    private readonly ulong _sourceJobId;
    private readonly List<ApiDotaEquipment> _dotaItemChanges = new();
    private ApiDotaRuntimeInventory? _dotaInventoryAfterChanges;

    public DotaGcBackend(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        _context = context;
        _request = request;
        _requestBody = Decode(request.BodyBase64);
        _sourceJobId = request.SourceJobId ?? InvalidJobId;
        Response = new ApiGCExchangeResponse { Handled = true };
    }

    public uint MessageType => _request.MessageType;
    public uint AppId => _context.AppId;
    public uint AccountId => _context.AccountId;
    public string AccountIdString => _context.AccountId.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public ulong SteamId => _context.SteamId;
    public string SteamIdString => _context.SteamId.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string PersonaName => _context.PersonaName;
    public string ClientIp => _context.ClientIp ?? string.Empty;
    public ulong SourceJobId => _sourceJobId;
    public string BodyBase64 => Encode(_requestBody);
    public string BodyHex => Convert.ToHexString(_requestBody);
    public ApiGCExchangeResponse Response { get; }

    public string DotaResolveGameServerConnectIp(string publicIpValue, string privateIpValue, string fallbackIp)
    {
        return GameServerConnectIpResolver?.Invoke(ClientIp, publicIpValue, privateIpValue, fallbackIp)
            ?? (string.IsNullOrWhiteSpace(fallbackIp) ? "127.0.0.1" : fallbackIp);
    }

    // Space-separated list (up to two) of host addresses reachable across the
    // different interfaces the dedicated server binds (0.0.0.0). The Dota connect
    // field carries two endpoints, so WiFi + ZeroTier peers each get a routable one.
    public string DotaGetHostConnectIps(string publicIpValue, string privateIpValue, string fallbackIp)
    {
        return GameServerConnectIpsResolver?.Invoke(ClientIp, publicIpValue, privateIpValue, fallbackIp)
            ?? DotaResolveGameServerConnectIp(publicIpValue, privateIpValue, fallbackIp);
    }

    public string DotaStartDedicatedServer(string lobbyId, string map)
    {
        var result = DedicatedServerStart?.Invoke(ParseUInt64(lobbyId), map);
        if (result == null || !result.Started)
        {
            Console.WriteLine($"[SKYNET][dedicated] start failed lobby={lobbyId}: {result?.Error ?? "supervisor unavailable"}");
            return "0";
        }

        return result.Port.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public string DotaClaimDedicatedServer(string gameServerSteamId, uint reportedPort)
    {
        return DedicatedServerClaim?.Invoke(ParseUInt64(gameServerSteamId), reportedPort) ?? "0";
    }

    public bool DotaDedicatedServerPortReserved(uint reportedPort)
    {
        return DedicatedServerPortReserved?.Invoke(reportedPort) ?? false;
    }

    public string DotaDedicatedServerStatus(string lobbyId)
    {
        return DedicatedServerStatus?.Invoke(ParseUInt64(lobbyId)) ?? "not_available";
    }

    public bool DotaReleaseDedicatedServer(string lobbyId, string reason)
    {
        return DedicatedServerRelease?.Invoke(ParseUInt64(lobbyId), reason ?? string.Empty) ?? false;
    }

    public static ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        var response = new ApiGCExchangeResponse { Handled = true };
        lock (PracticeLobbySync)
        {
            ExpireInactiveSessionsLocked(null);
            if (!PendingGcMessages.TryGetValue(context.SteamId, out var queue))
            {
                return response;
            }

            while (queue.Count > 0)
            {
                response.Messages.Add(queue.Dequeue());
            }

            PendingGcMessages.Remove(context.SteamId);
        }

        return response;
    }

    public static void ExpireInactiveSessions(IReadOnlySet<ulong> activeSteamIds)
    {
        lock (PracticeLobbySync)
        {
            ExpireInactiveSessionsLocked(activeSteamIds);
        }
    }

    public int NextObservedIndex(string bucket, int count)
    {
        if (count <= 1)
        {
            return 1;
        }

        string key = $"{SteamId}:{bucket ?? string.Empty}";
        lock (ObservedFixtureSync)
        {
            ObservedFixtureIndexes.TryGetValue(key, out int current);
            int next = (current % count) + 1;
            ObservedFixtureIndexes[key] = next;
            return next;
        }
    }

    public bool PracticeLobbyCreate()
    {
        lock (PracticeLobbySync)
        {
            LeaveCurrentLobbyLocked();
            var lobby = CreatePracticeLobbyLocked();
            ApplyPracticeLobbyCreateRequest(lobby, _requestBody);
            var member = GetOrAddLobbyMemberLocked(lobby, SteamId, AccountId, PersonaName);
            member.Team = TeamGoodGuys;
            member.Slot = 1;
            member.CoachTeam = TeamNoTeam;
            lobby.State = LobbyStateUi;
            RefreshLobbyVersion(lobby);

            AddProto(EMsgSOCacheSubscribed, BuildLobbySoCacheSubscribed(lobby));
            AddProto(EMsgSOSingleObject, BuildLobbySingleObject(lobby));
            PublishLobbySnapshotLocked(lobby);
        }

        return Reply(EMsgGCPracticeLobbyResponse, BuildResult(1));
    }

    public bool PracticeLobbyJoin()
    {
        uint result = JoinResultInvalidLobby;
        lock (PracticeLobbySync)
        {
            TryReadVarintField(_requestBody, 1, out ulong lobbyId);
            TryReadStringField(_requestBody, 3, out string passKey);

            if (!PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                result = JoinResultInvalidLobby;
            }
            else if (lobby.State != LobbyStateUi)
            {
                result = JoinResultAlreadyInGame;
            }
            else if (!string.IsNullOrEmpty(lobby.PassKey) && !string.Equals(lobby.PassKey, passKey, StringComparison.Ordinal))
            {
                result = JoinResultIncorrectPassword;
            }
            else if (lobby.Members.Count >= MaxPracticeLobbyMembers && lobby.Members.All(member => member.SteamId != SteamId))
            {
                result = JoinResultLobbyFull;
            }
            else
            {
                LeaveCurrentLobbyLocked(lobby.LobbyId);
                var member = GetOrAddLobbyMemberLocked(lobby, SteamId, AccountId, PersonaName);
                if (member.SteamId != lobby.LeaderSteamId)
                {
                    member.Team = TeamPlayerPool;
                    member.Slot = 0;
                    member.CoachTeam = TeamNoTeam;
                }

                RefreshLobbyVersion(lobby);
                AddProto(EMsgSOCacheSubscribed, BuildLobbySoCacheSubscribed(lobby));
                AddProto(EMsgSOSingleObject, BuildLobbySingleObject(lobby));
                BroadcastLobbyUpdateLocked(lobby, SteamId);
                PublishLobbySnapshotLocked(lobby);
                result = JoinResultSuccess;
            }
        }

        return Reply(EMsgGCPracticeLobbyJoinResponse, BuildJoinResponse(result));
    }

    public bool PracticeLobbySetDetails()
    {
        lock (PracticeLobbySync)
        {
            if (!TryGetCurrentLobbyLocked(out var lobby, out _))
            {
                return Reply(EMsgGCPracticeLobbyResponse, BuildResult(0));
            }

            ApplyPracticeLobbyDetails(lobby, _requestBody);
            RefreshLobbyVersion(lobby);
            BroadcastLobbyUpdateLocked(lobby);
            PublishLobbySnapshotLocked(lobby);
        }

        return Reply(EMsgGCPracticeLobbyResponse, BuildResult(1));
    }

    public bool PracticeLobbyLeave()
    {
        lock (PracticeLobbySync)
        {
            LeaveCurrentLobbyLocked();
        }

        return true;
    }

    public bool PracticeLobbySetTeamSlot()
    {
        lock (PracticeLobbySync)
        {
            if (!TryGetCurrentLobbyLocked(out var lobby, out var member))
            {
                return Reply(EMsgGCPracticeLobbyResponse, BuildResult(0));
            }

            uint nextTeam = member.Team;
            uint nextSlot = member.Slot;
            if (TryReadVarintField(_requestBody, 1, out ulong team))
            {
                nextTeam = (uint)team;
            }

            if (TryReadVarintField(_requestBody, 2, out ulong slot))
            {
                nextSlot = (uint)slot;
            }

            if (nextTeam != TeamPlayerPool && lobby.Members.Any(candidate => candidate.SteamId != SteamId && candidate.Team == nextTeam && candidate.Slot == nextSlot))
            {
                return Reply(EMsgGCPracticeLobbyResponse, BuildResult(0));
            }

            member.Team = nextTeam;
            member.Slot = nextTeam == TeamPlayerPool ? 0 : nextSlot;
            member.CoachTeam = TeamNoTeam;
            RefreshLobbyVersion(lobby);
            BroadcastLobbyUpdateLocked(lobby);
            PublishLobbySnapshotLocked(lobby);
        }

        return Reply(EMsgGCPracticeLobbyResponse, BuildResult(1));
    }

    public bool PracticeLobbySetCoach()
    {
        lock (PracticeLobbySync)
        {
            if (!TryGetCurrentLobbyLocked(out var lobby, out var member))
            {
                return Reply(EMsgGCPracticeLobbyResponse, BuildResult(0));
            }

            uint coachTeam = TeamNoTeam;
            if (TryReadVarintField(_requestBody, 1, out ulong team))
            {
                coachTeam = (uint)team;
            }

            if (coachTeam == TeamNoTeam)
            {
                member.CoachTeam = TeamNoTeam;
                member.Team = TeamPlayerPool;
                member.Slot = 0;
            }
            else
            {
                member.CoachTeam = coachTeam;
                member.Team = TeamSpectator;
                member.Slot = 1;
            }

            RefreshLobbyVersion(lobby);
            BroadcastLobbyUpdateLocked(lobby);
            PublishLobbySnapshotLocked(lobby);
        }

        return Reply(EMsgGCPracticeLobbyResponse, BuildResult(1));
    }

    public bool PracticeLobbyApplyTeam()
    {
        lock (PracticeLobbySync)
        {
            if (TryGetCurrentLobbyLocked(out var lobby, out _))
            {
                RefreshLobbyVersion(lobby);
                BroadcastLobbyUpdateLocked(lobby, 0, true);
                PublishLobbySnapshotLocked(lobby);
            }
        }

        return Reply(EMsgGCGenericResult, BuildResult(1));
    }

    public bool PracticeLobbyLaunch()
    {
        lock (PracticeLobbySync)
        {
            if (TryGetCurrentLobbyLocked(out var lobby, out _))
            {
                lobby.State = LobbyStateServerSetup;
                lobby.Connect = string.Empty;
                lobby.GameStartTime = 0;
                lobby.GameState = GameStateInit;
                lobby.RealtimeStatsStartStopSent = false;
                RefreshLobbyVersion(lobby);
                BroadcastLobbyUpdateLocked(lobby);
                PublishLobbySnapshotLocked(lobby);
            }
        }

        return true;
    }

    public bool EquipItems()
    {
        var changedItems = new List<ApiDotaEquipment>();
        foreach (var equipBody in ReadLengthDelimitedFields(_requestBody, 1))
        {
            TryReadVarintField(equipBody, 1, out ulong itemId);
            TryReadVarintField(equipBody, 2, out ulong heroClass);
            TryReadVarintField(equipBody, 3, out ulong slot);
            var style = TryReadVarintField(equipBody, 4, out ulong styleIndex) ? (uint)styleIndex : 0;
            Console.WriteLine($"Dota GC equip request steam={SteamId} item={itemId} hero={heroClass} slot={slot} style={style}");

            if (EquipItemSink != null)
            {
                changedItems.AddRange(EquipItemSink(SteamId, itemId, (uint)heroClass, (uint)slot, style));
            }
        }

        var inventory = InventoryProvider?.Invoke(SteamId);
        if (changedItems.Count > 0 && inventory != null)
        {
            AddProto(EMsgSOMultipleObjects, BuildEconMultipleObjects(SteamId, inventory, changedItems, EconObjectUpdateKind.Modified));

            lock (PracticeLobbySync)
            {
                if (TryGetCurrentLobbyLocked(out var lobby, out _) && lobby.ServerSteamId != 0)
                {
                    QueueEconItemChangesToServer(lobby.ServerSteamId, SteamId, inventory, changedItems);
                    PublishLobbySnapshotLocked(lobby);
                }
            }
        }

        return Reply(EMsgClientToGCEquipItemsResponse, BuildEquipItemsResponse(inventory?.Version ?? WelcomeVersion));
    }

    public bool SetItemStyle()
    {
        TryReadVarintField(_requestBody, 1, out ulong itemId);
        TryReadVarintField(_requestBody, 2, out ulong styleIndex);
        var changed = SetItemStyleSink?.Invoke(SteamId, itemId, (uint)styleIndex) ?? new List<ApiDotaEquipment>();
        var inventory = InventoryProvider?.Invoke(SteamId);
        if (changed.Count > 0 && inventory != null)
        {
            AddProto(EMsgSOMultipleObjects, BuildEconMultipleObjects(SteamId, inventory, changed, EconObjectUpdateKind.Modified));
        }

        return Reply(EMsgClientToGCSetItemStyleResponse, BuildResult(1));
    }

    public bool FindTopSourceTvGames()
    {
        return Reply(EMsgGCToClientFindTopSourceTvGamesResponse, BuildFindTopSourceTvGamesResponse(_requestBody));
    }

    public bool GetHeroStandings()
    {
        return Reply(EMsgGCGetHeroStandingsResponse, Array.Empty<byte>());
    }

    public bool GetHeroStatsHistory()
    {
        return Reply(EMsgGCGetHeroStatsHistoryResponse, BuildHeroStatsHistoryResponse(_requestBody));
    }

    public bool GameServerHello()
    {
        TryReadVarintField(_requestBody, 1, out ulong version);
        Console.WriteLine($"Dota GC server hello steam={SteamId} version={version}");
        AddProto(EMsgGCGameServerWelcome, BuildGameServerWelcome((uint)version));
        return true;
    }

    public bool GameServerInfo()
    {
        lock (PracticeLobbySync)
        {
            Console.WriteLine($"Dota GC server info steam={SteamId} waiting={TryGetWaitingLobbyLocked(out _)} assigned={PracticeLobbyByServerSteamId.ContainsKey(SteamId)}");
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                UpdateLobbyServerPort(lobby);
                BroadcastLobbyUpdateLocked(lobby);
            }
            else if (TryGetWaitingLobbyLocked(out var waitingLobby))
            {
                UpdateLobbyServerPort(waitingLobby);
                AttachServerToLobbyLocked(waitingLobby, false);
            }
        }

        return true;
    }

    public bool LobbyInitialized()
    {
        lock (PracticeLobbySync)
        {
            Console.WriteLine($"Dota GC lobby initialized steam={SteamId} waiting={TryGetWaitingLobbyLocked(out _)} assigned={PracticeLobbyByServerSteamId.ContainsKey(SteamId)}");
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                AttachServerToLobbyLocked(lobby, true);
            }
            else if (TryGetWaitingLobbyLocked(out var waitingLobby))
            {
                AttachServerToLobbyLocked(waitingLobby, true);
            }
        }

        return true;
    }

    public bool LanServerAvailable()
    {
        lock (PracticeLobbySync)
        {
            bool hasLobbyId = TryReadFixed64Field(_requestBody, 1, out ulong lobbyId) || TryReadVarintField(_requestBody, 1, out lobbyId);
            Console.WriteLine($"Dota GC LAN server available steam={SteamId} hasLobby={hasLobbyId} lobby={lobbyId}");
            if (hasLobbyId && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                AttachServerToLobbyLocked(lobby, false);
            }
        }

        return true;
    }

    public bool GameServerAvailable()
    {
        lock (PracticeLobbySync)
        {
            Console.WriteLine($"Dota GC server available steam={SteamId} waiting={TryGetWaitingLobbyLocked(out _)} assigned={PracticeLobbyByServerSteamId.ContainsKey(SteamId)}");
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                AttachServerToLobbyLocked(lobby, true);
            }
            else if (TryGetWaitingLobbyLocked(out var waitingLobby))
            {
                AttachServerToLobbyLocked(waitingLobby, true);
            }
        }

        return true;
    }

    public bool RequestBatchPlayerResources()
    {
        return Reply(EMsgGCToServerRequestBatchPlayerResourcesResponse, BuildBatchPlayerResourcesResponse(_requestBody));
    }

    public bool ConnectedPlayers()
    {
        lock (PracticeLobbySync)
        {
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                TryReadVarintField(_requestBody, 8, out ulong sendReason);
                TryReadVarintField(_requestBody, 2, out ulong loggedGameState);
                Console.WriteLine($"Dota GC connected players steam={SteamId} lobby={lobbyId} reason={sendReason} gameState={loggedGameState} members={lobby.Members.Count}");
                if (TryReadVarintField(_requestBody, 2, out ulong gameState) &&
                    (sendReason == ConnectedPlayersReasonGameState || gameState != GameStateInit))
                {
                    lobby.GameState = (uint)gameState;
                }
                else if (lobby.State == LobbyStateRun && lobby.GameState == GameStateInit)
                {
                    lobby.GameState = GameStateHeroSelection;
                }

                if (sendReason == ConnectedPlayersReasonGameState || sendReason == ConnectedPlayersReasonPlayerHero)
                {
                    ApplyConnectedPlayerUpdates(lobby, _requestBody);
                }

                if (lobby.GameState == GameStateHeroSelection)
                {
                    StartRealtimeStatsLocked(lobby);
                }

                RefreshLobbyVersion(lobby);
                BroadcastLobbyUpdateLocked(lobby);
                PublishLobbySnapshotLocked(lobby);
            }
        }

        return true;
    }

    public bool PlayerFailedToConnect()
    {
        lock (PracticeLobbySync)
        {
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.State = LobbyStateUi;
                RefreshLobbyVersion(lobby);
                BroadcastLobbyUpdateLocked(lobby);
                PublishLobbySnapshotLocked(lobby);
            }
        }

        return true;
    }

    public bool GameMatchSignOutPermission()
    {
        return Reply(EMsgGameMatchSignOutPermissionResponse, BuildBoolField(1, true));
    }

    public bool GameMatchSignOut()
    {
        ulong matchId = 0;
        var recipients = new List<ulong>();
        lock (PracticeLobbySync)
        {
            if (PracticeLobbyByServerSteamId.TryGetValue(SteamId, out ulong lobbyId) && PracticeLobbies.TryGetValue(lobbyId, out var lobby))
            {
                matchId = EnsureLobbyMatchIdLocked(lobby);
                recipients = lobby.Members.Select(member => member.SteamId).ToList();
                RefreshLobbyVersion(lobby);
                BroadcastLobbyUpdateLocked(lobby);
                PublishLobbySnapshotLocked(lobby);
            }
        }

        Reply(EMsgGameMatchSignOutResponse, BuildGameMatchSignOutResponse(matchId));
        if (matchId != 0)
        {
            foreach (ulong recipient in recipients)
            {
                QueueProtoLocked(recipient, EMsgGCToClientMatchSignedOut, BuildMatchSignedOut(matchId));
            }
        }

        return true;
    }

    public bool SetMatchHistoryAccess()
    {
        return Reply(EMsgDOTASetMatchHistoryAccessResponse, BuildResult(1));
    }

    public bool ServerRequestStatus()
    {
        return Reply(EMsgServerToGCRequestStatusResponse, BuildResult(0));
    }

    public bool RequestPlayerRecentAccomplishments()
    {
        TryReadVarintField(_requestBody, 1, out ulong accountId);
        TryReadVarintField(_requestBody, 2, out ulong heroId);
        Console.WriteLine($"Dota GC player recent accomplishments steam={SteamId} account={accountId} hero={heroId}");
        return Reply(EMsgServerToGCRequestPlayerRecentAccomplishmentsResponse, BuildRecentAccomplishmentsResponse((uint)accountId, (int)heroId));
    }

    public bool LeaverDetected()
    {
        return Reply(EMsgLeaverDetectedResponse, BuildResult(1));
    }

    public bool ServerRealtimeStats()
    {
        return true;
    }

    public bool ServerMatchStateHistory()
    {
        return true;
    }

    public bool ServerUpdateSpectatorCount()
    {
        return true;
    }

    public bool CacheSubscription()
    {
        return true;
    }

    public bool ClientHello()
    {
        ClientHelloInfo clientHello = ParseClientHello(_requestBody);
        AddProto(EMsgGCClientConnectionStatus, BuildConnectionStatus(clientHello));
        AddProto(EMsgDOTAGetEventPointsResponse, BuildEventPointsResponse(EventIdPlusSubscription, AccountId, true, 17));
        AddProto(EMsgDOTAGetEventPointsResponse, BuildEventPointsResponse(EventIdInitialProfile, AccountId, true, 17));
        AddProto(EMsgGCClientWelcome, BuildClientWelcome(clientHello));
        AddPostWelcomeCacheMessages(clientHello);
        AddProto(EMsgGCToClientProfileCardUpdated, BuildProfileCardForAccount(AccountId));
        AddProto(EMsgGCClientConnectionStatus, BuildConnectionStatusHaveSession(clientHello));
        return true;
    }

    public bool Standard()
    {
        switch (MessageType)
        {
            case EMsgGCClientHello:
                return ClientHello();
            case EMsgGCMatchmakingStatsRequest:
                return Reply(EMsgGCMatchmakingStatsResponse, BuildMatchmakingStatsResponse());
            case EMsgDOTAGetEventPoints:
                return Reply(EMsgDOTAGetEventPointsResponse, BuildEventPointsResponse(_requestBody));
            case EMsgGCNotificationsRequest:
                return Reply(EMsgGCNotificationsResponse, BuildNotificationsResponse());
            case EMsgClientToGCGetProfileCard:
                return Reply(EMsgClientToGCGetProfileCardResponse, BuildProfileCardResponse(_requestBody, false));
            case EMsgClientToGCGetProfileCardStats:
                return Reply(EMsgClientToGCGetProfileCardStatsResponse, BuildProfileCardResponse(_requestBody, true));
            case EMsgClientToGCGetProfileTickets:
                return Reply(EMsgClientToGCGetProfileTicketsResponse, BuildProfileTicketsResponse(_requestBody));
            case EMsgClientToGCGetQuestProgress:
                return Reply(EMsgClientToGCGetQuestProgressResponse, BuildResult(1));
            case EMsgClientToGCFindTopSourceTvGames:
                return Reply(EMsgGCToClientFindTopSourceTvGamesResponse, BuildFindTopSourceTvGamesResponse(_requestBody));
            case EMsgGCGetHeroStandings:
                return Reply(EMsgGCGetHeroStandingsResponse, Array.Empty<byte>());
            case EMsgGCGetHeroStatsHistory:
                return Reply(EMsgGCGetHeroStatsHistoryResponse, BuildHeroStatsHistoryResponse(_requestBody));
            case EMsgProfileRequest:
                return Reply(EMsgProfileResponse, BuildProfileResponse());
            case EMsgClientToGCRequestGuildData:
                return Reply(EMsgClientToGCRequestGuildDataResponse, BuildResult(5));
            case EMsgClientToGCRequestGuildMembership:
                return Reply(EMsgClientToGCRequestGuildMembershipResponse, BuildGuildMembershipResponse());
            case EMsgClientToGCRequestAccountGuildPersonaInfo:
                return Reply(EMsgClientToGCRequestAccountGuildPersonaInfoResponse, BuildAccountGuildPersonaInfoResponse());
            case EMsgClientToGCRequestAccountGuildPersonaInfoBatch:
                return Reply(EMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse, BuildAccountGuildPersonaInfoBatchResponse(_requestBody));
            case EMsgClientToGCGetCurrentPrivateCoachingSession:
                return Reply(EMsgClientToGCGetCurrentPrivateCoachingSessionResponse, BuildResult(1));
            case EMsgClientToGCGetAvailablePrivateCoachingSessions:
                return Reply(EMsgClientToGCGetAvailablePrivateCoachingSessionsResponse, BuildAvailablePrivateCoachingSessionsResponse());
            case EMsgClientToGCGetAvailablePrivateCoachingSessionsSummary:
                return Reply(EMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse, BuildAvailablePrivateCoachingSessionsSummaryResponse());
            case EMsgClientToGCRankRequest:
                return Reply(EMsgGCToClientRankResponse, Array.Empty<byte>());
            case EMsgClientToGCShowcaseGetUserData:
                return Reply(EMsgClientToGCShowcaseGetUserDataResponse, BuildResult(1));
            case EMsgClientToGCOverworldGetUserData:
                return Reply(EMsgClientToGCOverworldGetUserDataResponse, BuildResult(5));
            case EMsgClientToGCMonsterHunterGetUserData:
                return Reply(EMsgClientToGCMonsterHunterGetUserDataResponse, BuildResult(1));
            case EMsgGCRequestStoreSalesData:
                return Reply(EMsgGCRequestStoreSalesDataResponse, BuildStoreSalesDataResponse(_requestBody));
            case EMsgClientToGCEquipItems:
                return EquipItems();
            case EMsgClientToGCSetItemStyle:
                return SetItemStyle();
            case EMsgClientToGCUnknown8716:
            case EMsgClientToGCCancelUnfinalizedTransactions:
            case EMsgClientToGCAggregateMetrics:
                return Ignore();
            default:
                Response.Handled = false;
                return false;
        }
    }

    public bool Ignore()
    {
        Response.Handled = true;
        return true;
    }

    public bool NotHandled()
    {
        Response.Handled = false;
        return false;
    }

    public bool ReplyEmpty(uint responseMsg)
    {
        return Reply(responseMsg, Array.Empty<byte>());
    }

    public bool Reply(uint responseMsg, string payloadBase64)
    {
        return Reply(responseMsg, Decode(payloadBase64));
    }

    public bool Reply(uint responseMsg, byte[] payload)
    {
        if (_sourceJobId == InvalidJobId)
        {
            AddProto(responseMsg, payload);
        }
        else
        {
            AddRaw(responseMsg, payload, _sourceJobId);
        }

        return true;
    }

    public bool Proto(uint messageType, string payloadBase64)
    {
        AddProto(messageType, Decode(payloadBase64));
        return true;
    }

    public bool Raw(uint messageType, string payloadBase64)
    {
        AddRaw(messageType, Decode(payloadBase64), InvalidJobId);
        return true;
    }

    public bool MessageWithTargetJob(uint messageType, string payloadBase64, ulong targetJobId)
    {
        AddRaw(messageType, Decode(payloadBase64), targetJobId);
        return true;
    }

    public bool MessageWithTargetJobString(uint messageType, string payloadBase64, string targetJobId)
    {
        return MessageWithTargetJob(messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    public string FieldVarint(int fieldNumber, ulong value)
    {
        var response = new List<byte>();
        WriteVarintField(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldVarintString(int fieldNumber, string value)
    {
        return FieldVarint(fieldNumber, ParseUInt64(value));
    }

    public string FieldBool(int fieldNumber, bool value)
    {
        return FieldVarint(fieldNumber, value ? 1UL : 0UL);
    }

    public string FieldFixed32(int fieldNumber, uint value)
    {
        var response = new List<byte>();
        WriteFixed32Field(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldFixed64(int fieldNumber, ulong value)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, fieldNumber, value);
        return Encode(response.ToArray());
    }

    public string FieldFixed64String(int fieldNumber, string value)
    {
        return FieldFixed64(fieldNumber, ParseUInt64(value));
    }

    public string FieldString(int fieldNumber, string value)
    {
        var response = new List<byte>();
        WriteStringField(response, fieldNumber, value ?? string.Empty);
        return Encode(response.ToArray());
    }

    public string FieldBytes(int fieldNumber, string payloadBase64)
    {
        var response = new List<byte>();
        WriteBytesField(response, fieldNumber, Decode(payloadBase64));
        return Encode(response.ToArray());
    }

    public string Concat(params string[] payloadsBase64)
    {
        var response = new List<byte>();
        foreach (var payload in payloadsBase64 ?? Array.Empty<string>())
        {
            response.AddRange(Decode(payload));
        }

        return Encode(response.ToArray());
    }

    public ulong ReadVarint(int fieldNumber, ulong defaultValue = 0)
    {
        return TryReadVarintField(_requestBody, fieldNumber, out var value) ? value : defaultValue;
    }

    public ulong ReadVarintAt(int fieldNumber, int occurrence, ulong defaultValue = 0)
    {
        var values = ReadVarintFields(_requestBody, fieldNumber);
        return occurrence > 0 && occurrence <= values.Count ? values[occurrence - 1] : defaultValue;
    }

    public string ReadVarintString(int fieldNumber, string defaultValue = "0")
    {
        return ReadVarintAtString(fieldNumber, 1, defaultValue);
    }

    public string ReadVarintAtString(int fieldNumber, int occurrence, string defaultValue = "0")
    {
        var values = ReadVarintFields(_requestBody, fieldNumber);
        return occurrence > 0 && occurrence <= values.Count
            ? values[occurrence - 1].ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public string ReadFixed64String(int fieldNumber, string defaultValue = "0")
    {
        return ReadFixed64AtString(fieldNumber, 1, defaultValue);
    }

    public string ReadFixed64AtString(int fieldNumber, int occurrence, string defaultValue = "0")
    {
        return TryReadFixed64FieldAt(_requestBody, fieldNumber, occurrence, out var value)
            ? value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public uint ReadFixed32(int fieldNumber, uint defaultValue = 0)
    {
        return ReadFixed32At(fieldNumber, 1, defaultValue);
    }

    public uint ReadFixed32At(int fieldNumber, int occurrence, uint defaultValue = 0)
    {
        return TryReadFixed32FieldAt(_requestBody, fieldNumber, occurrence, out var value)
            ? value
            : defaultValue;
    }

    public string ReadString(int fieldNumber)
    {
        return ReadStringAt(fieldNumber, 1);
    }

    public string ReadStringAt(int fieldNumber, int occurrence)
    {
        var values = ReadLengthDelimitedFields(_requestBody, fieldNumber);
        return occurrence > 0 && occurrence <= values.Count
            ? System.Text.Encoding.UTF8.GetString(values[occurrence - 1])
            : string.Empty;
    }

    public string ReadBytes(int fieldNumber)
    {
        return ReadBytesAt(fieldNumber, 1);
    }

    public string ReadBytesAt(int fieldNumber, int occurrence)
    {
        var values = ReadLengthDelimitedFields(_requestBody, fieldNumber);
        return occurrence > 0 && occurrence <= values.Count ? Encode(values[occurrence - 1]) : string.Empty;
    }

    public int FieldCount(int fieldNumber)
    {
        return GcWire.CountFields(_requestBody, fieldNumber);
    }

    public bool QueueTo(ulong steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        PendingMessageQueued?.Invoke(steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            Protobuf = protobuf
        });
        return true;
    }

    public bool QueueToString(string steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        return QueueTo(ParseUInt64(steamId), messageType, payloadBase64, protobuf);
    }

    public bool QueueReplyTo(ulong steamId, uint messageType, string payloadBase64, ulong targetJobId)
    {
        PendingMessageQueued?.Invoke(steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            TargetJobId = targetJobId == InvalidJobId ? null : targetJobId
        });
        return true;
    }

    public bool QueueReplyToString(string steamId, uint messageType, string payloadBase64, string targetJobId)
    {
        return QueueReplyTo(ParseUInt64(steamId), messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    public bool QueueToPoll(ulong steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        GameCoordinatorPendingMessages.Enqueue(AppId, steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            Protobuf = protobuf
        });
        return true;
    }

    public bool QueueToPollString(string steamId, uint messageType, string payloadBase64, bool protobuf = true)
    {
        return QueueToPoll(ParseUInt64(steamId), messageType, payloadBase64, protobuf);
    }

    public bool QueueReplyToPoll(ulong steamId, uint messageType, string payloadBase64, ulong targetJobId)
    {
        GameCoordinatorPendingMessages.Enqueue(AppId, steamId, new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(Decode(payloadBase64)),
            TargetJobId = targetJobId == InvalidJobId ? null : targetJobId
        });
        return true;
    }

    public bool QueueReplyToPollString(string steamId, uint messageType, string payloadBase64, string targetJobId)
    {
        return QueueReplyToPoll(ParseUInt64(steamId), messageType, payloadBase64, ParseUInt64(targetJobId));
    }

    public string NextObjectIdString()
    {
        return GenerateSteamObjectId().ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public bool PublishDotaMatchSnapshot(
        string lobbyId,
        string matchId,
        string serverSteamId,
        string connect,
        uint state,
        uint gameState,
        uint gameStartTime,
        bool dedicated,
        string players)
    {
        if (MatchSnapshotSink == null)
        {
            return false;
        }

        var snapshot = new ApiDotaMatch
        {
            LobbyId = ParseUInt64(lobbyId),
            MatchId = ParseUInt64(matchId),
            ServerSteamId = ParseUInt64(serverSteamId),
            Connect = connect ?? string.Empty,
            State = state,
            GameState = gameState,
            GameStartTime = gameStartTime,
            Dedicated = dedicated,
            UpdatedAt = DateTime.UtcNow,
            Players = ParseSnapshotPlayers(players)
        };

        // Lua owns the lobby state machine, so persist this transition in the
        // same order in which Lua produced it. An asynchronous write here can
        // race a following abandon/delete and resurrect a stale match.
        MatchSnapshotSink.Invoke(snapshot);
        return true;
    }

    public string DotaGetActiveMatchJson()
    {
        return MatchSnapshotJsonProvider?.Invoke(SteamId) ?? string.Empty;
    }

    public string DotaGetMatchJson(string lobbyId)
    {
        return MatchSnapshotByLobbyJsonProvider?.Invoke(ParseUInt64(lobbyId)) ?? string.Empty;
    }

    public bool DotaRemoveMatchSnapshot(string lobbyId)
    {
        return MatchSnapshotDeleteSink?.Invoke(ParseUInt64(lobbyId)) ?? false;
    }

    public bool QueueLobbyPlayerItemsToServer(string serverSteamId, string members)
    {
        if (InventoryProvider == null)
        {
            return false;
        }

        ulong serverId = ParseUInt64(serverSteamId);
        if (serverId == 0)
        {
            return false;
        }

        var snapshot = ParseSnapshotPlayers(members);
        Console.WriteLine($"[SKYNET][items] QueueLobbyPlayerItemsToServer server={serverId} members={snapshot.Count}");
        foreach (var member in snapshot)
        {
            // Send cosmetics for anyone who will actually play (Radiant/Dire/Pool).
            // At server-attach time in bot/practice matches the human is often still
            // in the player pool (teams not assigned yet), so filtering to Good/Bad
            // only would drop their equipped items and the hero renders default.
            if (member.Team == TeamSpectator || member.Team == TeamNoTeam)
            {
                continue;
            }

            var inventory = InventoryProvider(member.SteamId);
            var ownerSync = GenerateSteamObjectId();
            var gameCache = BuildSoCacheSubscribedForOwner(0, new[] { 1u }, member.SteamId, WelcomeVersion, ownerSync);
            Console.WriteLine($"[SKYNET][items]   service0 cache bytes={gameCache.Length} owner={member.SteamId}");
            QueueServerProto(serverId, EMsgSOCacheSubscribed,
                gameCache);

            var itemObjects = BuildEconItemsForUser(member.SteamId, inventory, onlyEquipped: true);
            var itemBytes = itemObjects.Sum(item => item.Length);
            Console.WriteLine($"[SKYNET][items]   member={member.SteamId} team={member.Team} equippedItems={itemObjects.Count} itemBytes={itemBytes}");
            if (itemObjects.Count == 0)
            {
                continue;
            }

            var itemCache = BuildSoCacheSubscribedForOwner(1, new[] { 0u }, member.SteamId, inventory.Version, ownerSync,
                BuildSubscribedType(1, itemObjects.ToArray()));
            Console.WriteLine($"[SKYNET][items]   service1 cache bytes={itemCache.Length} owner={member.SteamId}");
            QueueServerProto(serverId, EMsgSOCacheSubscribed,
                itemCache);
        }

        return true;
    }

    private void QueueServerProto(ulong serverId, uint messageType, byte[] payload)
    {
        if (serverId == SteamId)
        {
            AddProto(messageType, payload);
            return;
        }

        QueueToPoll(serverId, messageType, Encode(payload));
    }

    public bool DotaEquipItem(string itemId, uint heroClass, uint slot, uint style)
    {
        if (EquipItemSink == null)
        {
            return false;
        }

        var changed = EquipItemSink(SteamId, ParseUInt64(itemId), heroClass, slot, style);
        if (changed.Count == 0)
        {
            RefreshDotaInventoryAfterChanges();
            return false;
        }

        _dotaItemChanges.AddRange(changed);
        RefreshDotaInventoryAfterChanges();
        return true;
    }

    public bool DotaSetItemStyle(string itemId, uint style)
    {
        if (SetItemStyleSink == null)
        {
            return false;
        }

        var changed = SetItemStyleSink(SteamId, ParseUInt64(itemId), style);
        if (changed.Count == 0)
        {
            RefreshDotaInventoryAfterChanges();
            return false;
        }

        _dotaItemChanges.AddRange(changed);
        RefreshDotaInventoryAfterChanges();
        return true;
    }

    public string DotaChangedItemsClientUpdate()
    {
        var inventory = RefreshDotaInventoryAfterChanges();
        if (_dotaItemChanges.Count == 0 || inventory == null)
        {
            return string.Empty;
        }

        return Encode(BuildEconMultipleObjects(SteamId, inventory, _dotaItemChanges, EconObjectUpdateKind.Modified));
    }

    public bool DotaQueueChangedItemsToServer(string serverSteamId)
    {
        var inventory = RefreshDotaInventoryAfterChanges();
        ulong serverId = ParseUInt64(serverSteamId);
        if (serverId == 0 || _dotaItemChanges.Count == 0 || inventory == null)
        {
            return false;
        }

        QueueEconItemChangesToServer(serverId, SteamId, inventory, _dotaItemChanges);
        return true;
    }

    public uint DotaResolveItemDef(string itemId)
    {
        return ItemDefResolver?.Invoke(SteamId, ParseUInt64(itemId)) ?? 0;
    }

    public string DotaGetEquipmentJson()
    {
        return EquipmentJsonProvider?.Invoke(SteamId) ?? "[]";
    }

    public bool DotaSetEquipmentJson(string json)
    {
        var ok = EquipmentJsonSink?.Invoke(SteamId, json ?? string.Empty) ?? false;
        RefreshDotaInventoryAfterChanges();
        return ok;
    }

    public string DotaCatalogItemJson(uint defIndex)
    {
        return CatalogItemJsonProvider?.Invoke(defIndex) ?? string.Empty;
    }

    public string DotaItemsClientUpdateFor(string defIndexesCsv)
    {
        var changed = ParseDefIndexCsv(defIndexesCsv);
        var inventory = RefreshDotaInventoryAfterChanges();
        if (changed.Count == 0 || inventory == null)
        {
            return string.Empty;
        }

        return Encode(BuildEconMultipleObjects(SteamId, inventory, changed, EconObjectUpdateKind.Modified));
    }

    public bool DotaQueueItemsToServerFor(string serverSteamId, string defIndexesCsv)
    {
        var changed = ParseDefIndexCsv(defIndexesCsv);
        var inventory = RefreshDotaInventoryAfterChanges();
        ulong serverId = ParseUInt64(serverSteamId);
        if (serverId == 0 || changed.Count == 0 || inventory == null)
        {
            return false;
        }

        QueueEconItemChangesToServer(serverId, SteamId, inventory, changed);
        return true;
    }

    private static List<ApiDotaEquipment> ParseDefIndexCsv(string? csv)
    {
        var result = new List<ApiDotaEquipment>();
        if (string.IsNullOrWhiteSpace(csv))
        {
            return result;
        }

        foreach (var part in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (uint.TryParse(part, out var defIndex) && defIndex != 0)
            {
                result.Add(new ApiDotaEquipment { DefIndex = defIndex });
            }
        }

        return result;
    }

    public string DotaEquipItemsResponse()
    {
        var inventory = RefreshDotaInventoryAfterChanges();
        return Encode(BuildEquipItemsResponse(inventory?.Version ?? GetCurrentCacheVersion(1)));
    }

    public string DotaSetItemStyleResponse(bool success)
    {
        return Encode(BuildResult(success ? 1u : 0u));
    }

    public string DotaInventoryVersionString()
    {
        return GetCurrentCacheVersion(1).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private ApiDotaRuntimeInventory? RefreshDotaInventoryAfterChanges()
    {
        _dotaInventoryAfterChanges = InventoryProvider?.Invoke(SteamId);
        return _dotaInventoryAfterChanges;
    }

    public string Result(uint result) => Encode(BuildResult(result));
    public string MatchmakingStatsResponse() => Encode(BuildMatchmakingStatsResponse());
    public string EventPointsResponse() => Encode(BuildEventPointsResponse(_requestBody));
    public string NotificationsResponse() => Encode(BuildNotificationsResponse());
    public string ProfileCardResponse(bool forceLocalAccount) => Encode(BuildProfileCardResponse(_requestBody, forceLocalAccount));
    public string ProfileTicketsResponse() => Encode(BuildProfileTicketsResponse(_requestBody));
    public string ProfileResponse() => Encode(BuildProfileResponse());
    public string GuildMembershipResponse() => Encode(BuildGuildMembershipResponse());
    public string AccountGuildPersonaInfoResponse() => Encode(BuildAccountGuildPersonaInfoResponse());
    public string AccountGuildPersonaInfoBatchResponse() => Encode(BuildAccountGuildPersonaInfoBatchResponse(_requestBody));
    public string AvailablePrivateCoachingSessionsResponse() => Encode(BuildAvailablePrivateCoachingSessionsResponse());
    public string AvailablePrivateCoachingSessionsSummaryResponse() => Encode(BuildAvailablePrivateCoachingSessionsSummaryResponse());
    public string StoreSalesDataResponse() => Encode(BuildStoreSalesDataResponse(_requestBody));
    public string ConnectionStatus() => Encode(BuildConnectionStatus(ParseClientHello(_requestBody)));
    public string ConnectionStatusHaveSession() => Encode(BuildConnectionStatusHaveSession(ParseClientHello(_requestBody)));
    public string ClientWelcomeResponse() => Encode(BuildClientWelcome(ParseClientHello(_requestBody)));
    public string GameSoCacheSubscribed() => Encode(BuildGameSoCacheSubscribed());
    public string EconSoCacheSubscribed() => Encode(BuildEconSoCacheSubscribed());
    public string ProfileCardUpdate() => Encode(BuildProfileCardForAccount(AccountId));

    private void AddPostWelcomeCacheMessages(ClientHelloInfo clientHello)
    {
        if (clientHello.HaveVersions.Count == 0)
        {
            AddProto(EMsgSOCacheSubscribed, BuildGameSoCacheSubscribed());
            AddProto(EMsgSOCacheSubscribed, BuildEconSoCacheSubscribed());
            return;
        }

        bool reportedEconCache = false;
        foreach (var haveVersion in clientHello.HaveVersions)
        {
            if (haveVersion.OwnerType == 1)
            {
                var currentVersion = GetCurrentCacheVersion(haveVersion.ServiceId);
                if (haveVersion.ServiceId == 1)
                {
                    reportedEconCache = true;
                }

                if (haveVersion.Version == currentVersion && (haveVersion.ServiceId == 0 || haveVersion.ServiceId == 1))
                {
                    AddProto(EMsgSOCacheSubscribedUpToDate, BuildSoCacheSubscribedUpToDate(haveVersion));
                }
                else if (haveVersion.ServiceId == 1)
                {
                    AddProto(EMsgSOCacheSubscribed, BuildEconSoCacheSubscribed());
                }
            }
            else if (haveVersion.OwnerType == 2 || haveVersion.OwnerType == 3)
            {
                AddProto(EMsgSOCacheUnsubscribed, BuildSoCacheUnsubscribed(haveVersion.OwnerType, haveVersion.OwnerId));
            }
        }

        if (!reportedEconCache)
        {
            AddProto(EMsgSOCacheSubscribed, BuildEconSoCacheSubscribed());
        }
    }

    private void AddProto(uint messageType, byte[] payload)
    {
        AddMessage(messageType, payload, null, true);
    }

    private void AddRaw(uint messageType, byte[] payload, ulong targetJobId)
    {
        AddMessage(messageType, payload, targetJobId == InvalidJobId ? null : targetJobId, false);
    }

    private void AddMessage(uint messageType, byte[] payload, ulong? targetJobId, bool protobuf)
    {
        Response.Messages.Add(new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            TargetJobId = targetJobId,
            Protobuf = protobuf
        });
    }

    private byte[] BuildClientWelcome(ClientHelloInfo clientHello)
    {
        var dotaWelcome = new List<byte>();
        WriteVarintField(dotaWelcome, 5, 0);
        WriteVarintField(dotaWelcome, 6, 0);
        WriteVarintField(dotaWelcome, 7, 1);
        WriteVarintField(dotaWelcome, 17, 0);
        WriteVarintField(dotaWelcome, 18, 0);
        WriteVarintField(dotaWelcome, 20, 0);
        WriteVarintField(dotaWelcome, 22, WelcomeVersion);
        WriteBytesField(dotaWelcome, 26, BuildExtraMessage(8024, BuildTopCustomGamesList()));
        WriteBytesField(dotaWelcome, 26, BuildExtraMessage(8075, BuildMatchGroupsVersion()));
        WriteBytesField(dotaWelcome, 26, BuildExtraMessage(8067, BuildChatRegionsEnabled()));
        WriteBytesField(dotaWelcome, 26, BuildExtraMessage(7465, BuildWeekendTourneySchedule()));
        WriteBytesField(dotaWelcome, 26, BuildExtraMessage(2591, BuildItemAges()));
        WriteVarintField(dotaWelcome, 28, ActiveEventId);
        WriteVarintField(dotaWelcome, 35, ActiveEventId);

        var clientWelcome = new List<byte>();
        WriteVarintField(clientWelcome, 1, clientHello.Version != 0 ? clientHello.Version : ClientWelcomeVersion);
        WriteBytesField(clientWelcome, 2, dotaWelcome.ToArray());
        if (!HasCurrentCache(clientHello, 0))
        {
            WriteBytesField(clientWelcome, 3, BuildGameSoCacheSubscribed());
        }

        foreach (var haveVersion in clientHello.HaveVersions)
        {
            if (haveVersion.OwnerType == 1 &&
                haveVersion.Version == GetCurrentCacheVersion(haveVersion.ServiceId) &&
                (haveVersion.ServiceId == 0 || haveVersion.ServiceId == 1))
            {
                WriteBytesField(clientWelcome, 4, BuildSoCacheSubscriptionCheck(haveVersion));
            }
        }

        WriteBytesField(clientWelcome, 5, BuildLocation());
        WriteVarintField(clientWelcome, 9, WelcomeVersion);
        WriteStringField(clientWelcome, 10, "US");
        WriteVarintField(clientWelcome, 12, (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        WriteVarintField(clientWelcome, 13, 0);
        return clientWelcome.ToArray();
    }

    private byte[] BuildGameServerWelcome(uint serverVersion)
    {
        var clientWelcome = new List<byte>();
        WriteVarintField(clientWelcome, 1, serverVersion != 0 ? serverVersion : WelcomeVersion);
        WriteVarintField(clientWelcome, 9, WelcomeVersion);
        return clientWelcome.ToArray();
    }

    private byte[] BuildGameServerSoCacheSubscribed(ulong syncVersion)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, 3, GenerateSteamObjectId());
        WriteBytesField(response, 4, BuildOwnerSoid(1, SteamId));
        WriteVarintField(response, 5, 0);
        WriteVarintField(response, 6, 1);
        WriteFixed64Field(response, 7, syncVersion);
        return response.ToArray();
    }

    private byte[] BuildBatchPlayerResourcesResponse(byte[] requestBody)
    {
        var response = new List<byte>();
        foreach (ulong accountId in ReadVarintFields(requestBody, 1))
        {
            var result = new List<byte>();
            WriteVarintField(result, 1, accountId);
            WriteVarintField(result, 2, 0);
            WriteVarintField(result, 3, 0);
            WriteVarintField(result, 4, 0);
            WriteVarintField(result, 5, 0);
            WriteVarintField(result, 6, 0);
            WriteBytesField(response, 6, result.ToArray());
        }

        return response.ToArray();
    }

    private static byte[] BuildRecentAccomplishmentsResponse(uint accountId, int heroId)
    {
        uint now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var playerRecord = BuildPlayerMatchRecord(0, 0);
        var playerCommends = BuildPlayerRecentCommends(0, 0);
        var playerMvps = BuildPlayerRecentMatchOutcomes(0, 0);

        var player = new List<byte>();
        WriteBytesField(player, 2, playerRecord);
        WriteVarintField(player, 3, 0);
        WriteVarintField(player, 4, 0);
        WriteBytesField(player, 5, playerCommends);
        WriteVarintField(player, 6, now);
        WriteBytesField(player, 8, playerMvps);

        var hero = new List<byte>();
        WriteBytesField(hero, 1, BuildPlayerRecentMatchOutcomes(0, 0));
        WriteBytesField(hero, 2, BuildPlayerMatchRecord(0, 0));

        var recent = new List<byte>();
        WriteBytesField(recent, 1, player.ToArray());
        WriteBytesField(recent, 2, hero.ToArray());

        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, recent.ToArray());
        return response.ToArray();
    }

    private static byte[] BuildPlayerMatchRecord(uint wins, uint losses)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, wins);
        WriteVarintField(response, 2, losses);
        return response.ToArray();
    }

    private static byte[] BuildPlayerRecentMatchOutcomes(uint outcomes, uint matchCount)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, outcomes);
        WriteVarintField(response, 2, matchCount);
        return response.ToArray();
    }

    private static byte[] BuildPlayerRecentCommends(uint commends, uint matchCount)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, commends);
        WriteVarintField(response, 2, matchCount);
        return response.ToArray();
    }

    private byte[] BuildBoolField(int fieldNumber, bool value)
    {
        var response = new List<byte>();
        WriteVarintField(response, fieldNumber, value ? 1u : 0u);
        return response.ToArray();
    }

    private static byte[] BuildRealtimeStatsStartStop(bool delayed)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, delayed ? 1u : 0u);
        return response.ToArray();
    }

    private byte[] BuildGameMatchSignOutResponse(ulong matchId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, matchId);
        WriteFixed32Field(response, 2, 0);
        WriteVarintField(response, 5, 0);
        WriteFixed32Field(response, 7, 1);
        return response.ToArray();
    }

    private byte[] BuildMatchSignedOut(ulong matchId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, matchId);
        return response.ToArray();
    }

    private byte[] BuildExtraMessage(uint messageId, byte[] contents)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, messageId);
        WriteBytesField(response, 2, contents);
        return response.ToArray();
    }

    private bool HasCurrentCache(ClientHelloInfo clientHello, uint serviceId)
    {
        var currentVersion = GetCurrentCacheVersion(serviceId);
        foreach (var haveVersion in clientHello.HaveVersions)
        {
            if (haveVersion.OwnerType == 1 && haveVersion.ServiceId == serviceId && haveVersion.Version == currentVersion)
            {
                return true;
            }
        }

        return false;
    }

    private ulong GetCurrentCacheVersion(uint serviceId)
    {
        if (serviceId == 1)
        {
            return InventoryProvider?.Invoke(SteamId)?.Version ?? WelcomeVersion;
        }

        return WelcomeVersion;
    }

    private byte[] BuildSoCacheSubscriptionCheck(SoCacheHaveVersion haveVersion)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, 2, GetCurrentCacheVersion(haveVersion.ServiceId));
        WriteBytesField(response, 3, BuildOwnerSoid(haveVersion.OwnerType, haveVersion.OwnerId));
        WriteVarintField(response, 4, haveVersion.ServiceId);
        return response.ToArray();
    }

    private byte[] BuildSoCacheSubscribedUpToDate(SoCacheHaveVersion haveVersion)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, 1, GetCurrentCacheVersion(haveVersion.ServiceId));
        WriteBytesField(response, 2, BuildOwnerSoid(haveVersion.OwnerType, haveVersion.OwnerId));
        WriteVarintField(response, 3, haveVersion.ServiceId);
        foreach (uint service in GetServiceList(haveVersion.ServiceId))
        {
            WriteVarintField(response, 4, service);
        }

        WriteFixed64Field(response, 5, 1);
        return response.ToArray();
    }

    private byte[] BuildSoCacheUnsubscribed(uint ownerType, ulong ownerId)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildOwnerSoid(ownerType, ownerId));
        return response.ToArray();
    }

    private uint[] GetServiceList(uint serviceId)
    {
        if (serviceId == 0)
        {
            return new[] { 1u };
        }

        if (serviceId == 1)
        {
            return new[] { 0u };
        }

        return Array.Empty<uint>();
    }

    private byte[] BuildTopCustomGamesList()
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, 0);
        return response.ToArray();
    }

    private byte[] BuildMatchGroupsVersion()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 107);
        return response.ToArray();
    }

    private byte[] BuildChatRegionsEnabled()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        return response.ToArray();
    }

    private byte[] BuildWeekendTourneySchedule()
    {
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildWeekendTourneyDivision(2, 1618077600, 1618078500, 1618682400, 67, false));
        WriteBytesField(response, 1, BuildWeekendTourneyDivision(3, 1618052400, 1618053300, 1618657200, 67, false));
        WriteBytesField(response, 1, BuildWeekendTourneyDivision(4, 1618056000, 1618056900, 1618660800, 67, false));
        WriteBytesField(response, 1, BuildWeekendTourneyDivision(6, 1618102800, 1618103700, 1618707600, 67, false));
        WriteBytesField(response, 1, BuildWeekendTourneyDivision(7, 1618095600, 1618096500, 1618700400, 67, false));
        return response.ToArray();
    }

    private byte[] BuildWeekendTourneyDivision(uint divisionCode, uint open, uint close, uint openNext, uint trophyId, bool freeWeekend)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, divisionCode);
        WriteVarintField(response, 2, open);
        WriteVarintField(response, 3, close);
        WriteVarintField(response, 4, openNext);
        WriteVarintField(response, 5, trophyId);
        WriteVarintField(response, 6, freeWeekend ? 1u : 0u);
        return response.ToArray();
    }

    private byte[] BuildItemAges()
    {
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildItemAge(1603497600, 19494503167UL));
        WriteBytesField(response, 1, BuildItemAge(1602979200, 19474865616UL));
        WriteBytesField(response, 1, BuildItemAge(1600992000, 19378682271UL));
        return response.ToArray();
    }

    private byte[] BuildItemAge(uint timestamp, ulong maxItemId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, timestamp);
        WriteVarintField(response, 2, maxItemId);
        return response.ToArray();
    }

    private byte[] BuildConnectionStatus(ClientHelloInfo clientHello)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 3);
        if (clientHello.ClientSessionNeed != 0)
        {
            WriteVarintField(response, 2, clientHello.ClientSessionNeed);
        }

        return response.ToArray();
    }

    private byte[] BuildConnectionStatusHaveSession(ClientHelloInfo clientHello)
    {
        var response = new List<byte>();
        if (clientHello.ClientSessionNeed != 0)
        {
            WriteVarintField(response, 2, clientHello.ClientSessionNeed);
        }

        return response.ToArray();
    }

    private byte[] BuildGameSoCacheSubscribed()
    {
        return BuildSoCacheSubscribed(
            0,
            new[] { 1u },
            BuildSubscribedType(2002, BuildDotaGameAccountClient()),
            BuildSubscribedType(2012, BuildDotaGameAccountPlus()));
    }

    private byte[] BuildEconSoCacheSubscribed()
    {
        var inventory = InventoryProvider?.Invoke(SteamId);
        var version = inventory?.Version ?? WelcomeVersion;
        var objects = new List<byte[]>
        {
            BuildSubscribedType(7, BuildEconGameAccountClient()),
            BuildSubscribedType(7, BuildDotaGameAccountClient()),
            BuildSubscribedType(2010)
        };

        var econItems = BuildEconItemsForUser(SteamId, inventory, onlyEquipped: false);
        if (econItems.Count > 0)
        {
            objects.Add(BuildSubscribedType(1, econItems.ToArray()));
        }

        return BuildSoCacheSubscribedForOwner(
            1,
            new[] { 0u },
            SteamId,
            version,
            1,
            objects.ToArray());
    }

    private byte[] BuildSoCacheSubscribed(uint serviceId, uint[] serviceList, params byte[][] objects)
    {
        return BuildSoCacheSubscribedForOwner(serviceId, serviceList, SteamId, WelcomeVersion, 1, objects);
    }

    private static byte[] BuildSoCacheSubscribedForOwner(
        uint serviceId,
        uint[] serviceList,
        ulong ownerId,
        ulong version,
        ulong syncVersion,
        params byte[][] objects)
    {
        var response = new List<byte>();
        foreach (var item in objects)
        {
            WriteBytesField(response, 2, item);
        }

        WriteFixed64Field(response, 3, version);
        WriteBytesField(response, 4, BuildOwnerSoid(1, ownerId));
        WriteVarintField(response, 5, serviceId);
        foreach (uint service in serviceList)
        {
            WriteVarintField(response, 6, service);
        }

        WriteFixed64Field(response, 7, syncVersion);
        return response.ToArray();
    }

    private static byte[] BuildSubscribedType(int typeId, params byte[][] objectData)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, (ulong)typeId);
        foreach (var item in objectData)
        {
            WriteBytesField(response, 2, item);
        }

        return response.ToArray();
    }

    private byte[] BuildOwnerSoid()
    {
        return BuildOwnerSoid(1, SteamId);
    }

    private static byte[] BuildOwnerSoid(uint ownerType, ulong ownerId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, ownerType);
        WriteVarintField(response, 2, ownerId);
        return response.ToArray();
    }

    private byte[] BuildDotaGameAccountClient()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, AccountId);
        WriteVarintField(response, 3, 285);
        WriteVarintField(response, 4, 363);
        WriteVarintField(response, 12, 875);
        WriteVarintField(response, 13, 21);
        WriteVarintField(response, 14, 100);
        WriteVarintField(response, 22, 1602786821);
        WriteVarintField(response, 24, 1);
        WriteVarintField(response, 49, 810);
        WriteVarintField(response, 59, 1602288064);
        WriteVarintField(response, 60, 244);
        WriteVarintField(response, 61, 261);
        WriteVarintField(response, 62, 142);
        WriteVarintField(response, 67, 82);
        WriteVarintField(response, 69, 3);
        WriteVarintField(response, 70, 74);
        WriteVarintField(response, 71, 29);
        WriteVarintField(response, 72, 8507);
        WriteVarintField(response, 73, 0);
        WriteVarintField(response, 88, 76561197960272819UL);
        WriteVarintField(response, 90, 1603411556);
        WriteVarintField(response, 91, 1602784740);
        WriteVarintField(response, 92, 1602782130);
        WriteVarintField(response, 104, 1602787292);
        WriteVarintField(response, 106, 1603414757);
        WriteVarintField(response, 107, 1602784665);
        WriteVarintField(response, 108, 1602781823);
        WriteBytesField(response, 115, BuildRoleHandicap(4, 0.26f));
        WriteBytesField(response, 115, BuildRoleHandicap(2, 0.26f));
        WriteBytesField(response, 115, BuildRoleHandicap(1, 0.26f));
        WriteBytesField(response, 115, BuildRoleHandicap(8, 0.26f));
        WriteBytesField(response, 115, BuildRoleHandicap(16, 0.26f));
        WriteVarintField(response, 116, 15);
        WriteVarintField(response, 120, 1602781823);
        WriteVarintField(response, 121, 1602781823);
        return response.ToArray();
    }

    private byte[] BuildRoleHandicap(uint role, float handicap)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, role);
        WriteFloatField(response, 2, handicap);
        return response.ToArray();
    }

    private byte[] BuildDotaGameAccountPlus()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, AccountId);
        WriteVarintField(response, 2, 0);
        WriteVarintField(response, 3, 0);
        WriteVarintField(response, 4, 0);
        WriteVarintField(response, 5, 0);
        WriteVarintField(response, 6, 0);
        WriteFixed32Field(response, 7, 0);
        WriteFixed64Field(response, 8, 0);
        return response.ToArray();
    }

    private byte[] BuildEconGameAccountClient()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 0);
        WriteVarintField(response, 2, 0);
        WriteVarintField(response, 3, 1);
        WriteVarintField(response, 4, 0);
        WriteVarintField(response, 5, 0);
        WriteFixed32Field(response, 6, 0);
        WriteVarintField(response, 9, 0);
        return response.ToArray();
    }

    private byte[] BuildLocation()
    {
        var response = new List<byte>();
        WriteFloatField(response, 1, 37.7749f);
        WriteFloatField(response, 2, -122.4194f);
        WriteStringField(response, 3, "US");
        return response.ToArray();
    }

    private byte[] BuildMatchmakingStatsResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 107);
        return response.ToArray();
    }

    private byte[] BuildEventPointsResponse(byte[] requestBody)
    {
        uint responseEventId = 0;
        uint responseAccountId = AccountId;

        if (TryReadVarintField(requestBody, 1, out ulong eventId))
        {
            responseEventId = (uint)eventId;
        }

        if (TryReadVarintField(requestBody, 2, out ulong accountId))
        {
            responseAccountId = (uint)accountId;
        }

        return BuildEventPointsResponse(responseEventId, responseAccountId, true, 35);
    }

    private byte[] BuildEventPointsResponse(uint eventId, uint accountId, bool owned, uint auditAction)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 0);
        WriteVarintField(response, 2, 0);
        WriteVarintField(response, 3, eventId);
        WriteVarintField(response, 4, 0);
        WriteVarintField(response, 5, 0);
        WriteVarintField(response, 7, accountId);
        WriteVarintField(response, 8, owned ? 1u : 0u);
        WriteVarintField(response, 9, auditAction);
        return response.ToArray();
    }

    private byte[] BuildStoreSalesDataResponse(byte[] requestBody)
    {
        TryReadVarintField(requestBody, 1, out ulong requestedVersion);
        var response = new List<byte>();
        WriteVarintField(response, 2, requestedVersion);
        WriteVarintField(response, 3, (ulong)DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds());
        return response.ToArray();
    }

    private byte[] BuildNotificationsResponse()
    {
        var update = new List<byte>();
        WriteVarintField(update, 1, 0);

        var response = new List<byte>();
        WriteBytesField(response, 1, update.ToArray());
        return response.ToArray();
    }

    private byte[] BuildProfileCardResponse(byte[] requestBody, bool forceLocalAccount)
    {
        uint accountId = AccountId;
        if (!forceLocalAccount && TryReadVarintField(requestBody, 1, out ulong requestedAccountId) && requestedAccountId != 0)
        {
            accountId = (uint)requestedAccountId;
        }

        return BuildProfileCardForAccount(accountId);
    }

    private byte[] BuildProfileCardForAccount(uint accountId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, accountId);
        WriteBytesField(response, 3, BuildProfileCardHeroSlot(1, 1, 64, 37));
        WriteBytesField(response, 3, BuildProfileCardHeroSlot(2, 2, 52, 29));
        WriteBytesField(response, 3, BuildProfileCardHeroSlot(3, 5, 45, 25));
        WriteBytesField(response, 3, BuildProfileCardStatSlot(4, 3, 285));
        WriteBytesField(response, 3, BuildProfileCardStatSlot(5, 5, 648));
        WriteVarintField(response, 4, 4500);
        WriteVarintField(response, 6, ActiveEventId);
        WriteVarintField(response, 8, 0);
        WriteVarintField(response, 9, 0);
        WriteVarintField(response, 10, 0);
        WriteVarintField(response, 11, 0);
        WriteVarintField(response, 12, 0);
        WriteVarintField(response, 17, 0);
        WriteVarintField(response, 23, 0);
        WriteVarintField(response, 25, 49);
        WriteVarintField(response, 26, 0);
        return response.ToArray();
    }

    private byte[] BuildProfileCardHeroSlot(uint slotId, uint heroId, uint wins, uint losses)
    {
        var hero = new List<byte>();
        WriteVarintField(hero, 1, heroId);
        WriteVarintField(hero, 2, wins);
        WriteVarintField(hero, 3, losses);

        var slot = new List<byte>();
        WriteVarintField(slot, 1, slotId);
        WriteBytesField(slot, 5, hero.ToArray());
        return slot.ToArray();
    }

    private byte[] BuildProfileCardStatSlot(uint slotId, uint statId, uint score)
    {
        var stat = new List<byte>();
        WriteVarintField(stat, 1, statId);
        WriteVarintField(stat, 2, score);

        var slot = new List<byte>();
        WriteVarintField(slot, 1, slotId);
        WriteBytesField(slot, 3, stat.ToArray());
        return slot.ToArray();
    }

    private byte[] BuildProfileTicketsResponse(byte[] requestBody)
    {
        uint accountId = AccountId;
        if (TryReadVarintField(requestBody, 1, out ulong requestedAccountId) && requestedAccountId != 0)
        {
            accountId = (uint)requestedAccountId;
        }

        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteVarintField(response, 2, accountId);
        return response.ToArray();
    }

    private static byte[] BuildFindTopSourceTvGamesResponse(byte[] requestBody)
    {
        var response = new List<byte>();
        if (TryReadStringField(requestBody, 1, out string searchKey) && !string.IsNullOrWhiteSpace(searchKey))
        {
            WriteStringField(response, 1, searchKey);
        }

        if (TryReadVarintField(requestBody, 2, out ulong leagueId))
        {
            WriteVarintField(response, 2, leagueId);
        }

        if (TryReadVarintField(requestBody, 3, out ulong heroId))
        {
            WriteVarintField(response, 3, heroId);
        }

        if (TryReadVarintField(requestBody, 4, out ulong startGame))
        {
            WriteVarintField(response, 4, startGame);
        }

        WriteVarintField(response, 5, 0);

        if (TryReadVarintField(requestBody, 5, out ulong gameListIndex))
        {
            WriteVarintField(response, 6, gameListIndex);
        }

        return response.ToArray();
    }

    private static byte[] BuildHeroStatsHistoryResponse(byte[] requestBody)
    {
        var response = new List<byte>();
        if (TryReadVarintField(requestBody, 1, out ulong heroId))
        {
            WriteVarintField(response, 1, heroId);
        }

        WriteVarintField(response, 3, 1);
        return response.ToArray();
    }

    private byte[] BuildProfileResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 6, 1);
        return response.ToArray();
    }

    private byte[] BuildGuildMembershipResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, Array.Empty<byte>());
        return response.ToArray();
    }

    private byte[] BuildAccountGuildPersonaInfoResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, BuildAccountGuildsPersonaInfo());
        return response.ToArray();
    }

    private byte[] BuildAccountGuildPersonaInfoBatchResponse(byte[] requestBody)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);

        foreach (ulong _ in ReadVarintFields(requestBody, 1))
        {
            WriteBytesField(response, 2, BuildAccountGuildsPersonaInfo());
        }

        return response.ToArray();
    }

    private byte[] BuildAccountGuildsPersonaInfo()
    {
        return Array.Empty<byte>();
    }

    private byte[] BuildAvailablePrivateCoachingSessionsResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, Array.Empty<byte>());
        return response.ToArray();
    }

    private byte[] BuildAvailablePrivateCoachingSessionsSummaryResponse()
    {
        var summary = new List<byte>();
        WriteVarintField(summary, 1, 50);

        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        WriteBytesField(response, 2, summary.ToArray());
        return response.ToArray();
    }

    private byte[] BuildResult(uint result)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, result);
        return response.ToArray();
    }

    private byte[] BuildJoinResponse(uint result)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, result);
        return response.ToArray();
    }

    private static byte[] BuildEquipItemsResponse(ulong cacheVersion)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, cacheVersion);
        return response.ToArray();
    }

    private enum EconObjectUpdateKind
    {
        Modified,
        Added,
        Removed
    }

    private static byte[] BuildEconMultipleObjects(
        ulong ownerSteamId,
        ApiDotaRuntimeInventory inventory,
        IEnumerable<ApiDotaEquipment> changedEquipment,
        EconObjectUpdateKind updateKind)
    {
        var changedObjects = new List<byte[]>();
        var equipmentByDefIndex = inventory.Equipment
            .GroupBy(item => item.DefIndex)
            .ToDictionary(group => group.Key, group => group.ToList());
        var inventoryPositions = BuildEconInventoryPositions(inventory);

        foreach (var defIndex in changedEquipment
                     .Where(equipment => equipment.DefIndex != 0)
                     .Select(equipment => equipment.DefIndex)
                     .Distinct())
        {
            var item = inventory.Items.FirstOrDefault(candidate => candidate.DefIndex == defIndex);
            if (item != null)
            {
                equipmentByDefIndex.TryGetValue(defIndex, out var currentEquipment);
                inventoryPositions.TryGetValue(defIndex, out var inventoryPosition);
                changedObjects.Add(BuildSubscribedType(1, BuildEconItem(ownerSteamId, item, currentEquipment ?? Enumerable.Empty<ApiDotaEquipment>(), inventoryPosition)));
            }
        }

        var response = new List<byte>();
        int fieldNumber = updateKind switch
        {
            EconObjectUpdateKind.Added => 4,
            EconObjectUpdateKind.Removed => 5,
            _ => 2
        };

        foreach (var item in changedObjects)
        {
            WriteBytesField(response, fieldNumber, item);
        }

        WriteFixed64Field(response, 3, inventory.Version);
        WriteBytesField(response, 6, BuildOwnerSoid(1, ownerSteamId));
        WriteVarintField(response, 7, 1);
        return response.ToArray();
    }

    private static Dictionary<uint, uint> BuildEconInventoryPositions(ApiDotaRuntimeInventory inventory)
    {
        var result = new Dictionary<uint, uint>();
        uint index = 1;
        foreach (var item in inventory.Items.OrderBy(item => item.DefIndex))
        {
            result[item.DefIndex] = index++;
        }

        return result;
    }

    private void QueueEconItemChangesToServer(
        ulong serverSteamId,
        ulong ownerSteamId,
        ApiDotaRuntimeInventory inventory,
        IEnumerable<ApiDotaEquipment> changedEquipment)
    {
        var equipmentByDefIndex = inventory.Equipment
            .GroupBy(item => item.DefIndex)
            .ToDictionary(group => group.Key, group => group.ToList());
        var inventoryPositions = BuildEconInventoryPositions(inventory);

        foreach (var defIndex in changedEquipment
                     .Where(equipment => equipment.DefIndex != 0)
                     .Select(equipment => equipment.DefIndex)
                     .Distinct())
        {
            var item = inventory.Items.FirstOrDefault(candidate => candidate.DefIndex == defIndex);
            if (item == null)
            {
                continue;
            }

            equipmentByDefIndex.TryGetValue(defIndex, out var currentEquipment);
            var current = currentEquipment ?? new List<ApiDotaEquipment>();
            inventoryPositions.TryGetValue(defIndex, out var inventoryPosition);
            var itemId = BuildDotaItemInstanceId(ownerSteamId, item.DefIndex);

            if (current.Count == 0)
            {
                QueueServerProto(serverSteamId, EMsgSOSingleObjectDestroyed,
                    BuildEconSingleObject(ownerSteamId, inventory.Version, BuildEconItemIdOnly(itemId)));
            }

            QueueServerProto(serverSteamId, EMsgSOSingleObject,
                BuildEconSingleObject(ownerSteamId, inventory.Version, BuildEconItem(ownerSteamId, item, current, inventoryPosition)));
        }
    }

    private static byte[] BuildEconSingleObject(ulong ownerSteamId, ulong version, byte[] objectData)
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, 1);
        WriteBytesField(response, 3, objectData);
        WriteFixed64Field(response, 4, version);
        WriteBytesField(response, 5, BuildOwnerSoid(1, ownerSteamId));
        WriteVarintField(response, 6, 1);
        return response.ToArray();
    }

    private static byte[] BuildEconItemIdOnly(ulong itemId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, itemId);
        return response.ToArray();
    }

    private static List<byte[]> BuildEconItemsForUser(
        ulong ownerSteamId,
        ApiDotaRuntimeInventory? inventory,
        bool onlyEquipped)
    {
        if (inventory == null)
        {
            return new List<byte[]>();
        }

        var equipmentByDefIndex = inventory.Equipment
            .GroupBy(item => item.DefIndex)
            .ToDictionary(group => group.Key, group => group.ToList());

        var items = new List<byte[]>();
        foreach (var item in inventory.Items)
        {
            equipmentByDefIndex.TryGetValue(item.DefIndex, out var equipped);
            if (onlyEquipped && (equipped == null || equipped.Count == 0))
            {
                continue;
            }

            items.Add(BuildEconItem(ownerSteamId, item, equipped ?? Enumerable.Empty<ApiDotaEquipment>(), (uint)(items.Count + 1)));
        }

        return items;
    }

    private static byte[] BuildEconItem(ulong ownerSteamId, ApiDotaItem item, IEnumerable<ApiDotaEquipment> equipment, uint inventoryPosition)
    {
        var response = new List<byte>();
        var itemId = BuildDotaItemInstanceId(ownerSteamId, item.DefIndex);
        WriteVarintField(response, 1, itemId);
        WriteVarintField(response, 2, SteamIdToAccountId(ownerSteamId));
        WriteVarintField(response, 3, inventoryPosition == 0 ? 1 : inventoryPosition);
        WriteVarintField(response, 4, item.DefIndex);
        WriteVarintField(response, 5, 1);
        WriteVarintField(response, 6, 1);
        WriteVarintField(response, 7, item.QualityId == 0 ? 6 : item.QualityId);
        WriteVarintField(response, 9, 2);
        var selectedStyle = 0u;
        foreach (var equipped in equipment)
        {
            selectedStyle = equipped.Style;
            WriteBytesField(response, 18, BuildEconItemEquipped(equipped.HeroId, equipped.SlotId));
        }

        WriteVarintField(response, 15, selectedStyle);
        return response.ToArray();
    }

    private static byte[] BuildEconItemEquipped(uint heroId, uint slotId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, heroId);
        WriteVarintField(response, 2, slotId);
        return response.ToArray();
    }

    private static ulong BuildDotaItemInstanceId(ulong steamId, uint defIndex)
    {
        ulong accountBits = steamId & 0xFFFFFFFFUL;
        return 0x7000000000000000UL | (accountBits << 20) | defIndex;
    }

    private static uint SteamIdToAccountId(ulong steamId)
    {
        return steamId >= 76561197960265728UL
            ? (uint)(steamId - 76561197960265728UL)
            : (uint)(steamId & 0xFFFFFFFFUL);
    }

    private PracticeLobbyState CreatePracticeLobbyLocked()
    {
        ulong lobbyId = GeneratePracticeLobbyId();

        while (PracticeLobbies.ContainsKey(lobbyId))
        {
            lobbyId++;
        }

        var lobby = new PracticeLobbyState
        {
            LobbyId = lobbyId,
            LeaderSteamId = SteamId,
            LeaderAccountId = AccountId,
            Version = GenerateSteamObjectId(),
            State = LobbyStateUi,
            GameMode = 1,
            GameName = "Room 1",
            ServerRegion = 0,
            Lan = true,
            AllowSpectating = false,
            DotaTvDelay = 0,
            CustomMapName = string.Empty,
            CustomGameMode = string.Empty,
            CustomGameId = 0,
            CustomMinPlayers = 0,
            CustomMaxPlayers = 0,
            Visibility = 0,
            GameState = GameStateInit,
            PassKey = string.Empty
        };

        PracticeLobbies[lobby.LobbyId] = lobby;
        GetOrAddLobbyMemberLocked(lobby, SteamId, AccountId, PersonaName);
        return lobby;
    }

    private static ulong GeneratePracticeLobbyId()
    {
        return GenerateSteamObjectId();
    }

    private static void RefreshLobbyVersion(PracticeLobbyState lobby)
    {
        lobby.Version = GenerateSteamObjectId();
        lobby.LastActivityUtc = DateTime.UtcNow;
    }

    private static ulong GenerateSteamObjectId()
    {
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        ulong counter = (ulong)Interlocked.Increment(ref SteamObjectCounter) & 0xFFFFFFUL;
        return (timestamp << 24) | counter;
    }

    private bool TryGetCurrentLobbyLocked(out PracticeLobbyState lobby, out PracticeLobbyMemberState member)
    {
        lobby = null!;
        member = null!;
        if (!PracticeLobbyBySteamId.TryGetValue(SteamId, out ulong lobbyId) || !PracticeLobbies.TryGetValue(lobbyId, out var foundLobby))
        {
            return false;
        }

        lobby = foundLobby;
        member = GetOrAddLobbyMemberLocked(lobby, SteamId, AccountId, PersonaName);
        return true;
    }

    private PracticeLobbyMemberState GetOrAddLobbyMemberLocked(PracticeLobbyState lobby, ulong steamId, uint accountId, string personaName)
    {
        var member = lobby.Members.FirstOrDefault(candidate => candidate.SteamId == steamId);
        if (member != null)
        {
            if (!string.IsNullOrWhiteSpace(personaName))
            {
                member.PersonaName = personaName;
            }

            member.LastSeenUtc = DateTime.UtcNow;
            lobby.LastActivityUtc = DateTime.UtcNow;
            PracticeLobbyBySteamId[steamId] = lobby.LobbyId;
            return member;
        }

        member = new PracticeLobbyMemberState
        {
            SteamId = steamId,
            AccountId = accountId,
            PersonaName = string.IsNullOrWhiteSpace(personaName) ? $"User{accountId}" : personaName,
            Team = steamId == lobby.LeaderSteamId ? TeamGoodGuys : TeamPlayerPool,
            Slot = steamId == lobby.LeaderSteamId ? 1u : 0u,
            CoachTeam = TeamNoTeam,
            Channel = 6,
            LastSeenUtc = DateTime.UtcNow
        };

        lobby.Members.Add(member);
        lobby.LastActivityUtc = DateTime.UtcNow;
        PracticeLobbyBySteamId[steamId] = lobby.LobbyId;
        return member;
    }

    private void LeaveCurrentLobbyLocked(ulong keepLobbyId = 0)
    {
        if (!PracticeLobbyBySteamId.TryGetValue(SteamId, out ulong lobbyId) || lobbyId == keepLobbyId || !PracticeLobbies.TryGetValue(lobbyId, out var lobby))
        {
            return;
        }

        var recipients = lobby.Members.Select(member => member.SteamId).ToList();
        bool destroyLobby = lobby.LeaderSteamId == SteamId || lobby.Members.Count <= 1;
        if (destroyLobby)
        {
            byte[] unsubscribed = BuildLobbySoCacheUnsubscribed(lobby);
            foreach (ulong memberSteamId in recipients)
            {
                PracticeLobbyBySteamId.Remove(memberSteamId);
                QueueProtoLocked(memberSteamId, EMsgSOCacheUnsubscribed, unsubscribed);
            }

            if (lobby.ServerSteamId != 0)
            {
                PracticeLobbyByServerSteamId.Remove(lobby.ServerSteamId);
            }

            PracticeLobbies.Remove(lobby.LobbyId);
            return;
        }

        lobby.Members.RemoveAll(member => member.SteamId == SteamId);
        PracticeLobbyBySteamId.Remove(SteamId);
        RefreshLobbyVersion(lobby);

        QueueProtoLocked(SteamId, EMsgSOCacheUnsubscribed, BuildLobbySoCacheUnsubscribed(lobby));
        BroadcastLobbyUpdateLocked(lobby, SteamId);
    }

    private void BroadcastLobbyUpdateLocked(PracticeLobbyState lobby, ulong exceptSteamId = 0, bool includeServer = false)
    {
        byte[] payload = BuildLobbyMultipleObjects(lobby);
        if (includeServer && lobby.ServerSteamId != 0 && lobby.ServerSteamId != exceptSteamId)
        {
            QueueProtoLocked(lobby.ServerSteamId, EMsgSOMultipleObjects, payload);
        }

        foreach (var member in lobby.Members)
        {
            if (member.SteamId == exceptSteamId)
            {
                continue;
            }

            QueueProtoLocked(member.SteamId, EMsgSOMultipleObjects, payload);
        }
    }

    private static void PublishLobbySnapshotLocked(PracticeLobbyState lobby)
    {
        if (MatchSnapshotSink == null)
        {
            return;
        }

        var snapshot = new ApiDotaMatch
        {
            LobbyId = lobby.LobbyId,
            MatchId = lobby.MatchId,
            ServerSteamId = lobby.ServerSteamId,
            Connect = lobby.Connect,
            State = lobby.State,
            GameState = lobby.GameState,
            GameStartTime = lobby.GameStartTime,
            UpdatedAt = DateTime.UtcNow,
            Players = lobby.Members.Select(member => new ApiDotaMatchPlayer
            {
                SteamId = member.SteamId,
                AccountId = member.AccountId,
                PersonaName = member.PersonaName,
                Team = member.Team,
                Slot = member.Slot,
                CoachTeam = member.CoachTeam,
                HeroId = member.HeroId
            }).ToList()
        };

        Task.Run(() => MatchSnapshotSink?.Invoke(snapshot));
    }

    private void QueueProtoLocked(ulong recipientSteamId, uint messageType, byte[] payload)
    {
        if (recipientSteamId == SteamId)
        {
            AddProto(messageType, payload);
            return;
        }

        var message = new ApiGCMessage
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            Protobuf = true
        };

        // Clients receive async GC messages through /api/events; only game servers
        // (which do not run the event pump) consume the poll queue. Queueing for both
        // would deliver the same message twice.
        if (PracticeLobbyByServerSteamId.ContainsKey(recipientSteamId))
        {
            if (!PendingGcMessages.TryGetValue(recipientSteamId, out var queue))
            {
                queue = new Queue<ApiGCMessage>();
                PendingGcMessages[recipientSteamId] = queue;
            }

            queue.Enqueue(message);
        }

        PendingMessageQueued?.Invoke(recipientSteamId, message);
    }

    private static void ExpireInactiveSessionsLocked(IReadOnlySet<ulong>? activeSteamIds)
    {
        var cutoff = DateTime.UtcNow - PracticeLobbyTimeout;
        foreach (var lobby in PracticeLobbies.Values.ToList())
        {
            var inactiveMembers = lobby.Members
                .Where(member => activeSteamIds != null
                    ? !activeSteamIds.Contains(member.SteamId)
                    : member.LastSeenUtc < cutoff)
                .Select(member => member.SteamId)
                .ToList();

            if (inactiveMembers.Count == 0 && lobby.LastActivityUtc >= cutoff)
            {
                continue;
            }

            if (inactiveMembers.Contains(lobby.LeaderSteamId) || lobby.Members.Count == inactiveMembers.Count)
            {
                var unsubscribed = BuildLobbySoCacheUnsubscribed(lobby);
                foreach (var member in lobby.Members.ToList())
                {
                    PracticeLobbyBySteamId.Remove(member.SteamId);
                    QueueStaticProtoLocked(member.SteamId, lobby, EMsgSOCacheUnsubscribed, unsubscribed);
                }

                if (lobby.ServerSteamId != 0)
                {
                    PracticeLobbyByServerSteamId.Remove(lobby.ServerSteamId);
                }

                PracticeLobbies.Remove(lobby.LobbyId);
                continue;
            }

            foreach (var steamId in inactiveMembers)
            {
                lobby.Members.RemoveAll(member => member.SteamId == steamId);
                PracticeLobbyBySteamId.Remove(steamId);
                QueueStaticProtoLocked(steamId, lobby, EMsgSOCacheUnsubscribed, BuildLobbySoCacheUnsubscribed(lobby));
            }

            RefreshLobbyVersion(lobby);
            var payload = BuildLobbyMultipleObjects(lobby);
            foreach (var member in lobby.Members)
            {
                QueueStaticProtoLocked(member.SteamId, lobby, EMsgSOMultipleObjects, payload);
            }
        }
    }

    private static void QueueStaticProtoLocked(ulong recipientSteamId, PracticeLobbyState lobby, uint messageType, byte[] payload)
    {
        var message = new ApiGCMessage
        {
            AppId = 570,
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            Protobuf = true
        };

        // Same split as QueueProtoLocked: poll queue for game servers, events for clients.
        if (PracticeLobbyByServerSteamId.ContainsKey(recipientSteamId))
        {
            if (!PendingGcMessages.TryGetValue(recipientSteamId, out var queue))
            {
                queue = new Queue<ApiGCMessage>();
                PendingGcMessages[recipientSteamId] = queue;
            }

            queue.Enqueue(message);
        }

        PendingMessageQueued?.Invoke(recipientSteamId, message);
    }

    private void AttachServerToLobbyLocked(PracticeLobbyState lobby, bool markRun)
    {
        if (SteamId != 0)
        {
            lobby.ServerSteamId = SteamId;
            PracticeLobbyByServerSteamId[SteamId] = lobby.LobbyId;
        }

        if (lobby.ServerPort == 0)
        {
            lobby.ServerPort = 27015;
        }

        if (markRun)
        {
            EnsureLobbyMatchIdLocked(lobby);
            lobby.State = LobbyStateRun;
            lobby.Connect = BuildConnectString(lobby);
            lobby.GameState = GameStateHeroSelection;
            foreach (var member in lobby.Members)
            {
                member.LeaverStatus = LeaverNone;
            }
        }
        else if (lobby.State != LobbyStateRun)
        {
            lobby.State = LobbyStateServerSetup;
        }

        if (markRun && lobby.GameStartTime == 0)
        {
            lobby.GameStartTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        RefreshLobbyVersion(lobby);
        SendLobbyPlayerItemsToServerLocked(lobby);
        if (markRun)
        {
            StartRealtimeStatsLocked(lobby);
        }

        BroadcastLobbyUpdateLocked(lobby, 0, true);
        PublishLobbySnapshotLocked(lobby);
    }

    private void StartRealtimeStatsLocked(PracticeLobbyState lobby)
    {
        if (lobby.ServerSteamId == 0 || lobby.RealtimeStatsStartStopSent)
        {
            return;
        }

        QueueProtoLocked(lobby.ServerSteamId, EMsgGCToServerRealtimeStatsStartStop, BuildRealtimeStatsStartStop(true));
        lobby.RealtimeStatsStartStopSent = true;
    }

    private bool TryGetWaitingLobbyLocked(out PracticeLobbyState lobby)
    {
        lobby = PracticeLobbies.Values
            .Where(candidate => candidate.State == LobbyStateServerSetup)
            .OrderByDescending(candidate => candidate.Version)
            .FirstOrDefault()!;
        return lobby != null;
    }

    private void UpdateLobbyServerPort(PracticeLobbyState lobby)
    {
        if (TryReadVarintField(_requestBody, 3, out ulong serverPort) && serverPort != 0)
        {
            lobby.ServerPort = (uint)serverPort;
        }
    }

    private ulong EnsureLobbyMatchIdLocked(PracticeLobbyState lobby)
    {
        if (lobby.MatchId == 0)
        {
            lobby.MatchId = ++MatchIdCounter;
        }

        return lobby.MatchId;
    }

    private void SendLobbyPlayerItemsToServerLocked(PracticeLobbyState lobby)
    {
        if (lobby.ServerSteamId == 0 || InventoryProvider == null)
        {
            return;
        }

        foreach (var member in lobby.Members)
        {
            // Include the player pool: at server-attach teams may not be assigned yet.
            if (member.Team == TeamSpectator || member.Team == TeamNoTeam)
            {
                continue;
            }

            var inventory = InventoryProvider(member.SteamId);
            var ownerSync = GenerateSteamObjectId();
            QueueProtoLocked(lobby.ServerSteamId, EMsgSOCacheSubscribed,
                BuildSoCacheSubscribedForOwner(0, new[] { 1u }, member.SteamId, WelcomeVersion, ownerSync));

            var itemObjects = BuildEconItemsForUser(member.SteamId, inventory, onlyEquipped: true);
            if (itemObjects.Count == 0)
            {
                continue;
            }

            QueueProtoLocked(lobby.ServerSteamId, EMsgSOCacheSubscribed,
                BuildSoCacheSubscribedForOwner(1, new[] { 0u }, member.SteamId, inventory.Version, ownerSync,
                    BuildSubscribedType(1, itemObjects.ToArray())));
        }
    }

    private static void ApplyConnectedPlayerUpdates(PracticeLobbyState lobby, byte[] body)
    {
        foreach (var player in ReadLengthDelimitedFields(body, 1))
        {
            bool hasSteamId = TryReadFixed64Field(player, 1, out ulong steamId) || TryReadVarintField(player, 1, out steamId);
            if (!hasSteamId)
            {
                continue;
            }

            var member = lobby.Members.FirstOrDefault(candidate => candidate.SteamId == steamId);
            if (member == null)
            {
                continue;
            }

            if (TryReadVarintField(player, 2, out ulong heroId) && heroId != 0)
            {
                member.HeroId = (uint)heroId;
            }

            member.LeaverStatus = LeaverNone;
        }
    }

    private static string BuildConnectString(PracticeLobbyState lobby)
    {
        uint port = lobby.ServerPort != 0 ? lobby.ServerPort : 27015;
        return $"127.0.0.1:{port} 127.0.0.1:{port}";
    }

    private void ApplyPracticeLobbyCreateRequest(PracticeLobbyState lobby, byte[] requestBody)
    {
        if (TryReadStringField(requestBody, 5, out string passKey))
        {
            lobby.PassKey = passKey;
        }

        if (TryReadLengthDelimitedField(requestBody, 7, out byte[] details))
        {
            ApplyPracticeLobbyDetails(lobby, details);
        }
    }

    private void ApplyPracticeLobbyDetails(PracticeLobbyState lobby, byte[] details)
    {
        if (TryReadStringField(details, 2, out string gameName) && !string.IsNullOrWhiteSpace(gameName))
        {
            lobby.GameName = gameName;
        }

        if (TryReadVarintField(details, 4, out ulong serverRegion))
        {
            lobby.ServerRegion = (uint)serverRegion;
        }

        if (TryReadVarintField(details, 5, out ulong gameMode) && gameMode != 0)
        {
            lobby.GameMode = (uint)gameMode;
        }

        if (TryReadLengthDelimitedField(details, 15, out byte[] passKeyBody))
        {
            lobby.PassKey = System.Text.Encoding.UTF8.GetString(passKeyBody);
        }

        if (TryReadVarintField(details, 24, out ulong dotaTvDelay))
        {
            lobby.DotaTvDelay = (uint)dotaTvDelay;
        }

        if (TryReadVarintField(details, 25, out ulong lan))
        {
            lobby.Lan = lan != 0;
        }

        if (TryReadStringField(details, 26, out string customGameMode))
        {
            lobby.CustomGameMode = customGameMode;
        }

        if (TryReadStringField(details, 27, out string customMapName))
        {
            lobby.CustomMapName = customMapName;
        }

        if (TryReadVarintField(details, 29, out ulong customGameId))
        {
            lobby.CustomGameId = customGameId;
        }

        if (TryReadVarintField(details, 30, out ulong minPlayers))
        {
            lobby.CustomMinPlayers = (uint)minPlayers;
        }

        if (TryReadVarintField(details, 31, out ulong maxPlayers))
        {
            lobby.CustomMaxPlayers = (uint)maxPlayers;
        }

        if (TryReadVarintField(details, 33, out ulong visibility))
        {
            lobby.Visibility = (uint)visibility;
        }
    }

    private static byte[] BuildLobbySoCacheSubscribed(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildSubscribedType(LobbyObjectTypeId, BuildLobbyObject(lobby)));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyInviteObjectTypeId, Array.Empty<byte>()));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyPersonaObjectTypeId, BuildLobbyPersonaObject(lobby)));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyBroadcastObjectTypeId, BuildLobbyBroadcastObject()));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyMemberObjectTypeId, BuildLobbyMemberSummaryObject(lobby)));
        WriteFixed64Field(response, 3, lobby.Version);
        WriteBytesField(response, 4, BuildOwnerSoid(3, lobby.LobbyId));
        return response.ToArray();
    }

    private static byte[] BuildLobbySoCacheUnsubscribed(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildOwnerSoid(3, lobby.LobbyId));
        return response.ToArray();
    }

    private static byte[] BuildLobbySingleObject(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, LobbyObjectTypeId);
        WriteBytesField(response, 3, BuildLobbyObject(lobby));
        WriteFixed64Field(response, 4, lobby.Version);
        WriteBytesField(response, 5, BuildOwnerSoid(3, lobby.LobbyId));
        return response.ToArray();
    }

    private static byte[] BuildLobbyMultipleObjects(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildSubscribedType(LobbyObjectTypeId, BuildLobbyObject(lobby)));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyMemberObjectTypeId, BuildLobbyMemberSummaryObject(lobby)));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyPersonaObjectTypeId, BuildLobbyPersonaObject(lobby)));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyBroadcastObjectTypeId, BuildLobbyBroadcastObject()));
        WriteBytesField(response, 2, BuildSubscribedType(LobbyInviteObjectTypeId, Array.Empty<byte>()));
        WriteFixed64Field(response, 3, lobby.Version);
        WriteBytesField(response, 6, BuildOwnerSoid(3, lobby.LobbyId));
        return response.ToArray();
    }

    private static byte[] BuildLobbyObject(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, lobby.LobbyId);
        WriteVarintField(response, 3, lobby.GameMode);
        WriteVarintField(response, 4, lobby.State);
        if (!string.IsNullOrWhiteSpace(lobby.Connect))
        {
            WriteStringField(response, 5, lobby.Connect);
        }

        if (lobby.ServerSteamId != 0)
        {
            WriteFixed64Field(response, 6, lobby.ServerSteamId);
        }

        WriteFixed64Field(response, 11, lobby.LeaderSteamId);
        WriteVarintField(response, 12, LobbyTypePractice);
        WriteVarintField(response, 13, 0);
        WriteVarintField(response, 14, 0);
        WriteStringField(response, 16, lobby.GameName);
        WriteVarintField(response, 21, lobby.ServerRegion);
        WriteVarintField(response, 28, lobby.GameState);
        if (lobby.MatchId != 0)
        {
            WriteVarintField(response, 30, lobby.MatchId);
        }

        WriteVarintField(response, 31, lobby.AllowSpectating ? 1u : 0u);
        WriteVarintField(response, 36, 3);
        WriteVarintField(response, 42, 0);
        WriteVarintField(response, 43, 0);
        WriteVarintField(response, 44, 0);
        WriteVarintField(response, 46, 0);
        WriteVarintField(response, 47, 0);
        WriteVarintField(response, 48, 0);
        WriteVarintField(response, 51, 0);
        WriteVarintField(response, 53, lobby.DotaTvDelay);
        WriteVarintField(response, 57, lobby.Lan ? 1u : 0u);
        WriteBytesField(response, 62, BuildLobbySelectionPriorityRule(lobby));
        WriteVarintField(response, 75, lobby.Visibility);
        WriteVarintField(response, 82, 0);
        if (lobby.GameStartTime != 0)
        {
            WriteVarintField(response, 87, lobby.GameStartTime);
        }

        WriteVarintField(response, 88, 0);
        WriteVarintField(response, 93, 3);
        WriteVarintField(response, 94, 0);
        WriteVarintField(response, 95, 0);
        WriteVarintField(response, 97, 0);
        WriteVarintField(response, 110, 0);
        WriteVarintField(response, 113, 0);
        for (int index = 0; index < lobby.Members.Count; index++)
        {
            WriteBytesField(response, 120, BuildLobbyMember(lobby.Members[index]));
            WriteVarintField(response, 121, (uint)index);
        }

        WriteVarintField(response, 127, 0);
        WriteVarintField(response, 128, (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return response.ToArray();
    }

    private static byte[] BuildLobbyMember(PracticeLobbyMemberState member)
    {
        var response = new List<byte>();
        WriteFixed64Field(response, 1, member.SteamId);
        WriteVarintField(response, 3, member.Team);
        WriteVarintField(response, 7, member.Slot);
        if (member.HeroId != 0)
        {
            WriteVarintField(response, 8, member.HeroId);
        }

        WriteVarintField(response, 16, member.LeaverStatus);
        if (member.CoachTeam != TeamNoTeam)
        {
            WriteVarintField(response, 23, member.CoachTeam);
        }

        return response.ToArray();
    }

    private static byte[] BuildLobbyPersonaObject(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        foreach (PracticeLobbyMemberState member in lobby.Members)
        {
            var personaName = new List<byte>();
            WriteStringField(personaName, 1, member.PersonaName);
            WriteBytesField(response, 1, personaName.ToArray());
        }

        WriteVarintField(response, 2, 0);
        return response.ToArray();
    }

    private static byte[] BuildLobbyBroadcastObject()
    {
        var response = new List<byte>();
        WriteBytesField(response, 1, Array.Empty<byte>());
        return response.ToArray();
    }

    private static byte[] BuildLobbyMemberSummaryObject(PracticeLobbyState lobby)
    {
        var response = new List<byte>();
        foreach (PracticeLobbyMemberState member in lobby.Members)
        {
            var entry = new List<byte>();
            WriteFixed64Field(entry, 1, member.SteamId);
            WriteVarintField(entry, 9, 0);
            WriteVarintField(entry, 11, 1);
            WriteVarintField(entry, 12, 0);
            WriteVarintField(entry, 13, 0);
            WriteVarintField(entry, 19, 75);
            WriteVarintField(entry, 19, 0);
            WriteVarintField(entry, 19, 0);
            WriteVarintField(entry, 19, 0);
            WriteBytesField(response, 1, entry.ToArray());
        }

        WriteFixed32Field(response, 2, 0);
        return response.ToArray();
    }

    private static byte[] BuildLobbySelectionPriorityRule(PracticeLobbyState lobby)
    {
        var rule = new List<byte>();
        WriteVarintField(rule, 1, lobby.LeaderAccountId);

        var choice = new List<byte>();
        WriteVarintField(choice, 1, 0);
        WriteBytesField(rule, 2, choice.ToArray());
        return rule.ToArray();
    }

    private ClientHelloInfo ParseClientHello(byte[] source)
    {
        var info = new ClientHelloInfo();
        if (TryReadVarintField(source, 1, out ulong value))
        {
            info.Version = (uint)value;
        }

        if (TryReadVarintField(source, 3, out value))
        {
            info.ClientSessionNeed = (uint)value;
        }

        foreach (byte[] haveVersionBody in ReadLengthDelimitedFields(source, 2))
        {
            if (TryParseSoCacheHaveVersion(haveVersionBody, out var haveVersion))
            {
                info.HaveVersions.Add(haveVersion);
            }
        }

        return info;
    }

    private static bool TryParseSoCacheHaveVersion(byte[] source, out SoCacheHaveVersion haveVersion)
    {
        haveVersion = new SoCacheHaveVersion
        {
            OwnerType = 1,
            OwnerId = 0,
            Version = 0,
            ServiceId = 0,
            CachedFileVersion = 0
        };

        if (TryReadLengthDelimitedField(source, 1, out byte[] ownerBody))
        {
            if (TryReadVarintField(ownerBody, 1, out ulong ownerType))
            {
                haveVersion.OwnerType = (uint)ownerType;
            }

            if (TryReadVarintField(ownerBody, 2, out ulong ownerId))
            {
                haveVersion.OwnerId = ownerId;
            }
        }

        if (TryReadFixed64Field(source, 2, out ulong fixedValue))
        {
            haveVersion.Version = fixedValue;
        }

        if (TryReadVarintField(source, 3, out ulong value))
        {
            haveVersion.ServiceId = (uint)value;
        }

        if (TryReadVarintField(source, 4, out value))
        {
            haveVersion.CachedFileVersion = (uint)value;
        }

        return haveVersion.OwnerId != 0 || haveVersion.Version != 0 || haveVersion.ServiceId != 0 || haveVersion.CachedFileVersion != 0;
    }

    private static void WriteBytesField(List<byte> destination, int fieldNumber, byte[] value)
    {
        WriteVarint(destination, (ulong)((fieldNumber << 3) | 2));
        WriteVarint(destination, (ulong)value.Length);
        destination.AddRange(value);
    }

    private static void WriteVarintField(List<byte> destination, int fieldNumber, ulong value)
    {
        WriteVarint(destination, (ulong)(fieldNumber << 3));
        WriteVarint(destination, value);
    }

    private static void WriteFixed32Field(List<byte> destination, int fieldNumber, uint value)
    {
        WriteVarint(destination, (ulong)((fieldNumber << 3) | 5));
        destination.AddRange(BitConverter.GetBytes(value));
    }

    private static void WriteFixed64Field(List<byte> destination, int fieldNumber, ulong value)
    {
        WriteVarint(destination, (ulong)((fieldNumber << 3) | 1));
        destination.AddRange(BitConverter.GetBytes(value));
    }

    private static void WriteFloatField(List<byte> destination, int fieldNumber, float value)
    {
        WriteVarint(destination, (ulong)((fieldNumber << 3) | 5));
        destination.AddRange(BitConverter.GetBytes(value));
    }

    private static void WriteStringField(List<byte> destination, int fieldNumber, string value)
    {
        WriteBytesField(destination, fieldNumber, System.Text.Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    private static void WriteVarint(List<byte> destination, ulong value)
    {
        while (value >= 0x80)
        {
            destination.Add((byte)(value | 0x80));
            value >>= 7;
        }

        destination.Add((byte)value);
    }

    private static List<byte[]> ReadLengthDelimitedFields(byte[] source, int expectedFieldNumber)
    {
        var result = new List<byte[]>();
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                break;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (fieldNumber == expectedFieldNumber && wireType == 2)
            {
                if (!TryReadVarint(source, ref index, out ulong length) || index + (int)length > source.Length)
                {
                    break;
                }

                byte[] body = new byte[(int)length];
                Array.Copy(source, index, body, 0, body.Length);
                index += body.Length;
                result.Add(body);
            }
            else if (!SkipField(source, ref index, wireType))
            {
                break;
            }
        }

        return result;
    }

    private static bool TryReadLengthDelimitedField(byte[] source, int expectedFieldNumber, out byte[] value)
    {
        List<byte[]> fields = ReadLengthDelimitedFields(source, expectedFieldNumber);
        if (fields.Count > 0)
        {
            value = fields[0];
            return true;
        }

        value = Array.Empty<byte>();
        return false;
    }

    private static bool TryReadStringField(byte[] source, int expectedFieldNumber, out string value)
    {
        if (TryReadLengthDelimitedField(source, expectedFieldNumber, out byte[] body))
        {
            value = System.Text.Encoding.UTF8.GetString(body);
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static bool TryReadFixed64Field(byte[] source, int expectedFieldNumber, out ulong value)
    {
        value = 0;
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                return false;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (wireType == 1)
            {
                if (index + 8 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber)
                {
                    value = BitConverter.ToUInt64(source, index);
                    return true;
                }

                index += 8;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private static bool TryReadFixed64FieldAt(byte[] source, int expectedFieldNumber, int occurrence, out ulong value)
    {
        value = 0;
        int seen = 0;
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                return false;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (wireType == 1)
            {
                if (index + 8 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
                {
                    value = BitConverter.ToUInt64(source, index);
                    return true;
                }

                index += 8;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private static bool TryReadFixed32FieldAt(byte[] source, int expectedFieldNumber, int occurrence, out uint value)
    {
        value = 0;
        int seen = 0;
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                return false;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (wireType == 5)
            {
                if (index + 4 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber && ++seen == occurrence)
                {
                    value = BitConverter.ToUInt32(source, index);
                    return true;
                }

                index += 4;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private static bool TryReadVarintField(byte[] source, int expectedFieldNumber, out ulong value)
    {
        value = 0;
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                return false;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (wireType == 0)
            {
                if (!TryReadVarint(source, ref index, out ulong fieldValue))
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber)
                {
                    value = fieldValue;
                    return true;
                }
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private static List<ulong> ReadVarintFields(byte[] source, int expectedFieldNumber)
    {
        var result = new List<ulong>();
        int index = 0;

        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out ulong key))
            {
                break;
            }

            int fieldNumber = (int)(key >> 3);
            int wireType = (int)(key & 7);
            if (fieldNumber == expectedFieldNumber && wireType == 0)
            {
                if (!TryReadVarint(source, ref index, out ulong fieldValue))
                {
                    break;
                }

                result.Add(fieldValue);
            }
            else if (fieldNumber == expectedFieldNumber && wireType == 2)
            {
                if (!TryReadVarint(source, ref index, out ulong length) || index + (int)length > source.Length)
                {
                    break;
                }

                int end = index + (int)length;
                while (index < end && TryReadVarint(source, ref index, out ulong packedValue))
                {
                    result.Add(packedValue);
                }
            }
            else if (!SkipField(source, ref index, wireType))
            {
                break;
            }
        }

        return result;
    }

    private static bool TryReadVarint(byte[] source, ref int index, out ulong value)
    {
        value = 0;
        int shift = 0;

        while (index < source.Length && shift < 64)
        {
            byte current = source[index++];
            value |= (ulong)(current & 0x7F) << shift;
            if ((current & 0x80) == 0)
            {
                return true;
            }

            shift += 7;
        }

        return false;
    }

    private static bool SkipField(byte[] source, ref int index, int wireType)
    {
        switch (wireType)
        {
            case 0:
                return TryReadVarint(source, ref index, out _);
            case 1:
                index += 8;
                return index <= source.Length;
            case 2:
                if (!TryReadVarint(source, ref index, out ulong length))
                {
                    return false;
                }

                index += (int)length;
                return index <= source.Length;
            case 5:
                index += 4;
                return index <= source.Length;
            default:
                return false;
        }
    }

    private static string Encode(byte[] payload)
    {
        return Convert.ToBase64String(payload ?? Array.Empty<byte>());
    }

    private static byte[] Decode(string? payloadBase64)
    {
        if (string.IsNullOrEmpty(payloadBase64))
        {
            return Array.Empty<byte>();
        }

        return Convert.FromBase64String(payloadBase64);
    }

    private static ulong ParseUInt64(string? value)
    {
        return ulong.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0UL;
    }

    private static uint ParseUInt32(string? value)
    {
        return uint.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0U;
    }

    private static List<ApiDotaMatchPlayer> ParseSnapshotPlayers(string? players)
    {
        var result = new List<ApiDotaMatchPlayer>();
        if (string.IsNullOrWhiteSpace(players))
        {
            return result;
        }

        foreach (var line in players.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('\t');
            if (parts.Length < 7)
            {
                continue;
            }

            result.Add(new ApiDotaMatchPlayer
            {
                SteamId = ParseUInt64(parts[0]),
                AccountId = ParseUInt32(parts[1]),
                PersonaName = parts[2],
                Team = ParseUInt32(parts[3]),
                Slot = ParseUInt32(parts[4]),
                CoachTeam = ParseUInt32(parts[5]),
                HeroId = ParseUInt32(parts[6])
            });
        }

        return result;
    }

    private sealed class ClientHelloInfo
    {
        public uint Version;
        public uint ClientSessionNeed;
        public List<SoCacheHaveVersion> HaveVersions { get; } = new();
    }

    private struct SoCacheHaveVersion
    {
        public uint OwnerType;
        public ulong OwnerId;
        public ulong Version;
        public uint ServiceId;
        public uint CachedFileVersion;
    }

    private sealed class PracticeLobbyState
    {
        public ulong LobbyId;
        public ulong LeaderSteamId;
        public uint LeaderAccountId;
        public ulong Version;
        public uint State;
        public uint GameMode;
        public string GameName = string.Empty;
        public uint ServerRegion;
        public bool Lan;
        public bool AllowSpectating;
        public uint DotaTvDelay;
        public string CustomMapName = string.Empty;
        public string CustomGameMode = string.Empty;
        public ulong CustomGameId;
        public uint CustomMinPlayers;
        public uint CustomMaxPlayers;
        public uint Visibility;
        public string PassKey = string.Empty;
        public ulong ServerSteamId;
        public uint ServerPort;
        public string Connect = string.Empty;
        public ulong MatchId;
        public uint GameStartTime;
        public uint GameState;
        public bool RealtimeStatsStartStopSent;
        public DateTime LastActivityUtc = DateTime.UtcNow;
        public List<PracticeLobbyMemberState> Members { get; } = new();
    }

    private sealed class PracticeLobbyMemberState
    {
        public ulong SteamId;
        public uint AccountId;
        public string PersonaName = string.Empty;
        public uint Team;
        public uint Slot;
        public uint CoachTeam;
        public uint LeaverStatus = LeaverDisconnected;
        public uint Channel;
        public uint HeroId;
        public DateTime LastSeenUtc = DateTime.UtcNow;
    }
}
