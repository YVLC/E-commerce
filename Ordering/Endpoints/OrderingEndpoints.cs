using DataEntities;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;

namespace Ordering.Endpoints;

public static class OrderingEndpoints
{
    public static void MapOrderingEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Orders");

        group.MapGet("/", async (OrderingDataContext db) =>
        {
            return await db.Orders.ToListAsync();
        })
        .WithName("GetAllAuthentications")
        .Produces<List<DataEntities.Order>>(StatusCodes.Status200OK);

    }





}