using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Endpoints;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

// Configure RabbitMQ
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost", // Replace with your RabbitMQ server hostname
        UserName = "guest",     // Replace with RabbitMQ username
        Password = "guest"      // Replace with RabbitMQ password
    };
    return factory.CreateConnection();
});

builder.AddRabbitMQClient(connectionName: "messaging");

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<PaymentDataContext>("paymentsdb");

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapPaymentEndpoints(app.Services.GetRequiredService<IConnection>());

app.UseStaticFiles();


await app.CreateDbIfNotExists();

app.Run();
