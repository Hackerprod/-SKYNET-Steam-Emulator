using System.Net;
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
        IConfiguration configuration)
    {
        _gameCoordinatorPlugins = gameCoordinatorPlugins;
        _gameCoordinatorTrace = gameCoordinatorTrace;
        _sessionTimeout = TimeSpan.FromMinutes(Math.Clamp(configuration.GetValue("Session:TimeoutMinutes", 30), 1, 1440));
        _presenceTimeout = TimeSpan.FromSeconds(Math.Clamp(configuration.GetValue("Presence:TimeoutSeconds", 90), 15, 3600));
        _statePath = Path.Combine(hostEnvironment.ContentRootPath, "Data", "api-state.json");
        _dotaStatsStore = new DotaStatsStore(
            Path.Combine(hostEnvironment.ContentRootPath, "Data", "skynet-dota-stats.db"),
            ResolveDotaStatsIdentity);
        _dotaPartyStore = new DotaPartyStore(
            Path.Combine(hostEnvironment.ContentRootPath, "Data", "skynet-dota-party.db"),
            ResolveDotaStatsIdentity);
        CleanupOrphanStateTempFiles();
        LoadState();
        NormalizeState();
        SaveState();
        EnsureDefaultAdminAccount();
        DotaGcBackend.StatsStore = _dotaStatsStore;
        DotaGcBackend.PartyStore = _dotaPartyStore;
        DotaGcBackend.PendingMessageQueued = EnqueueGcMessageEvent;
        DotaGcBackend.InventoryProvider = GetDotaRuntimeInventory;
        DotaGcBackend.EquipItemSink = EquipDotaItemFromGameCoordinator;
        DotaGcBackend.SetItemStyleSink = SetDotaItemStyleFromGameCoordinator;
        DotaGcBackend.MatchSnapshotSink = UpsertDotaMatchSnapshot;
        DotaGcBackend.MatchSnapshotJsonProvider = GetDotaActiveMatchJson;
        DotaGcBackend.MatchSnapshotByLobbyJsonProvider = GetDotaMatchByLobbyJson;
        DotaGcBackend.MatchSnapshotDeleteSink = RemoveDotaMatchSnapshot;
        DotaGcBackend.ItemDefResolver = ResolveDotaItemDefFromGameCoordinator;
        DotaGcBackend.EquipmentJsonProvider = GetDotaEquipmentJson;
        DotaGcBackend.EquipmentJsonSink = SetDotaEquipmentJson;
        DotaGcBackend.CatalogItemJsonProvider = GetDotaCatalogItemJson;
        LuaGameCoordinatorBackend.PendingMessageQueued = EnqueueGcMessageEvent;
    }
}
