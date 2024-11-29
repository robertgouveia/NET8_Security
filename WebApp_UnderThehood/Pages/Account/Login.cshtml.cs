using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderThehood.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // Simulating Security Verification
        if (Credential is not { Username: "admin", Password: "password" }) return Page();
        
        // Create Security Context
        var claims = new List<Claim> { new (ClaimTypes.Name, "admin"), new (ClaimTypes.Email, "admin@website.com"), new ("Department", "HR"), new ("Admin", "true"), new ("Manager", "true")};

        // Should use a constant instead of hard code
        var identity = new ClaimsIdentity(claims, "CookieAuth"); // Holds Claims for an Auth Type
        ClaimsPrincipal claimsPrincipal = new(identity);
        await HttpContext.SignInAsync("CookieAuth", claimsPrincipal); // Signs in using the scheme and principle

        return RedirectToPage("/Index");
    }
}

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}