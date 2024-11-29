using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderThehood.Pages;

[Authorize("AdminOnly")]
public class Settings : PageModel
{
    public void OnGet()
    {
        
    }
}