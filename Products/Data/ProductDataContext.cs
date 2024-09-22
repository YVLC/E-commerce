using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Products.Data;

public class ProductDataContext : DbContext
{
    public ProductDataContext (DbContextOptions<ProductDataContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Product { get; set; } = default!;
}

public static class Extensions
{
    public static async Task CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ProductDataContext>();
        try
        {
            context.Database.EnsureCreated();
            await DbInitializer.Initialize(context);
        }
        catch (NpgsqlException npgsqlEx)
        {
            // Specific handling for Npgsql exceptions
            Console.Error.WriteLine($"Npgsql Exception: {npgsqlEx.Message}");
            Console.Error.WriteLine(npgsqlEx.StackTrace);
            throw;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Database initialization failed: {ex}");
            throw;
        }
    }
}

public static class DbInitializer
{
    public static async Task Initialize(ProductDataContext context)
    {
        if (await context.Product.AnyAsync()) // Use async method here
            return;

        Thread.Sleep(300);
        var products = new List<Product>
        {
            new Product { Name = "Solar Powered Flashlight", Description = "A fantastic product for outdoor enthusiasts", Price = 19.99m, ImageUrl = "product1.png" },
            new Product { Name = "Hiking Poles", Description = "Ideal for camping and hiking trips", Price = 24.99m, ImageUrl = "product2.png" },
            new Product { Name = "Outdoor Rain Jacket", Description = "This product will keep you warm and dry in all weathers", Price = 49.99m, ImageUrl = "product3.png" },
            new Product { Name = "Survival Kit", Description = "A must-have for any outdoor adventurer", Price = 99.99m, ImageUrl = "product4.png" },
            new Product { Name = "Outdoor Backpack", Description = "This backpack is perfect for carrying all your outdoor essentials", Price = 39.99m, ImageUrl = "product5.png" },
            new Product { Name = "Camping Cookware", Description = "This cookware set is ideal for cooking outdoors", Price = 29.99m, ImageUrl = "product6.png" },
            new Product { Name = "Camping Stove", Description = "This stove is perfect for cooking outdoors", Price = 49.99m, ImageUrl = "product7.png" },
            new Product { Name = "Camping Lantern", Description = "This lantern is perfect for lighting up your campsite", Price = 19.99m, ImageUrl = "product8.png" },
            new Product { Name = "Camping Tent", Description = "This tent is perfect for camping trips", Price = 99.99m, ImageUrl = "product9.png" },
        };

        context.AddRange(products);

        await context.SaveChangesAsync(); // Save changes asynchronously
    }
}