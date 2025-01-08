using DataEntities;
using Store.Components.Models;
using Store.WebAppComponents;
using System.Security.Claims;
using System.Text.Json;

namespace Store.Services;

public class AuthenticationService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor; // To access the current HttpContext

    public AuthenticationService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<Authentication>> GetAuthentications()
    {
        List<Authentication>? authentications = null;
        var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            authentications = await response.Content.ReadFromJsonAsync(AuthSerializerContext.Default.ListAuthentication);
        }

        return authentications ?? new List<Authentication>();
    }
    public async Task<LoginResponse?> Login(string email, string password)
    {
        var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");

        if (response.IsSuccessStatusCode)
        {
            // Deserialize the response content into a list of authentications
            var authentications = await response.Content.ReadFromJsonAsync<List<Authentication>>();

            // Find the user matching the provided email and password
            var user = authentications?.FirstOrDefault(a => a.email == email && a.password == password);

            if (user != null)
            {
                // Return the user ID and role if found
                return new LoginResponse
                {
                    UserId = user.userid,
                    Role = user.role // Assuming the role is part of the `user` object
                };
            }
        }

        // Return null if no match is found or if response fails
        return null;
    }
    public async Task<bool> Register(string email, string password, string username, string firstname, string lastname, string phonenumber, string city, string country, string address, string postcode)
    {
        List<Authentication>? authentications = null;

        Guid guid = Guid.NewGuid();
        var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            authentications = await response.Content.ReadFromJsonAsync(AuthSerializerContext.Default.ListAuthentication);

            if (authentications.Any(a => a.email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Email already in use
            }
            var newUser = new Authentication
            {
                userid = guid,
                email = email,
                verifiedemail = false,
                password = password, // Ensure you hash the password before storing it
                username = username,
                firstname = firstname,
                lastname = lastname,
                phone_number = phonenumber,
                city = city,
                country = country,
                address = address,
                postcode = postcode
            };
            var postResponse = await httpClient.PostAsJsonAsync($"https://localhost:7238/api/Authentication/", newUser);
            Console.WriteLine(postResponse.ToString());
            return postResponse.IsSuccessStatusCode;
        }
        return false;
    }
    public async Task<bool> Update(string email, string password, string username, string firstname, string lastname, string? phonenumber, string city, string country, string address, string postcode)
    {
        List<Authentication>? authentications = null;

        var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            authentications = await response.Content.ReadFromJsonAsync(AuthSerializerContext.Default.ListAuthentication);

            var userIdstring = await GetUserIdAsync();
            if (Guid.TryParse(userIdstring, out var userid)) ;
            if (authentications.Any(a => a.email.Equals(email, StringComparison.OrdinalIgnoreCase) && !a.userid.Equals(userid)))
            {
                return false; // Email already in use
            }

            var newUser = new Authentication
            {
                userid = userid,
                email = email,
                password = password, // Ensure you hash the password before storing it
                username = username,
                firstname = firstname,
                lastname = lastname,
                phone_number = phonenumber,
                city = city,
                country = country,
                address = address,
                postcode = postcode
            };
            var postResponse = await httpClient.PutAsJsonAsync($"https://localhost:7238/api/Authentication/{userid}", newUser);
            Console.WriteLine(postResponse.ToString());
            return postResponse.IsSuccessStatusCode;
        }
        return false;
    }

    internal async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        var context = httpContextAccessor.HttpContext;
        var user = context?.User;

        if (user != null && user.Identity.IsAuthenticated)
        {
            return user; // Return the current user's ClaimsPrincipal
        }

        return null; 
    }
    internal async Task<string?> GetUserIdAsync()
    {
        // Get the current user from ClaimsPrincipal
        var user = await GetCurrentUserAsync();

        if (user == null) { return null; }

        // Check if the user is authenticated
        if (user.Identity?.IsAuthenticated == true)
        {
            // Extract the user ID from claims (e.g., ClaimTypes.NameIdentifier or "sub")
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Alternatively, check for "sub" if using JWT:
            // var userId = user.FindFirst("sub")?.Value;

            return userId;
        }

        // If the user is not authenticated, return null
        return null;
    }
    public async Task<Authentication?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            // Make sure the URL expects a GUID in the path
            var response = await httpClient.GetAsync($"https://localhost:7238/api/Authentication/{userId}");

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the user details from the response
                var user = await response.Content.ReadFromJsonAsync<Authentication>(AuthSerializerContext.Default.Authentication);
                return user;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Handle the case where the user is not found
                Console.WriteLine($"User with ID {userId} not found.");
            }
            else
            {
                // Handle other non-success responses
                Console.WriteLine($"Error fetching user by ID: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"An error occurred while fetching the user: {ex.Message}");
        }

        return null; // Return null if user is not found or an error occurs
    }


    public async Task<bool> UpdateRole(Guid userId, string roleName)
    {
        // Retrieve user list from the API
        var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");
        if (!response.IsSuccessStatusCode)
        {
            return false; // API call failed
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Deserialize the list of users
        var users = await response.Content.ReadFromJsonAsync(AuthSerializerContext.Default.ListAuthentication);
        if (users == null)
        {
            return false; // Unable to retrieve users
        }

        // Find the user by their ID
        var user = users.FirstOrDefault(u => u.userid == userId);
        if (user == null)
        {
            return false; // User not found
        }

        // Update the role of the found user
        user.role = roleName;

        // Send the updated user object back to the API
        var updateResponse = await httpClient.PostAsJsonAsync($"https://localhost:7238/api/Authentication/{userId}", user);
        return updateResponse.IsSuccessStatusCode;
    }
}