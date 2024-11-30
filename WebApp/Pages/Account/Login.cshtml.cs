using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Authorization;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class LoginModel(SignInManager<User> manager) : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await manager.PasswordSignInAsync(Credential.Email, Credential.Password, Credential.RememberMe, false);
        if (result.Succeeded) return RedirectToPage("/Index");

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("/Account/LoginTwoFactor", new { Credential.Email, Credential.RememberMe  });
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Login", "Your account is locked out");
        }
        
        ModelState.AddModelError("Login", "Failed to login");
        return Page();
    }
}