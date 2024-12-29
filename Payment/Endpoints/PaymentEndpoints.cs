using Payments.Data;
using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace Payments.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Payment");

        group.MapGet("/", async (PaymentDataContext db) =>
        {
            return await db.Payment.ToListAsync();
        })
        .WithName("GetAllPayments")
        .Produces<List<Payment>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async  (Guid id, PaymentDataContext db) =>
        {
            return await db.Payment.AsNoTracking()
                .FirstOrDefaultAsync(model => model.paymentId == id)
                is Payment model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetPaymentById")
        .Produces<Payment>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async  (Guid id, Payment payment, PaymentDataContext db) =>
        {
            var affected = await db.Payment
                .Where(model => model.paymentId == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.paymentId, payment.paymentId)
                  .SetProperty(m => m.amount, payment.amount)
                  .SetProperty(m => m.PaymentMethod, payment.PaymentMethod)
                  .SetProperty(m => m.paymentstatus, payment.paymentstatus)
                  .SetProperty(m => m.date, payment.date)
                  .SetProperty(m => m.orderid, payment.orderid)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdatePayment")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (Payment payment, PaymentDataContext db) =>
        {
            payment.paymentId = Guid.NewGuid();
            db.Payment.Add(payment);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Payment/{payment.paymentId}",payment);
        })
        .WithName("CreatePayment")
        .Produces<Payment>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async  (Guid id, PaymentDataContext db) =>
        {
            var affected = await db.Payment
                .Where(model => model.paymentId == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeletePayment")
        .Produces<Payment>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
