using Microsoft.EntityFrameworkCore;
using Ordering.Data;

namespace Ordering.Endpoints
{
    public static class OrderingEndpoints
    {
        public static void MapOrderingEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Orders");

            group.MapGet("/", async (OrderingDataContext db) =>
            {
                // Eagerly load the OrderItems along with Orders
                var orders = await db.Orders
                    .Include(o => o.OrderItems)  // Include related OrderItems
                    .ToListAsync();

                return Results.Ok(orders); // Return the orders along with the order items
            })
            .WithName("GetAllOrders")  // Changed name to better reflect the purpose
            .Produces<List<DataEntities.Order>>(StatusCodes.Status200OK);
        }
    }
}