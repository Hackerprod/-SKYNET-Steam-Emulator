using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public ApiAdminOverview Overview { get; private set; } = new();
    public ApiDotaCosmeticOverview Cosmetics { get; private set; } = new();

    [BindProperty]
    public string NewAdminPassword { get; set; } = string.Empty;

    [BindProperty]
    public ulong ResetTargetSteamId { get; set; }

    [BindProperty]
    public ulong TargetSteamId { get; set; }

    [BindProperty]
    public string TargetPersonaName { get; set; } = string.Empty;

    [BindProperty]
    public string CreateUsername { get; set; } = string.Empty;

    [BindProperty]
    public string CreatePassword { get; set; } = string.Empty;

    [BindProperty]
    public string DotaPath { get; set; } = @"D:\Games\Steam\steamapps\common\dota 2 beta";

    [BindProperty(SupportsGet = true)]
    public string ItemSearch { get; set; } = string.Empty;

    [BindProperty]
    public ulong EquipmentSteamId { get; set; }

    [BindProperty]
    public uint EquipmentHeroId { get; set; }

    [BindProperty]
    public string EquipmentHeroName { get; set; } = string.Empty;

    [BindProperty]
    public uint EquipmentSlotId { get; set; }

    [BindProperty]
    public string EquipmentSlot { get; set; } = string.Empty;

    [BindProperty]
    public uint EquipmentDefIndex { get; set; }

    [BindProperty]
    public uint EquipmentStyle { get; set; }

    [BindProperty]
    public ulong DeleteSteamId { get; set; }

    [BindProperty]
    public ulong ToggleAdminSteamId { get; set; }

    [BindProperty]
    public string GsAdvertisedIp { get; set; } = string.Empty;

    [BindProperty]
    public bool GsDedicatedEnabled { get; set; }

    [BindProperty]
    public string GsBindIp { get; set; } = "0.0.0.0";

    [BindProperty]
    public int GsPortStart { get; set; } = 27025;

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    public IndexModel(SteamApiStateService state)
    {
        _state = state;
    }

    public IActionResult OnGet()
    {
        ViewData["Title"] = "Admin";
        return LoadPage();
    }

    public IActionResult OnPostResetPassword()
    {
        var token = GetToken();
        return AjaxResult(
            _state.ResetUserPassword(token, ResetTargetSteamId, NewAdminPassword),
            "Password reset.");
    }

    public IActionResult OnPostPersona()
    {
        var token = GetToken();
        return AjaxResult(
            _state.AdminSetPersona(token, TargetSteamId, TargetPersonaName),
            "Username updated.");
    }

    public IActionResult OnPostCreateAccount()
    {
        var token = GetToken();
        var result = _state.AdminCreateWebAccount(token, CreateUsername, CreateUsername, CreatePassword, false);
        return AjaxResult(result != null, result != null ? $"Account '{result.Username}' created." : "Could not create account. Username may already exist.");
    }

    public IActionResult OnPostImportDotaItems()
    {
        var token = GetToken();
        var result = _state.ImportDotaCosmetics(token, new ApiDotaImportRequest { DotaPath = DotaPath });
        return AjaxResult(result.Success, result.Success ? $"Catalog updated: {result.ItemCount} items, {result.HeroCount} heroes." : $"Could not import catalog: {result.Message}");
    }

    public IActionResult OnPostEquipDotaItem()
    {
        var token = GetToken();
        return AjaxResult(
            _state.EquipDotaItemFromAdmin(token, new ApiDotaEquipItemRequest
            {
                SteamId = EquipmentSteamId,
                HeroId = EquipmentHeroId,
                HeroName = EquipmentHeroName,
                Slot = EquipmentSlot,
                SlotId = EquipmentSlotId,
                DefIndex = EquipmentDefIndex,
                Style = EquipmentStyle
            }),
            "Item equipped.");
    }

    public IActionResult OnPostClearDotaEquipment()
    {
        var token = GetToken();
        return AjaxResult(
            _state.ClearDotaEquipmentFromAdmin(token, EquipmentSteamId),
            "Equipment cleared.");
    }

    public IActionResult OnPostDeleteUser()
    {
        var token = GetToken();
        return AjaxResult(
            _state.AdminDeleteUser(token, DeleteSteamId),
            "User deleted.");
    }

    public IActionResult OnPostGameServerSettings()
    {
        var token = GetToken();
        var result = _state.UpdateGameServerSettings(token, new ApiGameServerSettings
        {
            AdvertisedGameServerIp = GsAdvertisedIp ?? string.Empty,
            DedicatedEnabled = GsDedicatedEnabled,
            DedicatedBindIp = string.IsNullOrWhiteSpace(GsBindIp) ? "0.0.0.0" : GsBindIp,
            DedicatedPortStart = GsPortStart
        });
        return AjaxResult(result.Success, result.Message);
    }

    public IActionResult OnPostToggleAdmin()
    {
        var token = GetToken();
        var overview = _state.GetAdminOverview(token);
        var account = overview?.Accounts.FirstOrDefault(a => a.SteamId == ToggleAdminSteamId);
        var makeAdmin = account != null && !account.IsAdmin;
        return AjaxResult(
            _state.AdminSetAdmin(token, ToggleAdminSteamId, makeAdmin),
            makeAdmin ? "User promoted to admin." : "Admin rights removed.");
    }

    private IActionResult LoadPage()
    {
        var token = GetToken();
        var overview = _state.GetAdminOverview(token);
        if (overview == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        Overview = overview;
        Cosmetics = _state.GetDotaCosmeticsOverview(token, ItemSearch, null, 120) ?? new ApiDotaCosmeticOverview();
        DotaPath = string.IsNullOrWhiteSpace(overview.DotaCosmetics.DotaPath) ? DotaPath : overview.DotaCosmetics.DotaPath;
        GsAdvertisedIp = overview.GameServerSettings.AdvertisedGameServerIp;
        GsDedicatedEnabled = overview.GameServerSettings.DedicatedEnabled;
        GsBindIp = overview.GameServerSettings.DedicatedBindIp;
        GsPortStart = overview.GameServerSettings.DedicatedPortStart;
        EquipmentSteamId = overview.Users.FirstOrDefault()?.SteamId ?? 0;
        ResetTargetSteamId = overview.Users.FirstOrDefault()?.SteamId ?? 0;
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
