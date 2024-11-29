using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        var httpClient = Factory.CreateClient("OurWebAPI");
        Forecasts = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? [];
    }
}