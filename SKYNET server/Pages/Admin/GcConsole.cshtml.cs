using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Admin;

public class GcConsoleModel : PageModel
{
    private readonly SteamApiStateService _state;
    private readonly GameCoordinatorTraceService _trace;

    public GcConsoleModel(SteamApiStateService state, GameCoordinatorTraceService trace)
    {
        _state = state;
        _trace = trace;
    }

    public IActionResult OnGet()
    {
        if (!_state.IsWebAdmin(GetToken()))
        {
            return RedirectToPage("/Auth/Login");
        }

        ViewData["Title"] = "GC Console";
        return Page();
    }

    public IActionResult OnGetEntries(long since)
    {
        if (!_state.IsWebAdmin(GetToken()))
        {
            return Unauthorized();
        }

        return new JsonResult(new { entries = _trace.GetSince(since) });
    }

    public IActionResult OnGetLiveState()
    {
        var diagnostics = _state.GetLiveDiagnostics(GetToken());
        return diagnostics == null ? Unauthorized() : new JsonResult(diagnostics);
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
