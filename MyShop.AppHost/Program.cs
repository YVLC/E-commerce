var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var paymentdb = db.AddDatabase("paymentsdb");

var authdb = db.AddDatabase("authdb");

var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsdb);

var payments = builder.AddProject<Projects.Payment>("payments")
        .WithReference(paymentdb);

var auths = builder.AddProject<Projects.Authentication>("auths")
        .WithReference(authdb);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(payments).WithReference(auths);

builder.AddProject<Projects.NotificationService>("notificationservice");

builder.Build().Run();
