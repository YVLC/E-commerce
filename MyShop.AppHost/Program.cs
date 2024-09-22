var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsdb);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(cache);

builder.AddProject<Projects.Payment>("payment");

builder.AddProject<Projects.Authentication>("authentication");

builder.AddProject<Projects.Basket>("basket");

builder.AddProject<Projects.NotificationService>("notidicationservice");

builder.Build().Run();
