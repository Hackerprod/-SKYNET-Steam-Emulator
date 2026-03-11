using System.Text.Json;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed class SteamUiMockService
{
    private readonly string _dataFilePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SteamUiMockService(IHostEnvironment hostEnvironment)
    {
        _dataFilePath = Path.Combine(hostEnvironment.ContentRootPath, "Data", "steam-ui.json");
    }

    public SteamUiSnapshot GetSnapshot()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new SteamUiSnapshot();
        }

        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<SteamUiSnapshot>(json, _jsonOptions) ?? new SteamUiSnapshot();
    }

    public IReadOnlyList<SteamGame> GetGames() => GetSnapshot().Games;

    public IReadOnlyList<SteamFriend> GetFriends() => GetSnapshot().Friends;

    public IReadOnlyList<SteamAchievement> GetAchievements() => GetSnapshot().Achievements;
}
