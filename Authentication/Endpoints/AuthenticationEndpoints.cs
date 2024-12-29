using Authentication.Data;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Authentication");

        // Endpoint to fetch all authentications
        group.MapGet("/", async (AuthenticationDataContext db) =>
        {
            return await db.Authentications.ToListAsync();
        })
        .WithName("GetAllAuthentications")
        .Produces<List<DataEntities.Authentication>>(StatusCodes.Status200OK);

        // Endpoint to fetch user details by ID
        group.MapGet("/{id:Guid}", async (Guid id, AuthenticationDataContext db) =>
        {
            var user = await db.Authentications.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound($"User with ID {id} not found.");
            }

            return Results.Ok(user);
        })
        .WithName("GetAuthenticationById")
        .Produces<DataEntities.Authentication>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}