using System;
using System.Collections.Generic;

namespace SKYNET_server.Persistence.Entities;

// Relational entities for the server's durable state. Deliberately excludes
// ephemeral, per-connection data (user presence/rich-presence, in-flight game
// sessions, P2P and event queues); that lives in memory and is rebuilt on
// connect. Names are plain domain nouns; the wire DTOs (Models/SteamApiModels)
// stay separate so the HTTP contract is not coupled to the storage schema.

public class UserRecord
{
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public int PlayerLevel { get; set; }
}

public class FriendEdge
{
    public ulong SteamId { get; set; }
    public ulong FriendSteamId { get; set; }
}

public class FriendRequestRecord
{
    public string Id { get; set; } = string.Empty;
    public ulong FromSteamId { get; set; }
    public ulong ToSteamId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public class AvatarRecord
{
    public ulong SteamId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}

public class StatRecord
{
    public ulong SteamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint Data { get; set; }
}

public class AchievementRecord
{
    public ulong SteamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Earned { get; set; }
    public DateTime Date { get; set; }
    public uint Progress { get; set; }
    public uint MaxProgress { get; set; }
}

public class WebAccountRecord
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

// Persistent "remember me" web sessions survive restarts. Transient in-game
// client sessions are kept in memory only and are not represented here.
public class WebSessionRecord
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public string ClientInstanceId { get; set; } = string.Empty;
    public string ProcessRole { get; set; } = "client";
    public string? RemoteIp { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public bool WebSession { get; set; }
    public bool Persistent { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}

public class GameServerRecord
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public uint IP { get; set; }
    public int Port { get; set; }
    public int QueryPort { get; set; }
    public uint Flags { get; set; }
    public byte Secure { get; set; }
    public string VersionString { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModDir { get; set; } = string.Empty;
    public bool Dedicated { get; set; }
    public int MaxPlayers { get; set; }
    public int BotPlayers { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public string MapName { get; set; } = string.Empty;
    public bool PasswordProtected { get; set; }
    public uint SpectatorPort { get; set; }
    public string SpectatorServerName { get; set; } = string.Empty;
    public string GameTags { get; set; } = string.Empty;
    public string GameData { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public Dictionary<string, string> KeyValues { get; set; } = new();
}

public class LobbyRecord
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public int LobbyType { get; set; }
    public int MaxMembers { get; set; }
    public bool Joinable { get; set; } = true;
    public Dictionary<string, string> LobbyData { get; set; } = new();
    public List<LobbyMemberValue> Members { get; set; } = new();
    public LobbyGameServerValue? GameServer { get; set; }
}

public class LobbyMemberValue
{
    public ulong SteamId { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}

public class LobbyGameServerValue
{
    public ulong SteamId { get; set; }
    public uint IP { get; set; }
    public uint Port { get; set; }
}

public class RemoteFileRecord
{
    public ulong OwnerSteamId { get; set; }
    public uint AppId { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public int Size { get; set; }
    public uint Timestamp { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public long SyncPlatforms { get; set; }
    public int Version { get; set; }
    public bool Persisted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class RemoteFileShareRecord
{
    public ulong Handle { get; set; }
    public ulong OwnerSteamId { get; set; }
    public uint AppId { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
}

public class DotaItemRecord
{
    public uint DefIndex { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Prefab { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
    public uint QualityId { get; set; }
    public string Rarity { get; set; } = string.Empty;
    public uint RarityId { get; set; }
    public string ImageInventory { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsTool { get; set; }
    public bool IsBundle { get; set; }
    public List<string> HeroNames { get; set; } = new();
    public List<uint> HeroIds { get; set; } = new();
}

public class DotaEquipmentRecord
{
    public ulong SteamId { get; set; }
    public uint HeroId { get; set; }
    public uint SlotId { get; set; }
    public string HeroName { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public uint DefIndex { get; set; }
    public ulong ItemId { get; set; }
    public uint Style { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Keyed by LobbyId: the live-match cache is one entry per lobby (MatchId can be
// 0/unassigned or shared, so it is a plain column, not the key).
public class DotaMatchRecord
{
    public ulong LobbyId { get; set; }
    public ulong MatchId { get; set; }
    public ulong ServerSteamId { get; set; }
    public string Connect { get; set; } = string.Empty;
    public uint State { get; set; }
    public uint GameState { get; set; }
    public uint GameStartTime { get; set; }
    public bool Dedicated { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DotaMatchPlayerRecord> Players { get; set; } = new();
}

public class DotaMatchPlayerRecord
{
    public ulong LobbyId { get; set; }
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint Team { get; set; }
    public uint Slot { get; set; }
    public uint CoachTeam { get; set; }
    public uint HeroId { get; set; }
    public List<DotaEquipmentValue> Equipment { get; set; } = new();
    public DotaMatchRecord? Match { get; set; }
}

// Equipment snapshot embedded in a completed match (stored as JSON on the
// match-player row; independent of the live DotaEquipment table).
public class DotaEquipmentValue
{
    public ulong SteamId { get; set; }
    public uint HeroId { get; set; }
    public string HeroName { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public uint SlotId { get; set; }
    public uint DefIndex { get; set; }
    public ulong ItemId { get; set; }
    public uint Style { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Single-row table (Id == 1) holding the Dota cosmetics import settings.
public class CosmeticSettingsRecord
{
    public int Id { get; set; } = 1;
    public string DotaPath { get; set; } = string.Empty;
    public DateTime LastImportAt { get; set; }
    public string LastImportStatus { get; set; } = string.Empty;
    public ulong EquipmentVersion { get; set; }
}

// Single-row table (Id == 1) for durable server-level scalars that don't belong
// to any aggregate (e.g. the active web user).
public class AppStateRecord
{
    public int Id { get; set; } = 1;
    public ulong ActiveWebSteamId { get; set; }
}

public class DotaHeroIdRecord
{
    public string Name { get; set; } = string.Empty;
    public uint HeroId { get; set; }
}

public class DotaHeroSlotRecord
{
    public string HeroName { get; set; } = string.Empty;
    public string SlotName { get; set; } = string.Empty;
    public uint SlotId { get; set; }
}
