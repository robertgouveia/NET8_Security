using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class LoginTwoFactorMFA(SignInManager<User> manager, UserManager<User> userManager) : PageModel
{
    [BindProperty] public MFAAppViewModel MFAAppViewModel { get; set; } = new();
    
    public void OnGet(bool rememberMe)
    {
        MFAAppViewModel.Token = string.Empty;
        MFAAppViewModel.RememberMe = rememberMe;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        
        // It does not need a user since the ID of the user is in a MFA Cookie
        var result = await manager.TwoFactorAuthenticatorSignInAsync(MFAAppViewModel.Token, MFAAppViewModel.RememberMe, false);
        if (result.Succeeded) return RedirectToPage("/Index");
        
        ModelState.AddModelError("Authenticator", result.IsLockedOut ? "Your account has been locked out." : "Invalid login attempt.");
        return Page();
    }
}

public class MFAAppViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; }
}

