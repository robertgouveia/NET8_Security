using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class ConfirmEmail(UserManager<User> manager) : PageModel
{
    [BindProperty]
    public string Message { get; set; } = string.Empty;
    
    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        var user = await manager.FindByIdAsync(userId);
        if (user is null)
        {
            Message = "Failed to validate email.";
            return Page();
        }

        var result = await manager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            Message = "Failed to confirm email.";
            return Page();
        }

        Message = "Email Address validated";
        return Page();
    }
}