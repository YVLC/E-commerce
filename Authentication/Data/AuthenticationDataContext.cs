using DataEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Authentication.Data;

public class AuthenticationDataContext : DbContext
{
    public AuthenticationDataContext (DbContextOptions<AuthenticationDataContext> options)
        : base(options)
    {
    }

    public DbSet<DataEntities.Authentication> Authentications { get; set; } = default!;
}

public static class Extensions
{
    public static async Task CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AuthenticationDataContext>();

        int maxRetries = 5;   // Maximum number of retries
        int delay = 1000;     // Initial delay in milliseconds (1 second)

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                context.Database.EnsureCreated();
                await DbInitializer.Initialize(context);
                break; // Exit loop if successful
            }
            catch (NpgsqlException npgsqlEx)
            {
                // Specific handling for Npgsql exceptions
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
                    if (attempt == maxRetries)
                    {
                        throw; // Rethrow the exception after max retries
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database initialization failed: {ex}");
                throw;
            }
            await Task.Delay(delay);
            delay *= 2; // Double the delay for the next attempt
        }
    }
}

public static class DbInitializer
{
    public static async Task Initialize(AuthenticationDataContext context)
    {
        if (await context.Authentications.AnyAsync()) // Use async method here
            return;

        Thread.Sleep(300);
        var products = new List<DataEntities.Authentication>
        {
            new DataEntities.Authentication { userid = new Guid(), email = "33", password = "1234", firstname = "sassad", username = "Visa", lastname = "successful", phone_number = "sdasd", city = "Somthing", country= "somthing", address = "s", postcode= "312", role = "Administrator"},
            new DataEntities.Authentication { userid = new Guid(), email = "3423", password = "1234", firstname = "sassad", username = "Visa", lastname = "successful", phone_number = "sdasd", city = "Somthing", country= "somthing",  address ="ss", postcode="3213", role = "User" },

        };
        context.AddRange(products);

        await context.SaveChangesAsync(); // Save changes asynchronously
    }
}