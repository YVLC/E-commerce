using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();

builder.AddRedisOutputCache("cache");

builder.Services.AddDbContext<ProductDataContext>(options =>
	options.UseInMemoryDatabase("inmemproducts"));

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseOutputCache();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseMiddleware<RandomFailureMiddleware>();

app.MapProductEndpoints();

app.UseStaticFiles();

app.CreateDbIfNotExists();

app.Run();


public class RandomFailureMiddleware : IMiddleware
{
	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var path = context.Request.Path.Value;

		if (path is null || !path.Contains("api/Product", StringComparison.InvariantCultureIgnoreCase))
			return next(context);

		return next(context);
	}
}
