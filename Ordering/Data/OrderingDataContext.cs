using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;


namespace Ordering.Data

{
    public class OrderingDataContext : DbContext
    {
        public OrderingDataContext(DbContextOptions<OrderingDataContext> options)
            : base(options)
        {
        }

        public DbSet<DataEntities.Order> Orders { get; set; } = default!;
    }

    public static class Extensions
    {
        public static async Task CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<OrderingDataContext>();

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
        public static async Task Initialize(OrderingDataContext context)
        {
            // Check if there are any products in the database
            if (await context.Orders.AnyAsync())
                return;

            var orders = new List<Order>
{
    new Order
    {
        Date = DateTime.Now,
        City = "Washington",
        Country = "USA",
        Description = null,
        OrderNumber = 1,
        Status = "in warehouse",
        Street = "Street Avenue",
        Total = 123,
        OrderItems = new List<Orderitem>
        {
            new Orderitem { ProductName = "smth", UnitPrice = 50, Units = 50, PictureUrl = "smth" },
            new Orderitem { ProductName = "smth2", UnitPrice = 50, Units = 50, PictureUrl = "smth" }
        }
    },
        new Order
    {
        Date = DateTime.Now,
        City = "Washington",
        Country = "USA",
        Description = null,
        OrderNumber = 2,
        Status = "in warehouse",
        Street = "Street Avenue",
        Total = 123,
        OrderItems = new List<Orderitem>
        {
            new Orderitem { ProductName = "smth", UnitPrice = 50, Units = 50, PictureUrl = "smth" },
            new Orderitem { ProductName = "smth2", UnitPrice = 50, Units = 50, PictureUrl = "smth" }
        }
    },
};

            // Add products and save changes asynchronously
            await context.AddRangeAsync(orders);
            await context.SaveChangesAsync();
        }
    }
}
