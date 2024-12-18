using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Ordering.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<OrderingDataContext>("orderdb");

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapOrderingEndpoints();

app.UseStaticFiles();

await app.CreateDbIfNotExists();

app.Run();