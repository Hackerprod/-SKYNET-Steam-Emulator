using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Users;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SkyNetUserDto CurrentUser { get; private set; } = new();
    public IReadOnlyList<SkyNetUserDto> Users { get; private set; } = Array.Empty<SkyNetUserDto>();

    [BindProperty]
    public ulong TargetSteamId { get; set; }

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public IndexModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Users";
        return LoadPage();
    }

    public IActionResult OnPostRequest()
    {
        StatusMessage = _state.SendFriendRequest(GetToken(), TargetSteamId)
            ? "Solicitud enviada."
            : "No se pudo enviar la solicitud.";

        return RedirectToPage();
    }

    public IActionResult OnPostAccept()
    {
        StatusMessage = _state.AcceptFriendRequestFrom(GetToken(), TargetSteamId)
            ? "Solicitud aceptada."
            : "No se pudo aceptar la solicitud.";

        return RedirectToPage();
    }

    public IActionResult OnPostRemove()
    {
        StatusMessage = _state.RemoveFriendOrRequest(GetToken(), TargetSteamId)
            ? "Relacion actualizada."
            : "No se pudo actualizar la relacion.";

        return RedirectToPage();
    }

    private IActionResult LoadPage()
    {
        var token = GetToken();
        var user = _state.GetWebUser(token);
        if (user == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        CurrentUser = user;
        Users = _state.GetWebUsersWithRelationships(token)
            .Where(candidate => candidate.SteamId != user.SteamId)
            .ToList();
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
