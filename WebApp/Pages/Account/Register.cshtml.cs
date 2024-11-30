using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resend;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

public class Register(UserManager<User> manager, IResend resend) : PageModel
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
        var user = new User
        {
            Email = RegisterViewModel.Email,
            UserName = RegisterViewModel.Email,
            //Department = RegisterViewModel.Department,
            //Position = RegisterViewModel.Position, -- Can also be done with claims
        };
        
        
        var result = await manager.CreateAsync(user, RegisterViewModel.Password);
        if (result.Succeeded)
        {
            var claims = new List<Claim>
            {
                new("Department", RegisterViewModel.Department),
                new("Position", RegisterViewModel.Position),
                new("Plan", "free"),
            };
            await manager.AddClaimsAsync(user, claims);
            
            var token = await manager.GenerateEmailConfirmationTokenAsync(user); // User contains ID after CreateAsync
            
            // Send an Email
            var link =  Url.PageLink("/Account/ConfirmEmail", values: new { userId = user.Id, token });

            var message = new EmailMessage
            {
                From = "localhost@websitech.uk",
                To = user.Email,
                Subject = "Confirm your email",
                HtmlBody = $"<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><title>Confirm your Email!</title></head><body><h1>Please click the link below:</h1><br><a href='https://websitech.uk/confirmEmail'>Confirmation Link</a></body></html>", // Ensure proper link formatting
                TextBody = $"Click the link: {link}" // Optional, but recommended
            };

            // Handle Email Response
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
    
    [Required]
    public string Department { get; set; } = string.Empty;
    
    [Required]
    public string Position { get; set; } = string.Empty;
}