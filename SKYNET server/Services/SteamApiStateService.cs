using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;
using SKYNET_server.Persistence;

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
    private readonly Dictionary<string, ApiSession> _sessions = new(StringComparer.Ordinal);
    private readonly Dictionary<uint, ApiTicket> _tickets = new();
    private readonly List<ApiQueuedEvent> _events = new();
    private readonly ILogger<SteamApiStateService> _logger;
    private readonly GameCoordinatorPluginRegistry _gameCoordinatorPlugins;
    private readonly GameCoordinatorTraceService _gameCoordinatorTrace;
    private readonly DotaStatsStore _dotaStatsStore;
    private readonly DotaPartyStore _dotaPartyStore;
    private readonly DotaLobbyInviteStore _dotaLobbyInviteStore;
    private readonly DotaGuildStore _dotaGuildStore;
    private readonly DotaDB _dotaDb;
    private readonly DotaDedicatedServerSupervisor _dotaDedicatedServers;
    private readonly GameServerSettingsService _gameServerSettings;
    // Auth session lifetime and the (much shorter) presence window used to
    // derive online/offline. Both configurable via appsettings:
    //   "Session:TimeoutMinutes" (default 30), "Presence:TimeoutSeconds" (default 90).
    private readonly TimeSpan _sessionTimeout;
    private readonly TimeSpan _presenceTimeout;
    private static readonly DateTime _serverStartTime = DateTime.UtcNow;

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
        DotaDedicatedServerSupervisor dotaDedicatedServers,
        GameServerSettingsService gameServerSettings,
        DotaDB dotaDb,
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<SteamApiStateService> logger)
    {
        _logger = logger;
        _gameCoordinatorPlugins = gameCoordinatorPlugins;
        _gameCoordinatorTrace = gameCoordinatorTrace;
        _dotaDedicatedServers = dotaDedicatedServers;
        _gameServerSettings = gameServerSettings;
        _dotaDb = dotaDb;
        _dbFactory = dbContextFactory;
        _sessionTimeout = TimeSpan.FromMinutes(Math.Clamp(configuration.GetValue("Session:TimeoutMinutes", 30), 1, 1440));
        _presenceTimeout = TimeSpan.FromSeconds(Math.Clamp(configuration.GetValue("Presence:TimeoutSeconds", 90), 15, 3600));
        var dataRoot = ResolveDataRoot(hostEnvironment.ContentRootPath, configuration);
        _statePath = Path.Combine(dataRoot, "api-state.json");
        // All Dota stores now share the consolidated app.db (their tables were
        // copied in by the importer); they keep their raw SQL, just this file.
        var appDbPath = Path.Combine(dataRoot, "app.db");
        _dotaStatsStore = new DotaStatsStore(appDbPath, ResolveDotaStatsIdentity);
        _dotaPartyStore = new DotaPartyStore(appDbPath, ResolveDotaStatsIdentity);
        _dotaLobbyInviteStore = new DotaLobbyInviteStore(appDbPath);
        _dotaGuildStore = new DotaGuildStore(appDbPath, ResolveDotaStatsIdentity);
        InitializePersistence(dataRoot);
        NormalizeState();
        StartBackgroundFlusher();
        RequestStateFlush();
        EnsureDefaultAdminAccount();
        DotaGcRuntimeServices.StatsStore = _dotaStatsStore;
        DotaGcRuntimeServices.PartyStore = _dotaPartyStore;
        DotaGcRuntimeServices.LobbyInviteStore = _dotaLobbyInviteStore;
        DotaGcRuntimeServices.GuildStore = _dotaGuildStore;
        DotaGcRuntimeServices.TeamJsonProvider = teamId => _dotaDb.GetTeam(teamId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        DotaGcRuntimeServices.TeamsForAccountJsonProvider = accountId => _dotaDb.GetTeamsForAccount(accountId);
        DotaGcRuntimeServices.PendingMessageQueued = EnqueueGcMessageEvent;
        DotaGcRuntimeServices.InventoryProvider = GetDotaRuntimeInventory;
        DotaGcRuntimeServices.EquipItemSink = EquipDotaItemFromGameCoordinator;
        DotaGcRuntimeServices.SetItemStyleSink = SetDotaItemStyleFromGameCoordinator;
        DotaGcRuntimeServices.MatchSnapshotSink = UpsertDotaMatchSnapshot;
        DotaGcRuntimeServices.MatchSnapshotJsonProvider = GetDotaActiveMatchJson;
        DotaGcRuntimeServices.MatchSnapshotByLobbyJsonProvider = GetDotaMatchByLobbyJson;
        DotaGcRuntimeServices.MatchSnapshotDeleteSink = RemoveDotaMatchSnapshot;
        DotaGcRuntimeServices.UserExistsProvider = IsKnownDotaUser;
        DotaGcRuntimeServices.UserOnlineProvider = IsOnlineDotaUser;
        DotaGcRuntimeServices.GameServerConnectIpResolver = ResolveDotaGameServerConnectIp;
        DotaGcRuntimeServices.GameServerConnectIpsResolver = ResolveDotaGameServerConnectIps;
        DotaGcRuntimeServices.GameServerChangeRequested = EnqueueGameServerChangeRequestedEvent;
        DotaGcRuntimeServices.DedicatedServerStart = (lobbyId, map) => _dotaDedicatedServers.Start(lobbyId, map);
        DotaGcRuntimeServices.DedicatedServerClaim = (gameServerSteamId, port) => _dotaDedicatedServers.ClaimLobby(gameServerSteamId, port);
        DotaGcRuntimeServices.DedicatedServerPortReserved = port => _dotaDedicatedServers.HasReservationForPort(port);
        DotaGcRuntimeServices.DedicatedServerStatus = lobbyId => _dotaDedicatedServers.GetStatus(lobbyId);
        DotaGcRuntimeServices.DedicatedServerRelease = (lobbyId, reason) => _dotaDedicatedServers.Release(lobbyId, reason);
        DotaGcRuntimeServices.ItemDefResolver = ResolveDotaItemDefFromGameCoordinator;
        DotaGcRuntimeServices.EquipmentJsonProvider = GetDotaEquipmentJson;
        DotaGcRuntimeServices.EquipmentJsonSink = SetDotaEquipmentJson;
        DotaGcRuntimeServices.CatalogItemJsonProvider = GetDotaCatalogItemJson;
    }

    private static string ResolveDataRoot(string contentRootPath, IConfiguration configuration)
    {
        var configuredRoot = configuration.GetValue<string>("Data:Root")?.Trim();
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            configuredRoot = Environment.GetEnvironmentVariable("SKYNET_DATA_ROOT")?.Trim();
        }

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot);
        }

        var root = new DirectoryInfo(Path.GetFullPath(contentRootPath));
        for (var current = root; current != null; current = current.Parent)
        {
            if (File.Exists(Path.Combine(current.FullName, "SKYNET server.csproj")))
            {
                return Path.Combine(current.FullName, "Data");
            }

            var nestedProject = Path.Combine(current.FullName, "SKYNET server", "SKYNET server.csproj");
            if (File.Exists(nestedProject))
            {
                return Path.Combine(current.FullName, "SKYNET server", "Data");
            }
        }

        return Path.Combine(root.FullName, "Data");
    }
}
