using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderThehood.Pages;

[Authorize("HRUser")]
public class HumanResource : PageModel
{
    public void OnGet()
    {
        
    }
}