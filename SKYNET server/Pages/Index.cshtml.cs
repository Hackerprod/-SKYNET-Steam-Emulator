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

    [BindProperty]
    public ulong FriendSteamId { get; set; }

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
        StatusMessage = user == null ? "Could not update name." : "Name updated.";
        return RedirectToPage();
    }

    public IActionResult OnPostAvatar()
    {
        var token = GetToken();
        if (AvatarFile == null || AvatarFile.Length == 0)
        {
            StatusMessage = "Select an image.";
            return RedirectToPage();
        }

        if (AvatarFile.Length > AvatarImage.MaxSourceBytes)
        {
            StatusMessage = "Image is too large (max 20 MB).";
            return RedirectToPage();
        }

        using var stream = new MemoryStream();
        AvatarFile.CopyTo(stream);
        // The server compresses and crops the image internally, so any
        // reasonable format/size is accepted (PNG, JPG, WEBP, ...).
        var updated = _state.PutSelfAvatar(token, new SkyNetAvatarUpdateDto
        {
            ContentBase64 = Convert.ToBase64String(stream.ToArray())
        });

        StatusMessage = updated
            ? "Avatar updated."
            : "Could not process image. Use a valid format (PNG, JPG, WEBP...).";
        return RedirectToPage();
    }

    public IActionResult OnPostStat()
    {
        var token = GetToken();
        var current = _state.GetStats(token, 0, true);
        if (current == null || string.IsNullOrWhiteSpace(StatName))
        {
            StatusMessage = "Could not save stat.";
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
        StatusMessage = "Stat updated.";
        return RedirectToPage();
    }

    public IActionResult OnPostAcceptFriend()
    {
        StatusMessage = _state.AcceptFriendRequest(GetToken(), RequestId)
            ? "Request accepted."
            : "Could not accept request.";

        return RedirectToPage();
    }

    public IActionResult OnPostDeclineFriend()
    {
        StatusMessage = _state.DeclineFriendRequest(GetToken(), RequestId)
            ? "Request declined."
            : "Could not decline request.";

        return RedirectToPage();
    }

    public IActionResult OnPostCancelFriend()
    {
        StatusMessage = _state.CancelFriendRequest(GetToken(), RequestId)
            ? "Request cancelled."
            : "Could not cancel request.";

        return RedirectToPage();
    }

    public IActionResult OnPostRemoveFriend()
    {
        StatusMessage = _state.RemoveFriend(GetToken(), FriendSteamId)
            ? "Friend removed."
            : "Could not remove friend.";

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
