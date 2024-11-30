using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resend;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class LoginTwoFactor(UserManager<User> manager, IResend resend, SignInManager<User> signInManager) : PageModel
{
    [BindProperty] public MFAViewModel MfaViewModel { get; set; } = new();
    
    public async Task OnGetAsync(string email, bool rememberMe)
    {
        MfaViewModel.Token = string.Empty;
        MfaViewModel.RememberMe = rememberMe;
        
        // Generate Security Code
        var user = await manager.FindByEmailAsync(email);
        
        if (!await manager.GetTwoFactorEnabledAsync(user!))
        {
            throw new InvalidOperationException("2FA is not enabled for this user.");
        }
        
        var token = await manager.GenerateTwoFactorTokenAsync(user!, "Email");

        // Send to the User
        await resend.EmailSendAsync(new EmailMessage
        {
           Subject = "Authentication - 2FA Code",
           To = email,
           From = "2fa@websitech.uk",
           TextBody = $"Your 2FA code: {token}",
           HtmlBody = $"<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><title>Your 2FA Code</title></head><body><h1>2FA Code: {token}</h1></body></html>", // Ensure proper link formatting
        });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, MfaViewModel.Token, MfaViewModel.RememberMe, false);
        if (result.Succeeded) return RedirectToPage("/Index");
        
        ModelState.AddModelError("UserProfile", result.IsLockedOut ? "Account Locked Out" : "2FA Failed.");
        return Page();
    }
}

public class MFAViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}