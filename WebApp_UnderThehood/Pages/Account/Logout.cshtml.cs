using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderThehood.Pages.Account;

public class Logout : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("CookieAuth"); // Kills cookies in relation to scheme
        return RedirectToPage("/Index");
    }
}