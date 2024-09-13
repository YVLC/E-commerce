var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var products = builder.AddProject<Projects.Products>("products").WithReference(cache)
        .WithReference(productsdb);

builder.AddProject<Projects.Store>("store").WithReference(products);

builder.Build().Run();
