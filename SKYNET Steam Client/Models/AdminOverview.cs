namespace SKYNET.Client.Models;

/// <summary>Snapshot shown in the client's Admin section. Mirrors the subset of
/// the server's ApiAdminOverview this view actually displays.</summary>
public sealed class AdminOverview
{
    public DateTime ServerStartTime { get; set; }
    public List<AdminUser> Users { get; set; } = new();
    public List<AdminLobby> Lobbies { get; set; } = new();
    public List<AdminGameServer> GameServers { get; set; } = new();
    public AdminGameServerSettings GameServerSettings { get; set; } = new();
    public List<string> HostAddresses { get; set; } = new();
}

public sealed class AdminUser
{
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = "";
    public string GameState { get; set; } = "offline";
}

public sealed class AdminLobby
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public List<AdminLobbyMember> Members { get; set; } = new();
}

public sealed class AdminLobbyMember
{
    public ulong SteamId { get; set; }
}

public sealed class AdminGameServer
{
    public ulong SteamId { get; set; }
    public uint AppId { get; set; }
    public string ServerName { get; set; } = "";
    public string MapName { get; set; } = "";
    public int MaxPlayers { get; set; }
    public bool Dedicated { get; set; }
    public int Port { get; set; }
}

public sealed class AdminGameServerSettings
{
    public string AdvertisedServerIp { get; set; } = "";
    public bool DedicatedEnabled { get; set; }
    public string DedicatedBindIp { get; set; } = "";
    public int DedicatedPortStart { get; set; }
}
