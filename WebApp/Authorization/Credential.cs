using System.ComponentModel.DataAnnotations;

namespace WebApp.Authorization;

public class Credential
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; } = false;
}