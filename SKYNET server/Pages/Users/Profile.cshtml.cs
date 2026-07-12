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
        ViewData["Title"] = "Profile";
        return LoadPage();
    }

    public IActionResult OnPostRequest()
    {
        StatusMessage = _state.SendFriendRequest(GetToken(), SteamId)
            ? "Request sent."
            : "Could not send request.";

        return RedirectToPage(new { steamId = SteamId });
    }

    public IActionResult OnPostAccept()
    {
        StatusMessage = _state.AcceptFriendRequestFrom(GetToken(), SteamId)
            ? "Request accepted."
            : "Could not accept request.";

        return RedirectToPage(new { steamId = SteamId });
    }

    public IActionResult OnPostRemove()
    {
        StatusMessage = _state.RemoveFriendOrRequest(GetToken(), SteamId)
            ? "Relationship updated."
            : "Could not update relationship.";

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
