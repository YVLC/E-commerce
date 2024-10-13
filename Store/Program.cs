using Store.Components;
using Store.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddHttpClient<PaymentService>(c =>
{
    var url = builder.Configuration["PaymentEndpoint"] ?? throw new InvalidOperationException("PaymentEndpoint is not set");

    c.BaseAddress = new(url);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
