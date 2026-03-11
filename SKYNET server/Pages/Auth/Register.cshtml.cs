using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SKYNET_server.Pages.Auth;

public class RegisterModel : PageModel
{
    public void OnGet()
    {
        ViewData["UseAppShell"] = false;
        ViewData["Title"] = "Registro";
    }
}
