using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

[Authorize]
public class UserProfile(UserManager<User> manager, SignInManager<User> signInManager) : PageModel
{
    [BindProperty]
    public UserProfileViewModel UserProfileViewModel { get; set; } = new();
    
    [BindProperty]
    public string? SuccessMessage { get; set; } // Needs to be nullable else you cannot submit form
    
    public async Task<IActionResult> OnGet()
    {
        SuccessMessage = string.Empty;
        
        var (user, department, position) = await GetCurrentUser();
        if (user is null) return RedirectToPage("/Index");
        
        UserProfileViewModel.Email = User.Identity?.Name ?? string.Empty;
        UserProfileViewModel.Department = department?.Value ?? string.Empty;
        UserProfileViewModel.Position = position?.Value ?? string.Empty;
        
        return Page();
    }

    private async Task<(User? user, Claim? department, Claim? position)> GetCurrentUser()
    {
        var user = await manager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

        var claims =  await manager.GetClaimsAsync(user!);
        var department = claims.FirstOrDefault(x => x.Type == "Department");
        var position = claims.FirstOrDefault(x => x.Type == "Position");
        
        return (user, department, position);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var (user, department, position) = await GetCurrentUser();
        if (user is null && department is null && position is null)
        {
            ModelState.AddModelError("UserProfile", "Unable to get user profile.");
            return Page();
        }
        
        try
        {
            await manager.ReplaceClaimAsync(user!, department!, new Claim(department!.Type, UserProfileViewModel.Department));
            await manager.ReplaceClaimAsync(user!, position!, new Claim(position!.Type, UserProfileViewModel.Position));
        }
        catch
        {
            ModelState.AddModelError("UserProfile", "Error occured while processing your request.");
        }
        
        SuccessMessage = "User profile successfully updated";
        await signInManager.RefreshSignInAsync(user!);
        return Page();
    }
}

public class UserProfileViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Department { get; set; } = string.Empty;
    [Required]
    public string Position { get; set; } = string.Empty;
}