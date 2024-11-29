using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderThehood.Authorization;
using WebApp_UnderThehood.DTO;

namespace WebApp_UnderThehood.Pages;

[Authorize("HRManagerOnly")]
public class HRManager : PageModel
{
    // Allows for http client creation
    public IHttpClientFactory Factory { get; }
    
    [BindProperty]
    public List<WeatherForecastDTO> Forecasts { get; set; }
    public HRManager(IHttpClientFactory factory)
    {
        Factory = factory;
    }
    public async Task OnGet()
    {
        // Check Token
        JwtToken? token;
        
        var strToken = HttpContext.Session.GetString("access_token"); // You can fetch a value from its key
        
        var httpClient = Factory.CreateClient("OurWebAPI");
        
        if (strToken is null)
        {
            token = await Authenticate(httpClient);
        }
        else
        {
            // Session
            token = JsonSerializer.Deserialize<JwtToken>(strToken) ?? new JwtToken();
        }

        if (string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt < DateTime.Now)
        {
            token = await Authenticate(httpClient);
        }
        
        // Attaches the token to the header
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        Forecasts = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? [];
    }

    private async Task<JwtToken> Authenticate(HttpClient httpClient)
    {
        // Authentication
        // This would ofc be stored of course in the appsettings (This is the credentials for the web application and not the user)
        var res = await httpClient.PostAsJsonAsync("auth", new Credential { Username = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        
        var strJwt = await res.Content.ReadAsStringAsync();
        HttpContext.Session.SetString("access_token", strJwt); // Adds a key value to the session
        return JsonSerializer.Deserialize<JwtToken>(strJwt) ?? new JwtToken();
    }
}