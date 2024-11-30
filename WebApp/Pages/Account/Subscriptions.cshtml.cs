using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;
using WebApp.Data.Account;

namespace WebApp.Pages.Account;

[Authorize]
public class Subscriptions(UserManager<User> manager) : PageModel
{
    [BindProperty] public SubscriptionViewModel SubscriptionViewModel { get; set; } = new();

    public IActionResult OnGet()
    {
        if (User.Claims.FirstOrDefault(x => x.Type == "Plan")?.Value!= "free")
        {
            return RedirectToPage("/Account/UserProfile");
        }

        return Page();
    }
    
    public async Task OnPostAsync()
    {
        var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        var price = SubscriptionViewModel.Plan switch
        {
            "enterprise" => "price_1QQtKQI649PmTAZOqLDqA62i",
            "premium" => "price_1QQtJpI649PmTAZO4T80M4D5",
            _ => "price_1QQtJ7I649PmTAZOn1GVPZF5"
        };

        var customerOptions = new CustomerCreateOptions
        {
            Email = email!.Value,
            Plan = price
        };

        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(customerOptions);

        var user = await manager.FindByEmailAsync(email.Value);
        await manager.AddClaimsAsync(user!, new[]
        {
            new Claim("customer", customer.Id),
            new Claim("plan", price)
        });

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(new SessionCreateOptions
        {
            SuccessUrl = "https://localhost:8080/success?plan{}",
            Customer = customer.Id,
            CustomerEmail = email.Value,
        });
        
    }
}

public class SubscriptionViewModel
{
    public string Plan { get; set; } = string.Empty;
}