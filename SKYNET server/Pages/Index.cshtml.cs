using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages;

public class IndexModel : PageModel
{
    private readonly SteamUiMockService _steamUiMockService;

    public SteamUiSnapshot Snapshot { get; private set; } = new();

    public IndexModel(SteamUiMockService steamUiMockService)
    {
        _steamUiMockService = steamUiMockService;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Dashboard";
        Snapshot = _steamUiMockService.GetSnapshot();
    }
}
