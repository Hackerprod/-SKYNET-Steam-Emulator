using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Games;

public class IndexModel : PageModel
{
    private readonly SteamUiMockService _steamUiMockService;

    public IReadOnlyList<SteamGame> Games { get; private set; } = Array.Empty<SteamGame>();

    public IndexModel(SteamUiMockService steamUiMockService)
    {
        _steamUiMockService = steamUiMockService;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Juegos";
        Games = _steamUiMockService.GetGames();
    }
}
