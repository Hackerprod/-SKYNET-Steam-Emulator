using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Friends;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public ApiUser CurrentUser { get; private set; } = new();
    public IReadOnlyList<ApiUser> Friends { get; private set; } = Array.Empty<ApiUser>();
    public IReadOnlyList<ApiUser> Users { get; private set; } = Array.Empty<ApiUser>();

    [BindProperty]
    public string FriendIdentifier { get; set; } = string.Empty;

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
        ViewData["Title"] = "Friends";
        return LoadPage();
    }

    public IActionResult OnPostAdd()
    {
        var token = GetToken();
        return AjaxResult(
            _state.AddFriend(token, FriendIdentifier),
            "Request sent.");
    }

    public IActionResult OnPostRemove()
    {
        var token = GetToken();
        return AjaxResult(
            _state.RemoveFriend(token, FriendSteamId),
            "Relationship updated.");
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
        Friends = _state.GetWebFriends(token);
        Users = _state.GetWebUsers(token)
            .Where(candidate => candidate.SteamId != user.SteamId && candidate.FriendRelationship == 0)
            .ToList();
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
