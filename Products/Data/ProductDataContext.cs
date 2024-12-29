using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Products.Data;

// DbContext for the Product entity
public class ProductDataContext : DbContext
{
    public ProductDataContext(DbContextOptions<ProductDataContext> options)
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

        int maxRetries = 5;   // Maximum number of retries
        int delay = 1000;     // Initial delay in milliseconds (1 second)

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Ensure the database is created before attempting initialization
                await context.Database.EnsureCreatedAsync();
                await DbInitializer.Initialize(context);
                break; // Exit loop if successful
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.Error.WriteLine($"Npgsql Exception: {npgsqlEx.Message}");
                Console.Error.WriteLine(npgsqlEx.StackTrace);
                if (attempt == maxRetries)
                {
                    Console.Error.WriteLine("Max retry attempts reached. Failing...");
                    throw; // Rethrow the exception after max retries
                }
            }
            catch (EndOfStreamException eosEx)
            {
                Console.Error.WriteLine($"Stream ended unexpectedly on attempt {attempt}: {eosEx.Message}");
                Console.Error.WriteLine(eosEx.StackTrace);

                if (attempt == maxRetries)
                {
                    Console.Error.WriteLine("Max retry attempts reached. Failing...");
                    throw; // Rethrow the exception after max retries
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database initialization failed: {ex}");
                if (attempt == maxRetries)
                {
                    throw; // Rethrow the exception after max retries
                }
            }
            await Task.Delay(delay);
            delay *= 2; // Double the delay for the next attempt
        }
    }
}

public static class DbInitializer
{
    public static async Task Initialize(ProductDataContext context)
    {
        // Check if there are any products in the database
        if (await context.Product.AnyAsync())
            return;

        var products = new List<Product>
        {
            new Product {Id = new Guid(), Name = "Solar Powered Flashlight", Description = "A fantastic product for outdoor enthusiasts", Price = 19.99m, ImageUrl = "product1.png" },
            new Product { Id = new Guid(),Name = "Hiking Poles", Description = "Ideal for camping and hiking trips", Price = 24.99m, ImageUrl = "product2.png" },
            new Product {Id = new Guid(),  Name = "Outdoor Rain Jacket", Description = "This product will keep you warm and dry in all weathers", Price = 49.99m, ImageUrl = "product3.png" },
            new Product {Id = new Guid(),  Name = "Survival Kit", Description = "A must-have for any outdoor adventurer", Price = 99.99m, ImageUrl = "product4.png" },
            new Product {Id = new Guid(),  Name = "Outdoor Backpack", Description = "This backpack is perfect for carrying all your outdoor essentials", Price = 39.99m, ImageUrl = "product5.png" },
            new Product {Id = new Guid(),  Name = "Camping Cookware", Description = "This cookware set is ideal for cooking outdoors", Price = 29.99m, ImageUrl = "product6.png" },
            new Product {Id = new Guid(),  Name = "Camping Stove", Description = "This stove is perfect for cooking outdoors", Price = 49.99m, ImageUrl = "product7.png" },
            new Product {Id = new Guid(),  Name = "Camping Lantern", Description = "This lantern is perfect for lighting up your campsite", Price = 19.99m, ImageUrl = "product8.png" },
            new Product {Id = new Guid(),  Name = "Camping Tent", Description = "This tent is perfect for camping trips", Price = 99.99m, ImageUrl = "product9.png" },
        };

        // Add products and save changes asynchronously
        await context.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}