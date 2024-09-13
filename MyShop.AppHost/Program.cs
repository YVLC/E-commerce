var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();

builder.AddProject<Projects.Products>("products").WithReference(cache);

builder.AddProject<Projects.Store>("store");

builder.Build().Run();
