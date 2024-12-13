using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Store.Components;
using Store.Services;
using Store;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<ProductService>();
builder.Services.AddHttpClient<ProductService>(c =>
{
    var url = builder.Configuration["ProductEndpoint"] ?? throw new InvalidOperationException("ProductEndpoint is not set");
    c.BaseAddress = new(url);
});
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddHttpClient<PaymentService>(c =>
{
    var url = builder.Configuration["PaymentEndpoint"] ?? throw new InvalidOperationException("PaymentEndpoint is not set");
    c.BaseAddress = new(url);
});
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddHttpClient<AuthenticationService>(c =>
{
    var url = builder.Configuration["AuthenticationEndpoint"] ?? throw new InvalidOperationException("Authentication is not set");
    c.BaseAddress = new(url);
});
builder.Services.AddScoped<BasketService>();

builder.Services.AddHttpContextAccessor();

// Register CustomAuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
    });

var app = builder.Build();

app.UseStaticFiles();

// Make sure you only register these routes once
app.MapRazorPages();
app.MapBlazorHub();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Middleware order
app.UseRouting();
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    Console.WriteLine($"Matched endpoint: {endpoint?.DisplayName ?? "None"}");
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapDefaultEndpoints();  // If this method is mapping conflicting routes, consider removing or modifying it.

app.Run();