using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using NotificationService;
using NotificationService.NotificationConsumerEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Configure RabbitMQ connection (use CreateConnection instead of CreateConnectionAsync)
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };

    return factory.CreateConnection(); // Using synchronous method here
});

// Register IModel correctly
builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel(); // Synchronously create the model from the connection
});

// Register the PaymentConsumer class to listen for messages
builder.Services.AddSingleton<PaymentConsumer>();

var app = builder.Build();

// Start consuming messages from RabbitMQ
var paymentConsumer = app.Services.GetRequiredService<PaymentConsumer>();
paymentConsumer.StartListening();

app.MapGet("/", () => "NotificationService is running and listening for messages...");

app.Run();