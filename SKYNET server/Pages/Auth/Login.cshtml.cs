using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly SteamApiStateService _state;

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public LoginModel(SteamApiStateService state)
    {
        _state = state;
    }

    public void OnGet()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Login";
    }

    public IActionResult OnPost()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Login";

        var result = _state.LoginWeb(Username, Password, RememberMe, SteamApiStateService.GetClientIp(Request));
        if (result == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return new JsonResult(new { success = false, message = "Invalid credentials." });
            ErrorMessage = "Invalid credentials.";
            return Page();
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.Add( RememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(12))
        };

        Response.Cookies.Append(SteamApiStateService.WebSessionCookieName, result.AccessToken, cookieOptions);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return new JsonResult(new { success = true, message = "Welcome back!", redirect = Url.Page("/Index") });

        return RedirectToPage("/Index");
    }
}
