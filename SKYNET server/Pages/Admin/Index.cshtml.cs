using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly SteamApiStateService _state;

    public SkyNetAdminOverviewDto Overview { get; private set; } = new();
    public SkyNetDotaCosmeticOverviewDto Cosmetics { get; private set; } = new();

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
    public string DotaPath { get; set; } = @"D:\Juegos\Steam\steamapps\common\dota 2 beta";

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
        StatusMessage = _state.ResetUserPassword(token, ResetTargetSteamId, NewAdminPassword)
            ? "Password reset."
            : "Could not reset password.";

        return RedirectToPage();
    }

    public IActionResult OnPostPersona()
    {
        var token = GetToken();
        StatusMessage = _state.AdminSetPersona(token, TargetSteamId, TargetPersonaName)
            ? "Username updated."
            : "Could not update username.";

        return RedirectToPage();
    }

    public IActionResult OnPostCreateAccount()
    {
        var token = GetToken();
        var result = _state.AdminCreateWebAccount(token, CreateUsername, CreateUsername, CreatePassword, false);
        StatusMessage = result != null
            ? $"Account '{result.Username}' created."
            : "Could not create account. Username may already exist.";

        return RedirectToPage();
    }

    public IActionResult OnPostImportDotaItems()
    {
        var token = GetToken();
        var result = _state.ImportDotaCosmetics(token, new SkyNetDotaImportRequestDto { DotaPath = DotaPath });
        StatusMessage = result.Success
            ? $"Catalog updated: {result.ItemCount} items, {result.HeroCount} heroes."
            : $"Could not import catalog: {result.Message}";

        return RedirectToPage(new { ItemSearch });
    }

    public IActionResult OnPostEquipDotaItem()
    {
        var token = GetToken();
        StatusMessage = _state.EquipDotaItemFromAdmin(token, new SkyNetDotaEquipItemRequestDto
        {
            SteamId = EquipmentSteamId,
            HeroId = EquipmentHeroId,
            HeroName = EquipmentHeroName,
            Slot = EquipmentSlot,
            SlotId = EquipmentSlotId,
            DefIndex = EquipmentDefIndex,
            Style = EquipmentStyle
        })
            ? "Item equipped."
            : "Could not equip item.";

        return RedirectToPage(new { ItemSearch });
    }

    public IActionResult OnPostClearDotaEquipment()
    {
        var token = GetToken();
        StatusMessage = _state.ClearDotaEquipmentFromAdmin(token, EquipmentSteamId)
            ? "Equipment cleared."
            : "Could not clear equipment.";

        return RedirectToPage(new { ItemSearch });
    }

    public IActionResult OnPostDeleteUser()
    {
        var token = GetToken();
        StatusMessage = _state.AdminDeleteUser(token, DeleteSteamId)
            ? "User deleted."
            : "Could not delete user. Ensure at least one admin remains.";

        return RedirectToPage(new { ItemSearch });
    }

    public IActionResult OnPostToggleAdmin()
    {
        var token = GetToken();
        var account = Overview.Accounts.FirstOrDefault(a => a.SteamId == ToggleAdminSteamId);
        var makeAdmin = account != null && !account.IsAdmin;
        StatusMessage = _state.AdminSetAdmin(token, ToggleAdminSteamId, makeAdmin)
            ? (makeAdmin ? "User promoted to admin." : "Admin rights removed.")
            : "Could not update admin status. Ensure at least one admin remains.";

        return RedirectToPage(new { ItemSearch });
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
        Cosmetics = _state.GetDotaCosmeticsOverview(token, ItemSearch, null, 120) ?? new SkyNetDotaCosmeticOverviewDto();
        DotaPath = string.IsNullOrWhiteSpace(overview.DotaCosmetics.DotaPath) ? DotaPath : overview.DotaCosmetics.DotaPath;
        EquipmentSteamId = overview.Users.FirstOrDefault()?.SteamId ?? 0;
        ResetTargetSteamId = overview.Users.FirstOrDefault()?.SteamId ?? 0;
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
