using System.Collections.Concurrent;

namespace SKYNET_server.Models;

public sealed class ApiSessionRequest
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public string ClientInstanceId { get; set; } = string.Empty;
    public string ProcessRole { get; set; } = "client";
    public bool UseActiveWebUser { get; set; }
}

public sealed class ApiSessionResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ApiUser User { get; set; } = new();
}

public sealed class ApiAvatarContent
{
    public ulong SteamId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public bool IsDefault { get; set; }
    public string ETag { get; set; } = string.Empty;
}

public sealed class ApiUser
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public ulong LobbyId { get; set; }
    public bool HasFriend { get; set; }
    public int FriendRelationship { get; set; }
    public int PersonaState { get; set; }
    public int PlayerLevel { get; set; }
    // Derived, viewer-facing presence detail: "offline" | "menu" | "in_match".
    // HeroId is the hero the user is playing when in_match (0 otherwise).
    public string GameState { get; set; } = "offline";
    public uint HeroId { get; set; }
    public Dictionary<string, string> RichPresence { get; set; } = new();
}

public sealed class ApiPersonaUpdate
{
    public string PersonaName { get; set; } = string.Empty;
}

public sealed class ApiPresenceUpdate
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class ApiAvatarUpdate
{
    public string ContentBase64 { get; set; } = string.Empty;
}

public sealed class ApiStat
{
    public string Name { get; set; } = string.Empty;
    public uint Data { get; set; }
}

public sealed class ApiAchievement
{
    public string Name { get; set; } = string.Empty;
    public bool Earned { get; set; }
    public DateTime Date { get; set; }
    public uint Progress { get; set; }
    public uint MaxProgress { get; set; }
}

public sealed class ApiStatsEnvelope
{
    public ulong SteamId { get; set; }
    public List<ApiStat> Stats { get; set; } = new();
    public List<ApiAchievement> Achievements { get; set; } = new();
    public int CurrentPlayers { get; set; }
}

public sealed class ApiWebAccount
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public sealed class ApiWebAccountView
{
    public string Username { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public sealed class ApiWebLoginResult
{
    public string AccessToken { get; set; } = string.Empty;
    public ApiUser User { get; set; } = new();
    public bool IsAdmin { get; set; }
}

public sealed class ApiAdminOverview
{
    public List<ApiUser> Users { get; set; } = new();
    public List<ApiWebAccountView> Accounts { get; set; } = new();
    public List<ApiLobby> Lobbies { get; set; } = new();
    public List<ApiGameServer> GameServers { get; set; } = new();
    public int FriendLinks { get; set; }
    public int PendingFriendRequests { get; set; }
    public int StatsProfiles { get; set; }
    public ApiDotaCosmeticSummary DotaCosmetics { get; set; } = new();
    public List<ApiDotaMatch> DotaMatches { get; set; } = new();
    public DateTime ServerStartTime { get; set; }
    public ApiGameServerSettings GameServerSettings { get; set; } = new();
    public List<string> HostAddresses { get; set; } = new();
}

public sealed class ApiGameServerSettings
{
    public string AdvertisedGameServerIp { get; set; } = string.Empty;
    public bool DedicatedEnabled { get; set; } = true;
    public string DedicatedBindIp { get; set; } = "0.0.0.0";
    public int DedicatedPortStart { get; set; } = 27025;
}

public sealed class ApiGameServerSettingsResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ApiGameServerSettings Settings { get; set; } = new();
}

public sealed class ApiFriendRequest
{
    public string Id { get; set; } = string.Empty;
    public ulong FromSteamId { get; set; }
    public ulong ToSteamId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public sealed class ApiFriendRequestView
{
    public string Id { get; set; } = string.Empty;
    public ApiUser FromUser { get; set; } = new();
    public ApiUser ToUser { get; set; } = new();
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
}

public sealed class ApiUserProfile
{
    public ApiUser User { get; set; } = new();
    public ApiStatsEnvelope Stats { get; set; } = new();
    public int FriendRelationship { get; set; }
    public bool IsSelf { get; set; }
    public ApiFriendRequestView? IncomingRequest { get; set; }
    public ApiFriendRequestView? OutgoingRequest { get; set; }
}

public sealed class ApiFriendActionRequest
{
    public ulong SteamId { get; set; }
    public string Identifier { get; set; } = string.Empty;
}

public sealed class ApiStoreStatsRequest
{
    public ulong SteamId { get; set; }
    public List<ApiStat> Stats { get; set; } = new();
    public List<ApiAchievement> Achievements { get; set; } = new();
}

public sealed class ApiEventEnvelope
{
    public string Cursor { get; set; } = string.Empty;
    public List<ApiEvent> Events { get; set; } = new();
}

public sealed class ApiEvent
{
    public string Type { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public ulong LobbyId { get; set; }
    public int PersonaState { get; set; }
    public int ChangeFlags { get; set; }
    public Dictionary<string, string> RichPresence { get; set; } = new();
    public string StatName { get; set; } = string.Empty;
    public uint StatValue { get; set; }
    public string AchievementName { get; set; } = string.Empty;
    public bool AchievementEarned { get; set; }
    public uint AchievementProgress { get; set; }
    public uint AchievementMaxProgress { get; set; }
    public ApiLobby? Lobby { get; set; }
    public string PayloadBase64 { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public ulong? TargetJobId { get; set; }
    public bool Protobuf { get; set; }
    public int Channel { get; set; }
    public ulong RemoteSteamId { get; set; }
    public int FriendRelationship { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class ApiLobbyQueryRequest
{
    public uint AppId { get; set; }
    public int Distance { get; set; }
    public int SlotsAvailable { get; set; }
    public string? KeyToMatch { get; set; }
    public int ValueToMatch { get; set; }
    public int ComparisonType { get; set; }
    public string? StringValueToMatch { get; set; }
}

public sealed class ApiCreateLobbyRequest
{
    public uint AppId { get; set; }
    public int LobbyType { get; set; }
    public int MaxMembers { get; set; }
    public Dictionary<string, string> LobbyData { get; set; } = new();
}

public sealed class ApiLobbyDataUpdateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class ApiLobbyDeleteDataRequest
{
    public string Key { get; set; } = string.Empty;
}

public sealed class ApiLobbySettingsUpdateRequest
{
    public bool? Joinable { get; set; }
    public int? LobbyType { get; set; }
    public ulong? OwnerSteamId { get; set; }
    public int? MaxMembers { get; set; }
}

public sealed class ApiLobbyChatRequest
{
    public string MessageBase64 { get; set; } = string.Empty;
}

public sealed class ApiLobbyGameServerUpdateRequest
{
    public ulong SteamIdGameServer { get; set; }
    public uint IP { get; set; }
    public uint Port { get; set; }
}

public sealed class ApiLobby
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public int LobbyType { get; set; }
    public int MaxMembers { get; set; }
    public bool Joinable { get; set; } = true;
    public Dictionary<string, string> LobbyData { get; set; } = new();
    public List<ApiLobbyMember> Members { get; set; } = new();
    public ApiLobbyGameServer? GameServer { get; set; }
}

public sealed class ApiLobbyMember
{
    public ulong SteamId { get; set; }
    public List<ApiLobbyMetaData> Data { get; set; } = new();
}

public sealed class ApiLobbyMetaData
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class ApiLobbyGameServer
{
    public ulong SteamId { get; set; }
    public uint IP { get; set; }
    public uint Port { get; set; }
}

public sealed class ApiRemoteStorageFile
{
    public ulong OwnerSteamId { get; set; }
    public uint AppId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentBase64 { get; set; } = string.Empty;
    public int Size { get; set; }
    public uint Timestamp { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public uint SyncPlatforms { get; set; }
    public int Version { get; set; }
    public bool Persisted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public sealed class ApiRemoteStorageUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentBase64 { get; set; } = string.Empty;
    public uint? SyncPlatforms { get; set; }
}

public sealed class ApiRemoteStorageFileListItem
{
    public string FileName { get; set; } = string.Empty;
    public int Size { get; set; }
    public uint Timestamp { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public int Version { get; set; }
}

public sealed class ApiRemoteStorageDeleteRequest
{
    public string FileName { get; set; } = string.Empty;
}

public sealed class ApiRemoteStorageShare
{
    public ulong Handle { get; set; }
    public int Result { get; set; }
}

public sealed class ApiRemoteStorageQuota
{
    public ulong TotalBytes { get; set; }
    public ulong AvailableBytes { get; set; }
}

public sealed class ApiAuthTicketRequest
{
    public uint AppId { get; set; }
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
    public int TicketBufferSize { get; set; }
}

public sealed class ApiAuthTicket
{
    public uint Handle { get; set; }
    public string TicketBase64 { get; set; } = string.Empty;
    public uint TicketSize { get; set; }
}

public sealed class ApiAuthValidateRequest
{
    public ulong SteamId { get; set; }
    public string TicketBase64 { get; set; } = string.Empty;
    public bool GameServer { get; set; }
    public uint AppId { get; set; }
}

public sealed class ApiAuthValidateResult
{
    public int BeginAuthSessionResult { get; set; }
    public int AuthSessionResponse { get; set; }
    public ulong OwnerSteamId { get; set; }
    public bool Success { get; set; }
}

public sealed class ApiConnectAuthRequest
{
    public uint IpClient { get; set; }
    public ulong SteamId { get; set; }
    public string AuthBlobBase64 { get; set; } = string.Empty;
    public uint AppId { get; set; }
}

public sealed class ApiConnectAuthResult
{
    public bool Success { get; set; }
    public ulong SteamId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public int DenyReason { get; set; }
    public string DenyMessage { get; set; } = string.Empty;
}

public sealed class ApiAuthEndSessionRequest
{
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
}

public sealed class ApiCancelAuthTicketRequest
{
    public uint Handle { get; set; }
    public bool GameServer { get; set; }
}

public sealed class ApiGameServerState
{
    public ApiGameServer? Server { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool Anonymous { get; set; }
}

public sealed class ApiGameServerResult
{
    public bool Success { get; set; }
    public uint PublicIP { get; set; }
    public byte Secure { get; set; }
    public ulong SteamId { get; set; }
}

public sealed class ApiGameServerPublicIp
{
    public uint PublicIP { get; set; }
}

public sealed class ApiGameServerUserData
{
    public ulong SteamId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public uint Score { get; set; }
}

public sealed class ApiDisconnectGameServerUser
{
    public ulong SteamId { get; set; }
}

public sealed class ApiGameServer
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

public sealed class ApiP2PPacketSend
{
    public ulong RemoteSteamId { get; set; }
    public string BufferBase64 { get; set; } = string.Empty;
    public int SendType { get; set; }
    public int Channel { get; set; }
}

public sealed class ApiP2PPacketBatch
{
    public List<ApiP2PPacketSend> Packets { get; set; } = new();
}

public sealed class ApiGCMessage
{
    public uint AppId { get; set; }
    public uint MessageType { get; set; }
    public string PayloadBase64 { get; set; } = string.Empty;
    public ulong? TargetJobId { get; set; }
    public bool Protobuf { get; set; }
}

public sealed class ApiSdrCa
{
    public string CaPublicKeyBase64 { get; set; } = string.Empty;
    public ulong CaKeyId { get; set; }
}

public sealed class ApiSdrCertRequest
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
}

public sealed class ApiSdrCert
{
    public string CertBase64 { get; set; } = string.Empty;
    public string SignatureBase64 { get; set; } = string.Empty;
    public string PrivateKeyBase64 { get; set; } = string.Empty;
    public string PublicKeyBase64 { get; set; } = string.Empty;
    public ulong CaKeyId { get; set; }
}

public sealed class ApiGCExchangeRequest
{
    public uint AppId { get; set; }
    public uint MessageType { get; set; }
    public string BodyBase64 { get; set; } = string.Empty;
    public ulong? SourceJobId { get; set; }
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
}

public sealed class ApiGCPollRequest
{
    public uint AppId { get; set; }
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
}

public sealed class ApiGCExchangeResponse
{
    public bool Handled { get; set; }
    public List<ApiGCMessage> Messages { get; set; } = new();
}

public sealed class ApiDotaItem
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

public sealed class ApiDotaEquipment
{
    public ulong SteamId { get; set; }
    public uint HeroId { get; set; }
    public string HeroName { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public uint SlotId { get; set; }
    public uint DefIndex { get; set; }
    public ulong ItemId { get; set; }
    public uint Style { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class ApiDotaCosmeticSettings
{
    public string DotaPath { get; set; } = string.Empty;
    public DateTime LastImportAt { get; set; }
    public string LastImportStatus { get; set; } = string.Empty;
    public ulong EquipmentVersion { get; set; }
}

public sealed class ApiDotaCosmeticSummary
{
    public int ItemCount { get; set; }
    public int HeroCount { get; set; }
    public int EquippedCount { get; set; }
    public string DotaPath { get; set; } = string.Empty;
    public DateTime LastImportAt { get; set; }
    public string LastImportStatus { get; set; } = string.Empty;
}

public sealed class ApiDotaCosmeticOverview
{
    public ApiDotaCosmeticSummary Summary { get; set; } = new();
    public List<ApiDotaItem> Items { get; set; } = new();
    public List<ApiDotaEquipment> Equipment { get; set; } = new();
    public List<ApiUser> Users { get; set; } = new();
}

public sealed class ApiDotaImportRequest
{
    public string DotaPath { get; set; } = string.Empty;
}

public sealed class ApiDotaItemImportResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int HeroCount { get; set; }
    public string SourcePath { get; set; } = string.Empty;
}

public sealed class ApiDotaEquipItemRequest
{
    public ulong SteamId { get; set; }
    public uint HeroId { get; set; }
    public string HeroName { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public uint SlotId { get; set; }
    public uint DefIndex { get; set; }
    public uint Style { get; set; }
}

public sealed class ApiDotaRuntimeInventory
{
    public ulong SteamId { get; set; }
    public List<ApiDotaItem> Items { get; set; } = new();
    public List<ApiDotaItem> OwnedItems { get; set; } = new();
    public List<ApiDotaEquipment> Equipment { get; set; } = new();
    public ulong Version { get; set; }
}

public sealed class ApiDotaMatch
{
    public ulong LobbyId { get; set; }
    public ulong MatchId { get; set; }
    public ulong ServerSteamId { get; set; }
    public string Connect { get; set; } = string.Empty;
    public uint State { get; set; }
    public uint GameState { get; set; }
    public uint GameStartTime { get; set; }
    public bool Dedicated { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<ApiDotaMatchPlayer> Players { get; set; } = new();
}

public sealed class ApiDotaMatchPlayer
{
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint Team { get; set; }
    public uint Slot { get; set; }
    public uint CoachTeam { get; set; }
    public uint HeroId { get; set; }
    public List<ApiDotaEquipment> Equipment { get; set; } = new();
}

public sealed class ApiState
{
    public Dictionary<ulong, ApiUser> Users { get; set; } = new();
    public Dictionary<ulong, HashSet<ulong>> FriendLinks { get; set; } = new();
    public List<ApiFriendRequest> FriendRequests { get; set; } = new();
    public Dictionary<ulong, string> Avatars { get; set; } = new();
    public Dictionary<ulong, ApiStatsEnvelope> Stats { get; set; } = new();
    public Dictionary<ulong, ApiLobby> Lobbies { get; set; } = new();
    public Dictionary<string, ApiRemoteStorageFile> Files { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<ulong, ApiRemoteStorageShareRecord> FileShares { get; set; } = new();
    public Dictionary<ulong, ApiGameServer> GameServers { get; set; } = new();
    public Dictionary<string, ApiWebAccount> WebAccounts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, ApiSession> WebSessions { get; set; } = new(StringComparer.Ordinal);
    public ulong ActiveWebSteamId { get; set; }
    public Dictionary<uint, ApiDotaItem> DotaItems { get; set; } = new();
    public Dictionary<ulong, List<ApiDotaEquipment>> DotaEquipment { get; set; } = new();
    public Dictionary<ulong, ApiDotaMatch> DotaMatches { get; set; } = new();
    public Dictionary<string, uint> DotaHeroIds { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dictionary<string, uint>> DotaHeroSlots { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public ApiDotaCosmeticSettings DotaCosmetics { get; set; } = new();
}

public sealed class ApiRemoteStorageShareRecord
{
    public ulong Handle { get; set; }
    public ulong OwnerSteamId { get; set; }
    public uint AppId { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
}

public sealed class ApiSession
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public string ClientInstanceId { get; set; } = string.Empty;
    public string ProcessRole { get; set; } = "client";
    public string? RemoteIp { get; set; }
    public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
    public bool WebSession { get; set; }
    public bool Persistent { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed class ApiTicket
{
    public uint Handle { get; set; }
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public bool GameServer { get; set; }
    public string TicketBase64 { get; set; } = string.Empty;
}

public sealed class ApiQueuedEvent
{
    public long Sequence { get; set; }
    public ulong RecipientSteamId { get; set; }
    public string RecipientProcessRole { get; set; } = string.Empty;
    public string RecipientClientInstanceId { get; set; } = string.Empty;
    public ApiEvent Event { get; set; } = new();
}
