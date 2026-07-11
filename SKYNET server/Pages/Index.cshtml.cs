using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SteamUiSnapshot Snapshot { get; private set; } = new();
    public SkyNetUserDto CurrentUser { get; private set; } = new();
    public SkyNetStatsEnvelopeDto Stats { get; private set; } = new();
    public IReadOnlyList<SkyNetFriendRequestViewDto> IncomingFriendRequests { get; private set; } = Array.Empty<SkyNetFriendRequestViewDto>();
    public IReadOnlyList<SkyNetFriendRequestViewDto> OutgoingFriendRequests { get; private set; } = Array.Empty<SkyNetFriendRequestViewDto>();

    [BindProperty]
    public string PersonaName { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? AvatarFile { get; set; }

    [BindProperty]
    public string StatName { get; set; } = string.Empty;

    [BindProperty]
    public uint StatValue { get; set; }

    [BindProperty]
    public string RequestId { get; set; } = string.Empty;

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public IndexModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Dashboard";
        return LoadPage();
    }

    public IActionResult OnPostPersona()
    {
        var token = GetToken();
        var user = _state.UpdatePersona(token, PersonaName);
        StatusMessage = user == null ? "No se pudo cambiar el nombre." : "Nombre actualizado.";
        return RedirectToPage();
    }

    public IActionResult OnPostAvatar()
    {
        var token = GetToken();
        if (AvatarFile == null || AvatarFile.Length == 0)
        {
            StatusMessage = "Selecciona una imagen PNG.";
            return RedirectToPage();
        }

        if (!string.Equals(AvatarFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
            AvatarFile.Length > 512 * 1024)
        {
            StatusMessage = "El avatar debe ser PNG y pesar 512 KB o menos.";
            return RedirectToPage();
        }

        using var stream = new MemoryStream();
        AvatarFile.CopyTo(stream);
        var updated = _state.PutSelfAvatar(token, new SkyNetAvatarUpdateDto
        {
            ContentBase64 = Convert.ToBase64String(stream.ToArray())
        });

        StatusMessage = updated ? "Avatar actualizado." : "No se pudo cambiar el avatar.";
        return RedirectToPage();
    }

    public IActionResult OnPostStat()
    {
        var token = GetToken();
        var current = _state.GetStats(token, 0, true);
        if (current == null || string.IsNullOrWhiteSpace(StatName))
        {
            StatusMessage = "No se pudo guardar el stat.";
            return RedirectToPage();
        }

        var stats = current.Stats.Where(s => !s.Name.Equals(StatName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        stats.Add(new SkyNetStatDto { Name = StatName.Trim(), Data = StatValue });
        _state.StoreStats(token, new SkyNetStoreStatsRequestDto
        {
            SteamId = current.SteamId,
            Stats = stats,
            Achievements = current.Achievements
        });
        StatusMessage = "Stat actualizado.";
        return RedirectToPage();
    }

    public IActionResult OnPostAcceptFriend()
    {
        StatusMessage = _state.AcceptFriendRequest(GetToken(), RequestId)
            ? "Solicitud aceptada."
            : "No se pudo aceptar la solicitud.";

        return RedirectToPage();
    }

    public IActionResult OnPostDeclineFriend()
    {
        StatusMessage = _state.DeclineFriendRequest(GetToken(), RequestId)
            ? "Solicitud rechazada."
            : "No se pudo rechazar la solicitud.";

        return RedirectToPage();
    }

    public IActionResult OnPostCancelFriend()
    {
        StatusMessage = _state.CancelFriendRequest(GetToken(), RequestId)
            ? "Solicitud cancelada."
            : "No se pudo cancelar la solicitud.";

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
        PersonaName = user.PersonaName;
        Snapshot = _state.GetWebSnapshot(token);
        Stats = _state.GetStats(token, 0, true) ?? new SkyNetStatsEnvelopeDto { SteamId = user.SteamId };
        IncomingFriendRequests = _state.GetIncomingFriendRequests(token);
        OutgoingFriendRequests = _state.GetOutgoingFriendRequests(token);
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
