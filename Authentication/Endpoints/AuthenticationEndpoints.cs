using DataEntities;
using Microsoft.EntityFrameworkCore;
using Authentication.Data;

namespace Authentication.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Authentication");

        group.MapGet("/", async (AuthenticationDataContext db) =>
        {
            return await db.Authentications.ToListAsync();
        })
        .WithName("GetAllAuthentications")
        .Produces<List<DataEntities.Authentication>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async  (Guid id, AuthenticationDataContext db) =>
        {
            return await db.Authentications.AsNoTracking()
                .FirstOrDefaultAsync(model => model.userid == id)
                is DataEntities.Authentication model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetAuthenticationById")
        .Produces<DataEntities.Authentication>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async  (Guid id, DataEntities.Authentication authentication, AuthenticationDataContext db) =>
        {
            var affected = await db.Authentications
                .Where(model => model.userid == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.userid, authentication.userid)
                  .SetProperty(m => m.email, authentication.email)
                  .SetProperty(m => m.username, authentication.username)
                  .SetProperty(m => m.firstname, authentication.firstname)
                  .SetProperty(m => m.lastname, authentication.lastname)
                  .SetProperty(m => m.address, authentication.address)
                  .SetProperty(m => m.postcode, authentication.postcode)
                  .SetProperty(m => m.role, "User")
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateAuthentication")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (DataEntities.Authentication authentication, AuthenticationDataContext db) =>
        {
            db.Authentications.Add(authentication);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Authentication/{authentication.userid}",authentication);
        })
        .WithName("CreateAuthentication")
        .Produces<DataEntities.Authentication>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async  (Guid id, AuthenticationDataContext db) =>
        {
            var affected = await db.Authentications
                .Where(model => model.userid == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteAuthentication")
        .Produces<DataEntities.Authentication>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
