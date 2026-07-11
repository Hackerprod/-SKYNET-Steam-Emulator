using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Achievements;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public IReadOnlyList<SteamAchievement> Achievements { get; private set; } = Array.Empty<SteamAchievement>();

    public IndexModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Stats";
        var token = Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
        if (_state.GetWebUser(token) == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        Achievements = _state.GetWebSnapshot(token).Achievements;
        return Page();
    }
}
