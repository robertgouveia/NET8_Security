using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resend;

namespace WebApp.Pages.Account;

public class Register(UserManager<IdentityUser> manager, IResend resend) : PageModel
{
    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new();
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        
        // Validate Email (Optional)
        
        // Create User
        var user = new IdentityUser
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email,
        };

        var result = await manager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            var token = await manager.GenerateEmailConfirmationTokenAsync(user); // User contains ID after CreateAsync
            
            // Send an Email
            var link =  Url.PageLink("/Account/ConfirmEmail", values: new { userId = user.Id, token });

            var message = new EmailMessage
            {
                From = "test@willowstutbury.uk",
                To = user.Email,
                Subject = "Confirm your email",
                HtmlBody = $"Click the link: <a href='{link}'>{link}</a>", // Ensure proper link formatting
                TextBody = $"Click the link: {link}" // Optional, but recommended
            };


            var res = await resend.EmailSendAsync(message);
            return RedirectToPage("/Account/Login");
        }
        
        foreach (var err in result.Errors)
        {
            ModelState.AddModelError("Register", err.Description);
        }

        return Page();
    }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}