using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Users;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public ApiUser CurrentUser { get; private set; } = new();
    public IReadOnlyList<ApiUser> Users { get; private set; } = Array.Empty<ApiUser>();

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

    public IActionResult OnPostRequest() => Act("request");

    public IActionResult OnPostAccept() => Act("accept");

    // Cancel (own pending), decline (incoming pending) and remove (friend) all end here; the server decides which it was.
    public IActionResult OnPostRemove() => Act("remove");

    // Read-only: the viewer's relationship to every user, so the cards follow changes made in game or by the other user.
    public IActionResult OnGetState()
    {
        Response.Headers["Cache-Control"] = "no-store";
        var relationships = _state.GetWebRelationships(GetToken());
        if (relationships == null)
        {
            return new JsonResult(new { success = false, message = "Session expired. Sign in again." }) { StatusCode = 401 };
        }

        return new JsonResult(new { success = true, relationships });
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

    private IActionResult Act(string action)
    {
        var result = _state.WebFriendAction(GetToken(), TargetSteamId, action);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            // relationship = what the viewer really has now; the page renders its buttons from it.
            return new JsonResult(new { success = result.Success, message = result.Message, relationship = result.Relationship })
            {
                StatusCode = result.Status
            };
        }

        StatusMessage = result.Success ? result.Message : "Could not update the relationship. " + result.Message;
        return RedirectToPage();
    }
}
