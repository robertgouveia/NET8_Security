using System.Text.Json.Serialization;

namespace WebApp_UnderThehood.Authorization;

public class JwtToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }
}