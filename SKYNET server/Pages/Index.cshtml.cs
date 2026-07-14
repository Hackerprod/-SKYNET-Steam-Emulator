using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SteamUiSnapshot Snapshot { get; private set; } = new();
    public ApiUser CurrentUser { get; private set; } = new();
    public ApiStatsEnvelope Stats { get; private set; } = new();
    public IReadOnlyList<ApiFriendRequestView> IncomingFriendRequests { get; private set; } = Array.Empty<ApiFriendRequestView>();
    public IReadOnlyList<ApiFriendRequestView> OutgoingFriendRequests { get; private set; } = Array.Empty<ApiFriendRequestView>();

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
        return AjaxResult(user != null, user != null ? "Name updated." : "Could not update name.");
    }

    public IActionResult OnPostAvatar()
    {
        var token = GetToken();
        if (AvatarFile == null || AvatarFile.Length == 0)
            return AjaxResult(false, "Select an image.");

        if (AvatarFile.Length > AvatarImage.MaxSourceBytes)
            return AjaxResult(false, "Image is too large (max 20 MB).");

        using var stream = new MemoryStream();
        AvatarFile.CopyTo(stream);
        var updated = _state.PutSelfAvatar(token, new ApiAvatarUpdate
        {
            ContentBase64 = Convert.ToBase64String(stream.ToArray())
        });
        return AjaxResult(updated, updated ? "Avatar updated." : "Could not process image. Use a valid format (PNG, JPG, WEBP...).");
    }

    public IActionResult OnPostStat()
    {
        var token = GetToken();
        var current = _state.GetStats(token, 0, true);
        if (current == null || string.IsNullOrWhiteSpace(StatName))
            return AjaxResult(false, "Could not save stat.");

        var stats = current.Stats.Where(s => !s.Name.Equals(StatName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        stats.Add(new ApiStat { Name = StatName.Trim(), Data = StatValue });
        _state.StoreStats(token, new ApiStoreStatsRequest
        {
            SteamId = current.SteamId,
            Stats = stats,
            Achievements = current.Achievements
        });
        return AjaxResult(true, "Stat updated.");
    }

    public IActionResult OnPostAcceptFriend()
    {
        return AjaxResult(
            _state.AcceptFriendRequest(GetToken(), RequestId),
            "Request accepted.");
    }

    public IActionResult OnPostDeclineFriend()
    {
        return AjaxResult(
            _state.DeclineFriendRequest(GetToken(), RequestId),
            "Request declined.");
    }

    public IActionResult OnPostCancelFriend()
    {
        return AjaxResult(
            _state.CancelFriendRequest(GetToken(), RequestId),
            "Request cancelled.");
    }

    public IActionResult OnPostRemoveFriend()
    {
        return AjaxResult(
            _state.RemoveFriend(GetToken(), FriendSteamId),
            "Friend removed.");
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
        Stats = _state.GetStats(token, 0, true) ?? new ApiStatsEnvelope { SteamId = user.SteamId };
        IncomingFriendRequests = _state.GetIncomingFriendRequests(token);
        OutgoingFriendRequests = _state.GetOutgoingFriendRequests(token);
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;

    private IActionResult AjaxResult(bool success, string message)
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return new JsonResult(new { success, message });
        StatusMessage = message;
        return RedirectToPage();
    }
}
