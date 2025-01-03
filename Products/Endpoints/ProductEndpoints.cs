﻿using DataEntities;
using Microsoft.EntityFrameworkCore;
using Products.Data;

namespace Products.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product");

        group.MapGet("/", async (ProductDataContext db) =>
        {
            return await db.Product.ToListAsync();
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async  (Guid id, ProductDataContext db) =>
        {
            return await db.Product.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/user/{userId}", async (Guid userId, ProductDataContext db) =>
        {
            // Fetch the products based on the userId
            var products = await db.Product
                .AsNoTracking()  // This ensures the query does not track the entities in the change tracker for performance reasons.
                .Where(p => p.UserId == userId)  // Filter products by the UserId
                .ToListAsync();

            return products.Any()
                ? Results.Ok(products)  // Return the products if found
                : Results.NotFound();    // Return NotFound if no products are found for the given userId
        })
        .WithName("GetProductsByUserId")
        .Produces<List<Product>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async  (Guid id, Product product, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (Product product, ProductDataContext db) =>
        {
            try
            {
                var products = new Product
                {
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Id = product.Id,
                    Name = product.Name,
                    Tag = product.Tag,
                    Type = product.Type,
                    UserId = product.UserId
                };
                db.Product.Add(products);
                await db.SaveChangesAsync();

                // Return a successful response with a 201 Created status
                return Results.Created($"/api/Product/{product.Id}", product);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors and return a 500 Internal Server Error
                return Results.Problem($"An error occurred while creating the product: {ex.Message}", statusCode: 500);
            }
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async  (Guid id, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

}
