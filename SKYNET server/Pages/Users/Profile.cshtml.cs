using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Users;

public class ProfileModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SkyNetUserProfileDto Profile { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public ulong SteamId { get; set; }

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public ProfileModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Perfil";
        return LoadPage();
    }

    public IActionResult OnPostRequest()
    {
        StatusMessage = _state.SendFriendRequest(GetToken(), SteamId)
            ? "Solicitud enviada."
            : "No se pudo enviar la solicitud.";

        return RedirectToPage(new { steamId = SteamId });
    }

    public IActionResult OnPostAccept()
    {
        StatusMessage = _state.AcceptFriendRequestFrom(GetToken(), SteamId)
            ? "Solicitud aceptada."
            : "No se pudo aceptar la solicitud.";

        return RedirectToPage(new { steamId = SteamId });
    }

    public IActionResult OnPostRemove()
    {
        StatusMessage = _state.RemoveFriendOrRequest(GetToken(), SteamId)
            ? "Relacion actualizada."
            : "No se pudo actualizar la relacion.";

        return RedirectToPage(new { steamId = SteamId });
    }

    private IActionResult LoadPage()
    {
        var token = GetToken();
        var profile = _state.GetWebUserProfile(token, SteamId);
        if (profile == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        Profile = profile;
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
