using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Friends;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SkyNetUserDto CurrentUser { get; private set; } = new();
    public IReadOnlyList<SkyNetUserDto> Friends { get; private set; } = Array.Empty<SkyNetUserDto>();
    public IReadOnlyList<SkyNetUserDto> Users { get; private set; } = Array.Empty<SkyNetUserDto>();

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
        StatusMessage = _state.AddFriend(token, FriendIdentifier)
            ? "Request sent."
            : "Could not send request. Use username, SteamID, AccountID or exact name.";

        return RedirectToPage();
    }

    public IActionResult OnPostRemove()
    {
        var token = GetToken();
        StatusMessage = _state.RemoveFriend(token, FriendSteamId)
            ? "Relationship updated."
            : "Could not update relationship.";

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
        Friends = _state.GetWebFriends(token);
        Users = _state.GetWebUsers(token)
            .Where(candidate => candidate.SteamId != user.SteamId && candidate.FriendRelationship == 0)
            .ToList();
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
