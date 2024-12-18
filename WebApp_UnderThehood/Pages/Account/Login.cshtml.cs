using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderThehood.Authorization;

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
        var claims = new List<Claim> { new (ClaimTypes.Name, "admin"), new (ClaimTypes.Email, "admin@website.com"), new ("Department", "HR"), new ("Admin", "true"), new ("Manager", "true"), new ("EmploymentDate", "2024-01-01")};

        // Should use a constant instead of hard code
        var identity = new ClaimsIdentity(claims, "CookieAuth"); // Holds Claims for an Auth Type
        ClaimsPrincipal claimsPrincipal = new(identity);

        var authProperies = new AuthenticationProperties
        {
            IsPersistent = Credential.RememberMe, // Allowing survival of browser closure
        };
        
        await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, authProperies); // Signs in using the scheme and principle

        return RedirectToPage("/Index");
    }
}