var builder = DistributedApplication.CreateBuilder(args);

var basketcache = builder.AddRedis("cache");

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var authdb = db.AddDatabase("authdb");

var paymentdb = db.AddDatabase("paymentdb");

var auths = builder.AddProject<Projects.Authentication>("auths")
        .WithReference(authdb);

var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsdb);

var payments = builder.AddProject<Projects.Payment>("payments")
        .WithReference(paymentdb);

var basket = builder.AddProject<Projects.Basket>("basket").WithReference(basketcache);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(auths).WithReference(payments).WithReference(basket);

builder.AddProject<Projects.NotificationService>("notidicationservice");

builder.Build().Run();
