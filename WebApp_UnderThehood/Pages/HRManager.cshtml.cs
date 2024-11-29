using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderThehood.Pages;

[Authorize("HRManagerOnly")]
public class HRManager : PageModel
{
    public void OnGet()
    {
        
    }
}