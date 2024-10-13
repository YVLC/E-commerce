using Microsoft.EntityFrameworkCore;
using Authentication.Data;
using Authentication.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AuthenticationDataContext>("authdb");

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapAuthenticationEndpoints();

app.UseStaticFiles();

await app.CreateDbIfNotExists();

app.Run();
