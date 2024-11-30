using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

[Authorize] // Log in first
public class AuthenticatorWithMFASetup : PageModel
{
    private UserManager<User> manager { get; set; }
    
    [BindProperty]
    public MFASetupViewModel MfaSetupViewModel { get; set; } = new();

    public bool Succeeded { get; set; }

    public AuthenticatorWithMFASetup(UserManager<User> manager)
    {
        this.manager = manager;
    }
    
    public async Task OnGetAsync()
    {
        Succeeded = false;
        
        var user = await manager.GetUserAsync(User);
        if (user is null) throw new InvalidOperationException("Unable to load user");
        
        await manager.ResetAuthenticatorKeyAsync(user);
        var key = await manager.GetAuthenticatorKeyAsync(user);
        
        MfaSetupViewModel.Key = key ?? string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        
        var user = await manager.GetUserAsync(User);
        if (user is null) throw new InvalidOperationException("Unable to load user");

        if (!await manager.VerifyTwoFactorTokenAsync(user, manager.Options.Tokens.AuthenticatorTokenProvider, MfaSetupViewModel.SecurityCode))
        {
            ModelState.AddModelError("AuthenticatorSetup", "Failed to authenticate user.");
            return Page();
        }
        
        await manager.SetTwoFactorEnabledAsync(user, true);
        Succeeded = true;
        return Page();
    }
}

public class MFASetupViewModel
{
    public string? Key { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; } = string.Empty;
}