using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
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

        MfaSetupViewModel.QRCodeBytes = GenerateQRCode("web app", MfaSetupViewModel.Key, user.Email ?? string.Empty);
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

    private Byte[] GenerateQRCode(string provider, string key, string userEmail)
    {
        var qrCodeGenerator = new QRCodeGenerator();
        var qrCodeData = qrCodeGenerator.CreateQrCode($"optauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
}

public class MFASetupViewModel
{
    public string? Key { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; } = string.Empty;
    
    public Byte[]? QRCodeBytes { get; set; }
}