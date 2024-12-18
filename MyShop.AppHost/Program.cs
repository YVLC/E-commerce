var builder = DistributedApplication.CreateBuilder(args);

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

builder.AddProject<Projects.NotificationService>("notificationservice");

builder.Build().Run();
