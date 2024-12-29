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

        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!; // Explicitly declare DbSet for OrderItems

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Order and OrderItem relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderNumber)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure OrderItem primary key
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.Id);
        }
    }

    public static class Extensions
    {
        public static async Task CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<OrderingDataContext>();

            int maxRetries = 5; // Maximum number of retries
            int delay = 1000; // Initial delay in milliseconds (1 second)

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
            // Ensure the Orders table is populated with initial data
            if (await context.Orders.AnyAsync()) return;

            var orders = new List<Order>
            {
                new Order
                {
                    OrderNumber = new Guid(),
                    Date = DateTime.UtcNow, // Store date as UTC
                    Status = "in warehouse",
                    City = "Washington",
                    Country = "USA",
                    Street = "Street Avenue",
                    Total = 123,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "smth", UnitPrice = 50, Units = 1, PictureUrl = "smth" },
                        new OrderItem { ProductName = "smth2", UnitPrice = 50, Units = 2, PictureUrl = "smth2" }
                    }
                },
                new Order
                {
                    OrderNumber = new Guid(),
                    Date = DateTime.UtcNow, // Store date as UTC
                    Status = "in warehouse",
                    City = "Washington",
                    Country = "USA",
                    Street = "Street Avenue",
                    Total = 123,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "smth3", UnitPrice = 30, Units = 1, PictureUrl = "smth3" },
                        new OrderItem { ProductName = "smth4", UnitPrice = 20, Units = 3, PictureUrl = "smth4" }
                    }
                },
            };

            foreach (var order in orders)
            {
                var existingOrder = await context.Orders
                    .Include(o => o.OrderItems) // Ensure OrderItems are loaded
                    .FirstOrDefaultAsync(o => o.OrderNumber == order.OrderNumber);

                if (existingOrder == null)
                {
                    await context.Orders.AddAsync(order);
                }
                else
                {
                    // Update the existing order and its related items
                    context.Entry(existingOrder).CurrentValues.SetValues(order);

                    // Remove existing items not in the new list
                    foreach (var existingItem in existingOrder.OrderItems.ToList())
                    {
                        if (!order.OrderItems.Any(oi => oi.Id == existingItem.Id))
                        {
                            context.Entry(existingItem).State = EntityState.Deleted;
                        }
                    }

                    // Add or update items in the new list
                    foreach (var newItem in order.OrderItems)
                    {
                        var existingItem = existingOrder.OrderItems
                            .FirstOrDefault(oi => oi.Id == newItem.Id);

                        if (existingItem == null)
                        {
                            existingOrder.OrderItems.Add(newItem); // Add new item
                        }
                        else
                        {
                            context.Entry(existingItem).CurrentValues.SetValues(newItem); // Update existing item
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}