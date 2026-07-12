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
            ? "Request sent."
            : "Could not send request.";

        return RedirectToPage();
    }

    public IActionResult OnPostAccept()
    {
        StatusMessage = _state.AcceptFriendRequestFrom(GetToken(), TargetSteamId)
            ? "Request accepted."
            : "Could not accept request.";

        return RedirectToPage();
    }

    public IActionResult OnPostRemove()
    {
        StatusMessage = _state.RemoveFriendOrRequest(GetToken(), TargetSteamId)
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
        Users = _state.GetWebUsersWithRelationships(token)
            .Where(candidate => candidate.SteamId != user.SteamId)
            .ToList();
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
