using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Users;

public class ProfileModel : PageModel
{
    private readonly SteamApiStateService _state;

    public ApiUserProfile Profile { get; private set; } = new();

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

    public IActionResult OnPostRequest() => Act("request");

    public IActionResult OnPostAccept() => Act("accept");

    // Cancel (own pending), decline (incoming pending) and remove (friend) all end here; the server decides which it was.
    public IActionResult OnPostRemove() => Act("remove");

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

    private IActionResult Act(string action)
    {
        var result = _state.WebFriendAction(GetToken(), SteamId, action);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            // relationship = what the viewer really has now; the page renders its buttons from it.
            return new JsonResult(new { success = result.Success, message = result.Message, relationship = result.Relationship })
            {
                StatusCode = result.Status
            };
        }

        StatusMessage = result.Success ? result.Message : "Could not update the relationship. " + result.Message;
        return RedirectToPage(new { steamId = SteamId });
    }
}
