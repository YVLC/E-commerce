using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Store.Components;
using Store.Services;

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

builder.Services.AddHttpContextAccessor();

// Register CustomAuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login"; // Path to the login page
        options.AccessDeniedPath = "/access-denied"; // Path for access denied
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30); // Set cookie expiration
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware configuration order
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Add anti-forgery middleware after authentication and authorization
app.UseAntiforgery();

app.MapRazorPages();
app.MapBlazorHub();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();