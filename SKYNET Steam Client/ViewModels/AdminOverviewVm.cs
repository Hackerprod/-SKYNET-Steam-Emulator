using SKYNET.Client.Models;

namespace SKYNET.Client.ViewModels;

/// <summary>Display-ready wrapper around <see cref="AdminOverview"/> for the Admin view.</summary>
public sealed class AdminOverviewVm
{
    public AdminOverviewVm(AdminOverview data)
    {
        var uptime = DateTime.UtcNow - data.ServerStartTime;
        UptimeText = uptime.TotalDays >= 1
            ? $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m"
            : $"{(int)uptime.TotalHours}h {uptime.Minutes}m";
        StartedAtText = data.ServerStartTime.ToLocalTime().ToString("g");
        HostAddressesText = data.HostAddresses.Count > 0 ? string.Join(", ", data.HostAddresses) : "(none)";

        AdvertisedIp = string.IsNullOrWhiteSpace(data.GameServerSettings.AdvertisedServerIp)
            ? "(not set)"
            : data.GameServerSettings.AdvertisedServerIp;
        DedicatedEnabled = data.GameServerSettings.DedicatedEnabled;
        DedicatedBindIp = data.GameServerSettings.DedicatedBindIp;
        DedicatedPortStart = data.GameServerSettings.DedicatedPortStart;

        Users = data.Users
            .Select(u => new AdminRowVm(
                string.IsNullOrWhiteSpace(u.PersonaName) ? $"#{u.SteamId}" : u.PersonaName,
                string.Equals(u.GameState, "offline", StringComparison.OrdinalIgnoreCase) ? "Offline" : u.GameState))
            .ToList();
        UsersCountText = Users.Count == 1 ? "1 user" : $"{Users.Count} users";

        Lobbies = data.Lobbies
            .Select(l => new AdminRowVm(
                $"Lobby {l.SteamId}",
                $"{l.Members.Count} member(s) - owner {l.OwnerSteamId} - AppID {l.AppId}"))
            .ToList();
        LobbiesCountText = Lobbies.Count == 1 ? "1 active lobby" : $"{Lobbies.Count} active lobbies";

        GameServers = data.GameServers
            .Select(g => new AdminRowVm(
                string.IsNullOrWhiteSpace(g.ServerName) ? $"Server {g.SteamId}" : g.ServerName,
                $"{(g.Dedicated ? "Dedicated" : "Listen")} - map {(string.IsNullOrWhiteSpace(g.MapName) ? "?" : g.MapName)} - port {g.Port} - AppID {g.AppId}"))
            .ToList();
        GameServersCountText = GameServers.Count == 1 ? "1 registered server" : $"{GameServers.Count} registered servers";
    }

    public string UptimeText { get; }
    public string StartedAtText { get; }
    public string HostAddressesText { get; }
    public string AdvertisedIp { get; }
    public bool DedicatedEnabled { get; }
    public string DedicatedBindIp { get; }
    public int DedicatedPortStart { get; }
    public string UsersCountText { get; }
    public string LobbiesCountText { get; }
    public string GameServersCountText { get; }
    public List<AdminRowVm> Users { get; }
    public List<AdminRowVm> Lobbies { get; }
    public List<AdminRowVm> GameServers { get; }
}

public sealed class AdminRowVm
{
    public AdminRowVm(string title, string subtitle)
    {
        Title = title;
        Subtitle = subtitle;
    }

    public string Title { get; }
    public string Subtitle { get; }
}
