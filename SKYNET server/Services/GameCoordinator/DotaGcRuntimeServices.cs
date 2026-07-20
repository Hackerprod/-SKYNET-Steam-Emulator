using SKYNET_server.Models;

namespace SKYNET_server.Services;

/// <summary>
/// Host-side services exposed to the TypeScript Dota GC runtime.
/// This class is deliberately limited to data stores and callbacks; protocol
/// handling remains in GC/570 TypeScript modules.
/// </summary>
public static class DotaGcRuntimeServices
{
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
    public static Action<ulong, string, string>? GameServerChangeRequested { get; set; }
    public static Func<ulong, string, DotaDedicatedServerSupervisor.DedicatedLaunchResult>? DedicatedServerStart { get; set; }
    public static Func<ulong, uint, string>? DedicatedServerClaim { get; set; }
    public static Func<uint, bool>? DedicatedServerPortReserved { get; set; }
    public static Func<ulong, string>? DedicatedServerStatus { get; set; }
    public static Func<ulong, string, bool>? DedicatedServerRelease { get; set; }
    public static DotaStatsStore? StatsStore { get; set; }
    public static DotaPartyStore? PartyStore { get; set; }
    public static DotaLobbyInviteStore? LobbyInviteStore { get; set; }
    public static DotaGuildStore? GuildStore { get; set; }
    public static DotaChatStore ChatStore { get; set; } = new();

    public static Func<ulong, ulong, uint>? ItemDefResolver { get; set; }
    public static Func<ulong, string>? EquipmentJsonProvider { get; set; }
    public static Func<ulong, string, bool>? EquipmentJsonSink { get; set; }
    public static Func<uint, string>? CatalogItemJsonProvider { get; set; }
}
