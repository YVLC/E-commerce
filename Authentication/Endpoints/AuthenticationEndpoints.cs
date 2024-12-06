using DataEntities;
using Microsoft.EntityFrameworkCore;
using Authentication.Data;

namespace Authentication.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Authentication");

        group.MapGet("/", async (AuthenticationDataContext db) =>
        {
            return await db.Authentications.ToListAsync();
        })
        .WithName("GetAllAuthentications")
        .Produces<List<DataEntities.Authentication>>(StatusCodes.Status200OK);

    }





}
