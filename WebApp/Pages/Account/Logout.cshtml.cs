using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class Logout(SignInManager<User> manager) : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await manager.SignOutAsync(); // Kills cookies in relation to scheme
        return RedirectToPage("/Account/Login");
    }
}