using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<PaymentDataContext>("paymentsdb");

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapPaymentEndpoints();

app.UseStaticFiles();


await app.CreateDbIfNotExists();

app.Run();
