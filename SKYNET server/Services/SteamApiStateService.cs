using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SKYNET_server.GC.Dota2;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const int PersonaChangeName = 1;
    private const int PersonaChangeStatus = 2; // k_EPersonaChangeStatus (online/offline transition)
    private const int PersonaChangeAvatar = 64;
    private const int PersonaChangeRichPresence = 2048;
    private const int PersonaChangeRelationship = 512;
    private const int FriendRelationshipNone = 0;
    private const int FriendRelationshipRequestRecipient = 2;
    private const int FriendRelationshipFriend = 3;
    private const int FriendRelationshipRequestInitiator = 4;
    private const int BeginAuthOk = 0;
    private const int BeginAuthInvalidTicket = 1;
    private const int AuthResponseOk = 0;
    private const int AuthResponseInvalidTicket = 8;
    private const int ResultOk = 1;

    private static readonly byte[] DefaultAvatarPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9WnN0E4AAAAASUVORK5CYII=");

    private readonly object _sync = new();
    private readonly string _statePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly Dictionary<string, ApiSession> _sessions = new(StringComparer.Ordinal);
    private readonly Dictionary<uint, ApiTicket> _tickets = new();
    private readonly List<ApiQueuedEvent> _events = new();
    private readonly GameCoordinatorPluginRegistry _gameCoordinatorPlugins;
    private readonly GameCoordinatorTraceService _gameCoordinatorTrace;
    private readonly DotaStatsStore _dotaStatsStore;
    private readonly DotaPartyStore _dotaPartyStore;
    private readonly DotaLobbyInviteStore _dotaLobbyInviteStore;
    private readonly DotaDedicatedServerSupervisor _dotaDedicatedServers;
    private readonly string _advertisedGameServerIp;
    // Auth session lifetime and the (much shorter) presence window used to
    // derive online/offline. Both configurable via appsettings:
    //   "Session:TimeoutMinutes" (default 30), "Presence:TimeoutSeconds" (default 90).
    private readonly TimeSpan _sessionTimeout;
    private readonly TimeSpan _presenceTimeout;

    private ApiState _state = new();
    private long _nextEventSequence = 1;
    private uint _nextTicketHandle = 1000;
    private ulong _nextLobbyId = 90000000000000000;
    private ulong _nextFileShareHandle = 70000000000000000;

    public SteamApiStateService(
        IHostEnvironment hostEnvironment,
        GameCoordinatorPluginRegistry gameCoordinatorPlugins,
        GameCoordinatorTraceService gameCoordinatorTrace,
        IConfiguration configuration,
        DotaDedicatedServerSupervisor dotaDedicatedServers)
    {
        _gameCoordinatorPlugins = gameCoordinatorPlugins;
        _gameCoordinatorTrace = gameCoordinatorTrace;
        _dotaDedicatedServers = dotaDedicatedServers;
        _sessionTimeout = TimeSpan.FromMinutes(Math.Clamp(configuration.GetValue("Session:TimeoutMinutes", 30), 1, 1440));
        _presenceTimeout = TimeSpan.FromSeconds(Math.Clamp(configuration.GetValue("Presence:TimeoutSeconds", 90), 15, 3600));
        _advertisedGameServerIp = configuration.GetValue<string>("GameCoordinator:Dota:AdvertisedGameServerIp")?.Trim() ?? string.Empty;
        _statePath = Path.Combine(hostEnvironment.ContentRootPath, "Data", "api-state.json");
        _dotaStatsStore = new DotaStatsStore(
            Path.Combine(hostEnvironment.ContentRootPath, "Data", "skynet-dota-stats.db"),
            ResolveDotaStatsIdentity);
        _dotaPartyStore = new DotaPartyStore(
            Path.Combine(hostEnvironment.ContentRootPath, "Data", "skynet-dota-party.db"),
            ResolveDotaStatsIdentity);
        _dotaLobbyInviteStore = new DotaLobbyInviteStore(
            Path.Combine(hostEnvironment.ContentRootPath, "Data", "skynet-dota-lobby-invites.db"));
        CleanupOrphanStateTempFiles();
        LoadState();
        NormalizeState();
        SaveState();
        EnsureDefaultAdminAccount();
        DotaGcBackend.StatsStore = _dotaStatsStore;
        DotaGcBackend.PartyStore = _dotaPartyStore;
        DotaGcBackend.LobbyInviteStore = _dotaLobbyInviteStore;
        DotaGcBackend.PendingMessageQueued = EnqueueGcMessageEvent;
        DotaGcBackend.InventoryProvider = GetDotaRuntimeInventory;
        DotaGcBackend.EquipItemSink = EquipDotaItemFromGameCoordinator;
        DotaGcBackend.SetItemStyleSink = SetDotaItemStyleFromGameCoordinator;
        DotaGcBackend.MatchSnapshotSink = UpsertDotaMatchSnapshot;
        DotaGcBackend.MatchSnapshotJsonProvider = GetDotaActiveMatchJson;
        DotaGcBackend.MatchSnapshotByLobbyJsonProvider = GetDotaMatchByLobbyJson;
        DotaGcBackend.MatchSnapshotDeleteSink = RemoveDotaMatchSnapshot;
        DotaGcBackend.UserExistsProvider = IsKnownDotaUser;
        DotaGcBackend.UserOnlineProvider = IsOnlineDotaUser;
        DotaGcBackend.GameServerConnectIpResolver = ResolveDotaGameServerConnectIp;
        DotaGcBackend.GameServerConnectIpsResolver = ResolveDotaGameServerConnectIps;
        DotaGcBackend.DedicatedServerStart = (lobbyId, map) => _dotaDedicatedServers.Start(lobbyId, map);
        DotaGcBackend.DedicatedServerClaim = (gameServerSteamId, port) => _dotaDedicatedServers.ClaimLobby(gameServerSteamId, port);
        DotaGcBackend.DedicatedServerPortReserved = port => _dotaDedicatedServers.HasReservationForPort(port);
        DotaGcBackend.DedicatedServerStatus = lobbyId => _dotaDedicatedServers.GetStatus(lobbyId);
        DotaGcBackend.DedicatedServerRelease = (lobbyId, reason) => _dotaDedicatedServers.Release(lobbyId, reason);
        DotaGcBackend.ItemDefResolver = ResolveDotaItemDefFromGameCoordinator;
        DotaGcBackend.EquipmentJsonProvider = GetDotaEquipmentJson;
        DotaGcBackend.EquipmentJsonSink = SetDotaEquipmentJson;
        DotaGcBackend.CatalogItemJsonProvider = GetDotaCatalogItemJson;
        LuaGameCoordinatorBackend.PendingMessageQueued = EnqueueGcMessageEvent;
    }
}
