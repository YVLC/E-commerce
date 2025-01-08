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

        group.MapPost("/", async (AuthenticationDataContext db, DataEntities.Authentication authRecord) =>
        {
            // Map the incoming Authentication object to the entity
            var authentication = new DataEntities.Authentication
            {
                userid = Guid.NewGuid(), // Generate a new User ID
                email = authRecord.email,
                verifiedemail = authRecord.verifiedemail,
                password = authRecord.password,
                username = authRecord.username,
                firstname = authRecord.firstname,
                lastname = authRecord.lastname,
                phone_number = authRecord.phone_number,
                city = authRecord.city,
                country = authRecord.country,
                address = authRecord.address,
                postcode = authRecord.postcode,
                role = "User"
            };
            // Save the Authentication record to the database
            await db.Authentications.AddAsync(authentication);
            await db.SaveChangesAsync();
            // Return created authentication details
            return Results.Ok(authentication);
        })
        .WithName("CreateUser")
        .Produces<DataEntities.Authentication>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
        group.MapPut("/{userid}", async (AuthenticationDataContext db, Guid userid, DataEntities.Authentication authRecord) =>
        {
            // Find the existing user in the database
            var existingUser = await db.Authentications.FindAsync(userid);
            if (existingUser == null)
            {
                // If the user is not found, return a 404 Not Found
                return Results.NotFound($"User with ID {userid} not found.");
            }
            existingUser.userid = authRecord.userid;
            // Update the fields of the existing user with the new values
            if (!string.IsNullOrEmpty(authRecord.email))
            {
                existingUser.email = authRecord.email;
            }
            if (!string.IsNullOrEmpty(authRecord.verifiedemail.ToString())) // Assuming verifiedemail is a boolean, no need to check for null.
            {
                existingUser.verifiedemail = authRecord.verifiedemail;
            }
            if (!string.IsNullOrEmpty(authRecord.password))
            {
                existingUser.password = authRecord.password;
            }
            if (!string.IsNullOrEmpty(authRecord.username))
            {
                existingUser.username = authRecord.username;
            }
            if (!string.IsNullOrEmpty(authRecord.firstname))
            {
                existingUser.firstname = authRecord.firstname;
            }
            if (!string.IsNullOrEmpty(authRecord.lastname))
            {
                existingUser.lastname = authRecord.lastname;
            }
            if (!string.IsNullOrEmpty(authRecord.phone_number))
            {
                existingUser.phone_number = authRecord.phone_number;
            }
            if (!string.IsNullOrEmpty(authRecord.city))
            {
                existingUser.city = authRecord.city;
            }
            if (!string.IsNullOrEmpty(authRecord.country))
            {
                existingUser.country = authRecord.country;
            }
            if (!string.IsNullOrEmpty(authRecord.address))
            {
                existingUser.address = authRecord.address;
            }
            if (!string.IsNullOrEmpty(authRecord.postcode))
            {
                existingUser.postcode = authRecord.postcode;
            }
            if (!string.IsNullOrEmpty(authRecord.role))
            {
                existingUser.role = authRecord.role;
            }
            // Save the changes to the database
            await db.SaveChangesAsync();
            // Return the updated user details
            return Results.Ok(existingUser); // Return the updated authentication details
        })
    .WithName("UpdateUser")
    .Produces<DataEntities.Authentication>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);
    }
}