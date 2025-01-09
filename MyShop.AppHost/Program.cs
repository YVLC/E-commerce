using RabbitMQ.Client;

var builder = DistributedApplication.CreateBuilder(args);

// Manually configure RabbitMQ connection
var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithManagementPlugin();

// Create and configure RabbitMQ connection using ConnectionFactory
var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",   // RabbitMQ server hostname
    Port = 5672,             // RabbitMQ port (default: 5672)
    UserName = "guest",      // RabbitMQ username
    Password = "guest"       // RabbitMQ password
};

// Set up the RabbitMQ connection and channel
var connection = connectionFactory.CreateConnection();
var channel = connection.CreateModel();

// Declare the exchange (ensures it exists)
channel.ExchangeDeclare(exchange: "paymentExchange", type: ExchangeType.Fanout);

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var paymentdb = db.AddDatabase("paymentsdb");

var authdb = db.AddDatabase("authdb");

var orderdb = db.AddDatabase("orderdb");

var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsdb);

var orders = builder.AddProject<Projects.Ordering>("orders")
        .WithReference(orderdb);

var payments = builder.AddProject<Projects.Payment>("payments")
        .WithReference(paymentdb);

var auths = builder.AddProject<Projects.Authentication>("auths")
        .WithReference(authdb);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(payments).WithReference(auths).WithReference(orders);

builder.AddProject<Projects.NotificationService>("notificationservice").WithReference(rabbitmq);

builder.Build().Run();
