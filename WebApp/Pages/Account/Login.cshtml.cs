using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Authorization;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class LoginModel(SignInManager<User> manager) : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();
    
    [BindProperty]
    public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }
    
    public async Task OnGetAsync()
    {
        // Gets external providers
        ExternalLogins = await manager.GetExternalAuthenticationSchemesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await manager.PasswordSignInAsync(Credential.Email, Credential.Password, Credential.RememberMe, false);
        if (result.Succeeded) return RedirectToPage("/Index");

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("/Account/LoginTwoFactorMFA", new { Credential.RememberMe  });
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Login", "Your account is locked out");
        }
        
        ModelState.AddModelError("Login", "Failed to login");
        return Page();
    }
    
    // Page handler
    public IActionResult OnPostLoginExternally(string provider) // Provider comes from the name
    {
        var properties = manager.ConfigureExternalAuthenticationProperties(provider, Url.Action("ExternalLoginCallback", "Account"));

        // Challenge helps us redirect
        return Challenge(properties, provider);
    }
}