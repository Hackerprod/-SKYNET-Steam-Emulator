using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly SteamApiStateService _state;

    public LogoutModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        var token = Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
        _state.LogoutWeb(token);
        Response.Cookies.Delete(SteamApiStateService.WebSessionCookieName);
        return RedirectToPage("/Auth/Login");
    }
}
