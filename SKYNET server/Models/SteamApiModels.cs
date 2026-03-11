using System.Collections.Concurrent;

namespace SKYNET_server.Models;

public sealed class SkyNetSessionRequestDto
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
}

public sealed class SkyNetSessionDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public SkyNetUserDto User { get; set; } = new();
}

public sealed class SkyNetUserDto
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public ulong LobbyId { get; set; }
    public bool HasFriend { get; set; }
    public int PersonaState { get; set; }
    public int PlayerLevel { get; set; }
    public Dictionary<string, string> RichPresence { get; set; } = new();
}

public sealed class SkyNetPersonaUpdateDto
{
    public string PersonaName { get; set; } = string.Empty;
}

public sealed class SkyNetPresenceUpdateDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class SkyNetStatDto
{
    public string Name { get; set; } = string.Empty;
    public uint Data { get; set; }
}

public sealed class SkyNetAchievementDto
{
    public string Name { get; set; } = string.Empty;
    public bool Earned { get; set; }
    public DateTime Date { get; set; }
    public uint Progress { get; set; }
    public uint MaxProgress { get; set; }
}

public sealed class SkyNetStatsEnvelopeDto
{
    public ulong SteamId { get; set; }
    public List<SkyNetStatDto> Stats { get; set; } = new();
    public List<SkyNetAchievementDto> Achievements { get; set; } = new();
    public int CurrentPlayers { get; set; }
}

public sealed class SkyNetStoreStatsRequestDto
{
    public ulong SteamId { get; set; }
    public List<SkyNetStatDto> Stats { get; set; } = new();
    public List<SkyNetAchievementDto> Achievements { get; set; } = new();
}

public sealed class SkyNetEventEnvelopeDto
{
    public string Cursor { get; set; } = string.Empty;
    public List<SkyNetEventDto> Events { get; set; } = new();
}

public sealed class SkyNetEventDto
{
    public string Type { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public ulong LobbyId { get; set; }
    public int ChangeFlags { get; set; }
    public Dictionary<string, string> RichPresence { get; set; } = new();
    public string StatName { get; set; } = string.Empty;
    public uint StatValue { get; set; }
    public string AchievementName { get; set; } = string.Empty;
    public bool AchievementEarned { get; set; }
    public uint AchievementProgress { get; set; }
    public uint AchievementMaxProgress { get; set; }
    public SkyNetLobbyDto? Lobby { get; set; }
    public string PayloadBase64 { get; set; } = string.Empty;
    public uint MessageType { get; set; }
    public int Channel { get; set; }
    public ulong RemoteSteamId { get; set; }
}

public sealed class SkyNetLobbyQueryRequestDto
{
    public uint AppId { get; set; }
    public int Distance { get; set; }
    public int SlotsAvailable { get; set; }
    public string? KeyToMatch { get; set; }
    public int ValueToMatch { get; set; }
    public int ComparisonType { get; set; }
    public string? StringValueToMatch { get; set; }
}

public sealed class SkyNetCreateLobbyRequestDto
{
    public uint AppId { get; set; }
    public int LobbyType { get; set; }
    public int MaxMembers { get; set; }
    public Dictionary<string, string> LobbyData { get; set; } = new();
}

public sealed class SkyNetLobbyDataUpdateRequestDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class SkyNetLobbyDeleteDataRequestDto
{
    public string Key { get; set; } = string.Empty;
}

public sealed class SkyNetLobbySettingsUpdateRequestDto
{
    public bool? Joinable { get; set; }
    public int? LobbyType { get; set; }
    public ulong? OwnerSteamId { get; set; }
    public int? MaxMembers { get; set; }
}

public sealed class SkyNetLobbyGameServerUpdateRequestDto
{
    public ulong SteamIdGameServer { get; set; }
    public uint IP { get; set; }
    public uint Port { get; set; }
}

public sealed class SkyNetLobbyDto
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public int LobbyType { get; set; }
    public int MaxMembers { get; set; }
    public bool Joinable { get; set; } = true;
    public Dictionary<string, string> LobbyData { get; set; } = new();
    public List<SkyNetLobbyMemberDto> Members { get; set; } = new();
    public SkyNetLobbyGameServerDto? GameServer { get; set; }
}

public sealed class SkyNetLobbyMemberDto
{
    public ulong SteamId { get; set; }
    public List<SkyNetLobbyMetaDataDto> Data { get; set; } = new();
}

public sealed class SkyNetLobbyMetaDataDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class SkyNetLobbyGameServerDto
{
    public ulong SteamId { get; set; }
    public uint IP { get; set; }
    public uint Port { get; set; }
}

public sealed class SkyNetRemoteStorageFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentBase64 { get; set; } = string.Empty;
    public int Size { get; set; }
    public uint Timestamp { get; set; }
}

public sealed class SkyNetRemoteStorageFileListItemDto
{
    public string FileName { get; set; } = string.Empty;
    public int Size { get; set; }
    public uint Timestamp { get; set; }
}

public sealed class SkyNetRemoteStorageDeleteRequestDto
{
    public string FileName { get; set; } = string.Empty;
}

public sealed class SkyNetRemoteStorageShareDto
{
    public ulong Handle { get; set; }
    public int Result { get; set; }
}

public sealed class SkyNetRemoteStorageQuotaDto
{
    public ulong TotalBytes { get; set; }
    public ulong AvailableBytes { get; set; }
}

public sealed class SkyNetAuthTicketRequestDto
{
    public uint AppId { get; set; }
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
    public int TicketBufferSize { get; set; }
}

public sealed class SkyNetAuthTicketDto
{
    public uint Handle { get; set; }
    public string TicketBase64 { get; set; } = string.Empty;
    public uint TicketSize { get; set; }
}

public sealed class SkyNetAuthValidateRequestDto
{
    public ulong SteamId { get; set; }
    public string TicketBase64 { get; set; } = string.Empty;
    public bool GameServer { get; set; }
    public uint AppId { get; set; }
}

public sealed class SkyNetAuthValidateResultDto
{
    public int BeginAuthSessionResult { get; set; }
    public int AuthSessionResponse { get; set; }
    public ulong OwnerSteamId { get; set; }
    public bool Success { get; set; }
}

public sealed class SkyNetConnectAuthRequestDto
{
    public uint IpClient { get; set; }
    public ulong SteamId { get; set; }
    public string AuthBlobBase64 { get; set; } = string.Empty;
    public uint AppId { get; set; }
}

public sealed class SkyNetConnectAuthResultDto
{
    public bool Success { get; set; }
    public ulong SteamId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public int DenyReason { get; set; }
    public string DenyMessage { get; set; } = string.Empty;
}

public sealed class SkyNetAuthEndSessionRequestDto
{
    public ulong SteamId { get; set; }
    public bool GameServer { get; set; }
}

public sealed class SkyNetCancelAuthTicketRequestDto
{
    public uint Handle { get; set; }
    public bool GameServer { get; set; }
}

public sealed class SkyNetGameServerStateDto
{
    public SkyNetGameServerDto? Server { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool Anonymous { get; set; }
}

public sealed class SkyNetGameServerResultDto
{
    public bool Success { get; set; }
    public uint PublicIP { get; set; }
    public byte Secure { get; set; }
    public ulong SteamId { get; set; }
}

public sealed class SkyNetGameServerPublicIpDto
{
    public uint PublicIP { get; set; }
}

public sealed class SkyNetGameServerUserDataDto
{
    public ulong SteamId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public uint Score { get; set; }
}

public sealed class SkyNetDisconnectGameServerUserDto
{
    public ulong SteamId { get; set; }
}

public sealed class SkyNetGameServerDto
{
    public uint AppId { get; set; }
    public uint IP { get; set; }
    public int Port { get; set; }
    public int QueryPort { get; set; }
    public uint Flags { get; set; }
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

public sealed class SkyNetP2PPacketSendDto
{
    public ulong RemoteSteamId { get; set; }
    public string BufferBase64 { get; set; } = string.Empty;
    public int SendType { get; set; }
    public int Channel { get; set; }
}

public sealed class SkyNetGCMessageDto
{
    public uint MessageType { get; set; }
    public string PayloadBase64 { get; set; } = string.Empty;
}

public sealed class ApiState
{
    public Dictionary<ulong, SkyNetUserDto> Users { get; set; } = new();
    public Dictionary<ulong, HashSet<ulong>> FriendLinks { get; set; } = new();
    public Dictionary<ulong, SkyNetStatsEnvelopeDto> Stats { get; set; } = new();
    public Dictionary<ulong, SkyNetLobbyDto> Lobbies { get; set; } = new();
    public Dictionary<string, SkyNetRemoteStorageFileDto> Files { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<ulong, SkyNetGameServerDto> GameServers { get; set; } = new();
}

public sealed class ApiSession
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ulong SteamId { get; set; }
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
    public SkyNetEventDto Event { get; set; } = new();
}
