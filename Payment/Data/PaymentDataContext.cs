﻿using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Payments.Data;

public class PaymentDataContext : DbContext
{
    public PaymentDataContext (DbContextOptions<PaymentDataContext> options)
        : base(options)
    {
    }

    public DbSet<DataEntities.Payment> Payment { get; set; } = default!;
}

public static class Extensions
{
    public static async Task CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PaymentDataContext>();

        int maxRetries = 5;   // Maximum number of retries
        int delay = 1000;     // Initial delay in milliseconds (1 second)

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                await DbInitializer.Initialize(context);
                Console.WriteLine("Database initialized successfully.");
                break; // Exit loop if successful
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.Error.WriteLine($"Npgsql Exception on attempt {attempt}: {npgsqlEx.Message}");
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
                Console.Error.WriteLine($"Unexpected error on attempt {attempt}: {ex.Message}");
                if (attempt == maxRetries)
                {
                    throw; // Rethrow the exception after max retries
                }
            }
            // Exponential backoff delay
            await Task.Delay(delay);
            delay *= 2; // Double the delay for the next attempt
        }
    }
}

public static class DbInitializer
{
    public static async Task Initialize(PaymentDataContext context)
    {
        if (await context.Payment.AnyAsync()) // Use async method here
            return;

        Guid temp = Guid.NewGuid();
        Thread.Sleep(300);
        var products = new List<DataEntities.Payment>
        {
            new DataEntities.Payment { paymentId = new Guid(), amount = 33, date = DateTime.UtcNow, orderid = temp, PaymentMethod = "Visa", paymentstatus = "successful"},
            new DataEntities.Payment { paymentId = new Guid(), amount = 33, date = DateTime.UtcNow, orderid = temp, PaymentMethod = "Visa", paymentstatus = "successful"},

        };
        context.AddRange(products);

        await context.SaveChangesAsync(); // Save changes asynchronously
    }
}