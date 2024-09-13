var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var db = builder.AddPostgres("db").WithPgAdmin();

var productsdb = db.AddDatabase("productsdb");

var products = builder.AddProject<Projects.Products>("products")
        .WithReference(productsdb);

builder.AddProject<Projects.Store>("store").WithReference(products).WithReference(cache);

builder.Build().Run();
