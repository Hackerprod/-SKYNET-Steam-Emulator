using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Games;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public IReadOnlyList<SteamGame> Games { get; private set; } = Array.Empty<SteamGame>();

    public IndexModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Games";
        var token = Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
        if (_state.GetWebUser(token) == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        Games = _state.GetWebSnapshot(token).Games;
        return Page();
    }
}
