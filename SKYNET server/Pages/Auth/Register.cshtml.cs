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
        ViewData["Title"] = "Registro";
    }

    public IActionResult OnPost()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Registro";

        if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
        {
            ErrorMessage = "La confirmacion no coincide.";
            return Page();
        }

        var result = _state.RegisterWeb(Username, PersonaName, Password, SteamApiStateService.GetClientIp(Request));
        if (result == null)
        {
            ErrorMessage = "No se pudo crear el usuario. Revisa los datos o usa otro nombre.";
            return Page();
        }

        Response.Cookies.Append(SteamApiStateService.WebSessionCookieName, result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(14)
        });

        return RedirectToPage("/Index");
    }
}
