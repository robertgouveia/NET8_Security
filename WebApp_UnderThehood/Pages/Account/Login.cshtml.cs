using System.ComponentModel.DataAnnotations;
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

    public void OnPost()
    {
        
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