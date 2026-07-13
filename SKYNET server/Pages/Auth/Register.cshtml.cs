using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly SteamApiStateService _state;

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string PersonaName { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public RegisterModel(SteamApiStateService state)
    {
        _state = state;
    }

    public void OnGet()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Register";
    }

    public IActionResult OnPost()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Register";

        if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return new JsonResult(new { success = false, message = "Passwords do not match." });
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        var result = _state.RegisterWeb(Username, PersonaName, Password, SteamApiStateService.GetClientIp(Request));
        if (result == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return new JsonResult(new { success = false, message = "Could not create account. Check your data or try a different name." });
            ErrorMessage = "Could not create account. Check your data or try a different name.";
            return Page();
        }

        Response.Cookies.Append(SteamApiStateService.WebSessionCookieName, result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(14)
        });

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return new JsonResult(new { success = true, message = "Account created!", redirect = Url.Page("/Index") });

        return RedirectToPage("/Index");
    }
}
