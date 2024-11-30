using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data.Account;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(SignInManager<User> manager) : Controller
{
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var loginInfo = await manager.GetExternalLoginInfoAsync();
        if (loginInfo is null) return Unauthorized();

        var email = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        var username = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

        if (email is null || username is null) return Unauthorized();

        var user = new User
        {
            Email = email.Value,
            UserName = username.Value,
        };

        await manager.SignInAsync(user, false);
        return RedirectToPage("/Index");
    }
}