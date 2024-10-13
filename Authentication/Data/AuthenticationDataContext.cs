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
    public static async Task Initialize(AuthenticationDataContext context)
    {
        if (await context.Authentications.AnyAsync()) // Use async method here
            return;

        Thread.Sleep(300);
        var products = new List<DataEntities.Authentication>
        {
            new DataEntities.Authentication { userid = new Guid(), email = "33", password = "1234", firstname = "sassad", username = "Visa", lastname = "successful", phone_number = "sdasd" },
            new DataEntities.Authentication { userid = new Guid(), email = "3423", password = "1234", firstname = "sassad", username = "Visa", lastname = "successful", phone_number = "sdasd" },

        };
        context.AddRange(products);

        await context.SaveChangesAsync(); // Save changes asynchronously
    }
}