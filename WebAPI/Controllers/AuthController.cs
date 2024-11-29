using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        // Simulating Security Verification
        if (credential is not { Username: "admin", Password: "password" })
        {
            ModelState.AddModelError("Unauthorized", "You are not authorized to access this endpoint.");
            return Unauthorized(ModelState);
        }
        
        List<Claim> claims = 
        [
            new(ClaimTypes.Name, "admin"),
            new(ClaimTypes.Email, "admin@website.com"),
            new("Department", "HR"),
            new("Admin", "true"),
            new("Manager", "true"),
            new("EmploymentDate", "2024-01-01")
        ];
        
        var expiresAt = DateTime.Now.AddMinutes(10);

        return Ok(new
        {
            access_token = CreateToken(claims, expiresAt), // Returns token string
            expires_at = expiresAt,
        });
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiration)
    {
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");
        
        // Generate JWT
        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(jwt); // Takes a token and returns a string
    }
}

public class Credential
{
    public string Username { get; set; }
    public string Password { get; set; }
}