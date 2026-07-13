using System.Text.Json;
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
        return AjaxResult(
            _state.SendFriendRequest(GetToken(), TargetSteamId),
            "Request sent.");
    }

    public IActionResult OnPostAccept()
    {
        return AjaxResult(
            _state.AcceptFriendRequestFrom(GetToken(), TargetSteamId),
            "Request accepted.");
    }

    public IActionResult OnPostRemove()
    {
        return AjaxResult(
            _state.RemoveFriendOrRequest(GetToken(), TargetSteamId),
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
        Users = _state.GetWebUsersWithRelationships(token)
            .Where(candidate => candidate.SteamId != user.SteamId)
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
