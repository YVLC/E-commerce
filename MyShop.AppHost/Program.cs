var builder = DistributedApplication.CreateBuilder(args);

var basketcache = builder.AddRedis("cache");

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

var basket = builder.AddProject<Projects.Basket>("basket").WithReference(basketcache);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(payments).WithReference(auths).WithReference(basket);

builder.AddProject<Projects.NotificationService>("notificationservice");

builder.Build().Run();
