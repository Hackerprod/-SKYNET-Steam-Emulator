using System.Text.Json;
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
        return AjaxResult(
            _state.SendFriendRequest(GetToken(), SteamId),
            "Request sent.");
    }

    public IActionResult OnPostAccept()
    {
        return AjaxResult(
            _state.AcceptFriendRequestFrom(GetToken(), SteamId),
            "Request accepted.");
    }

    public IActionResult OnPostRemove()
    {
        return AjaxResult(
            _state.RemoveFriendOrRequest(GetToken(), SteamId),
            "Relationship updated.");
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

    private IActionResult AjaxResult(bool success, string message)
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return new JsonResult(new { success, message });
        StatusMessage = message;
        return RedirectToPage(new { steamId = SteamId });
    }
}
