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
    public string AdminUsername { get; set; } = string.Empty;

    [BindProperty]
    public string CurrentAdminPassword { get; set; } = string.Empty;

    [BindProperty]
    public string NewAdminPassword { get; set; } = string.Empty;

    [BindProperty]
    public ulong TargetSteamId { get; set; }

    [BindProperty]
    public string TargetPersonaName { get; set; } = string.Empty;

    [BindProperty]
    public ulong LeftSteamId { get; set; }

    [BindProperty]
    public ulong RightSteamId { get; set; }

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

    public IActionResult OnPostCredentials()
    {
        var token = GetToken();
        StatusMessage = _state.ChangeAdminCredentials(token, AdminUsername, CurrentAdminPassword, NewAdminPassword)
            ? "Credenciales admin actualizadas."
            : "No se pudieron cambiar las credenciales admin.";

        return RedirectToPage();
    }

    public IActionResult OnPostPersona()
    {
        var token = GetToken();
        StatusMessage = _state.AdminSetPersona(token, TargetSteamId, TargetPersonaName)
            ? "Nombre de usuario actualizado."
            : "No se pudo actualizar el nombre.";

        return RedirectToPage();
    }

    public IActionResult OnPostLink()
    {
        var token = GetToken();
        StatusMessage = _state.AdminLinkFriends(token, LeftSteamId, RightSteamId)
            ? "Relacion de amistad creada."
            : "No se pudo crear la relacion.";

        return RedirectToPage();
    }

    public IActionResult OnPostUnlink()
    {
        var token = GetToken();
        StatusMessage = _state.AdminUnlinkFriends(token, LeftSteamId, RightSteamId)
            ? "Relacion de amistad eliminada."
            : "No se pudo eliminar la relacion.";

        return RedirectToPage();
    }

    public IActionResult OnPostImportDotaItems()
    {
        var token = GetToken();
        var result = _state.ImportDotaCosmetics(token, new SkyNetDotaImportRequestDto { DotaPath = DotaPath });
        StatusMessage = result.Success
            ? $"Catalogo actualizado: {result.ItemCount} items, {result.HeroCount} heroes."
            : $"No se pudo importar el catalogo: {result.Message}";

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
            ? "Item equipado."
            : "No se pudo equipar el item.";

        return RedirectToPage(new { ItemSearch });
    }

    public IActionResult OnPostClearDotaEquipment()
    {
        var token = GetToken();
        StatusMessage = _state.ClearDotaEquipmentFromAdmin(token, EquipmentSteamId)
            ? "Equipamiento del usuario limpiado."
            : "No se pudo limpiar el equipamiento.";

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
        AdminUsername = overview.Accounts.FirstOrDefault(account => account.IsAdmin)?.Username ?? string.Empty;
        DotaPath = string.IsNullOrWhiteSpace(overview.DotaCosmetics.DotaPath) ? DotaPath : overview.DotaCosmetics.DotaPath;
        EquipmentSteamId = overview.Users.FirstOrDefault()?.SteamId ?? 0;
        return Page();
    }

    private string GetToken() => Request.Cookies[SteamApiStateService.WebSessionCookieName] ?? string.Empty;
}
