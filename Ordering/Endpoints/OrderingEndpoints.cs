using DataEntities;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Store.Services; //This has to be refactored to DataEntities.Services or Refactore DataEntities to include OrderItem

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

            group.MapPost("/", async (OrderingDataContext db, OrderRecord orderRecord) =>
            {
                // Map the incoming OrderRecord to the Order entity
                var order = new Order
                {
                    OrderNumber = orderRecord.OrderNumber,  // Generate a new Order ID
                    Date = orderRecord.Date,
                    Status = orderRecord.Status,
                    City = orderRecord.City,
                    Country = orderRecord.Country,
                    Street = orderRecord.Street,
                    Total = orderRecord.Total
                };

                // Map the OrderItems
                foreach (var orderItem in orderRecord.OrderItems)
                {
                    var orderItemEntity = new DataEntities.OrderItem
                    {
                        Id = orderItem.Id,
                        ProductName = orderItem.ProductName,
                        UnitPrice = orderItem.UnitPrice,
                        Units = orderItem.Units,
                        PictureUrl = orderItem.PictureUrl
                    };

                    order.OrderItems.Add(orderItemEntity);
                }

                // Save the order and order items to the database
                await db.Orders.AddAsync(order);
                await db.SaveChangesAsync();

                return Results.Created($"/api/Orders/{order.OrderNumber}", order); // Return created order
            })
            .WithName("CreateOrder")
            .Produces<Order>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}