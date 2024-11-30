using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account;

public class Register(UserManager<IdentityUser> manager) : PageModel
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
        if (result.Succeeded) return RedirectToPage("/Index");
        
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