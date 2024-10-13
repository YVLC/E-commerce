using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Payments.Data;

public class PaymentDataContext : DbContext
{
    public PaymentDataContext (DbContextOptions<PaymentDataContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payment { get; set; } = default!;
}

public static class Extensions
{
    public static async Task CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PaymentDataContext>();
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
    public static async Task Initialize(PaymentDataContext context)
    {
        if (await context.Payment.AnyAsync()) // Use async method here
            return;

        Thread.Sleep(300);
        var products = new List<Payment>
        {
            new Payment { paymentId = new Guid(), amount = "33", date = "24/01/12", orderid = "sassad", paymentmethod = "Visa", paymentstatus = "successful", transactionid = "sdasd" },
            new Payment { paymentId = new Guid(), amount = "33", date = "24/01/12", orderid = "sassad", paymentmethod = "Visa", paymentstatus = "successful", transactionid = "sdasd" },

        };
        context.AddRange(products);

        await context.SaveChangesAsync(); // Save changes asynchronously
    }
}