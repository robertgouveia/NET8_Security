var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", opt =>
{
    opt.Cookie.Name = "CookieAuth";
    opt.LoginPath = "/Account/Login"; // Default
    opt.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(opt =>
{
    // HR user policy is met if the department claim holds HR
    opt.AddPolicy("HRUser", policy => policy.RequireClaim("Department", "HR"));
    opt.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    opt.AddPolicy("HRManagerOnly", policy => policy.RequireClaim("Department", "HR").RequireClaim("Manager"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Allows for calling Authentication Handler
app.UseAuthorization();

app.MapRazorPages();

app.Run();