using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Achievements;

public class IndexModel : PageModel
{
    private readonly SteamUiMockService _steamUiMockService;

    public IReadOnlyList<SteamAchievement> Achievements { get; private set; } = Array.Empty<SteamAchievement>();

    public IndexModel(SteamUiMockService steamUiMockService)
    {
        _steamUiMockService = steamUiMockService;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Achievements";
        Achievements = _steamUiMockService.GetAchievements();
    }
}
