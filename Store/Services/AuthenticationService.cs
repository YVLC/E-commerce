using DataEntities;
using Store.Components.Models;
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
    public async Task<string?> Login(string email, string password)
{
    var response = await httpClient.GetAsync("https://localhost:7238/api/Authentication");
    if (response.IsSuccessStatusCode)
    {
        var authentications = await response.Content.ReadFromJsonAsync(AuthSerializerContext.Default.ListAuthentication);

        var user = authentications?.FirstOrDefault(a => a.email == email && a.password == password);
        return user?.role; // Return the role if found; null otherwise
    }
    return null;
}
    public async Task<bool> Register(string email, string password, string username, string firstname, string lastname, string? phonenumber, string address, string postcode)
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
                password = password, // Ensure you hash the password before storing it
                username = username,
                firstname = firstname,
                lastname = lastname,
                phone_number = phonenumber,
                address = address,
                postcode = postcode
            };
            var postResponse = await httpClient.PostAsJsonAsync("https://localhost:7238/api/Authentication/", newUser);
            Console.WriteLine(postResponse.ToString());
            return postResponse.IsSuccessStatusCode;
        }
        return false;
    }
    public async Task<bool> Update(Guid guid,string email, string password, string username, string firstname, string lastname, string? phonenumber, string address, string postcode)
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

            if (authentications.Any(a => a.email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // Email already in use
            }
            var newUser = new Authentication
            {
                userid = guid,
                email = email,
                password = password, // Ensure you hash the password before storing it
                username = username,
                firstname = firstname,
                lastname = lastname,
                phone_number = phonenumber,
                address = address,
                postcode = postcode
            };
            var postResponse = await httpClient.PostAsJsonAsync("https://localhost:7238/api/Authentication/", newUser);
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

        // Optionally handle cases where the user is not authenticated
        return new ClaimsPrincipal(new ClaimsIdentity()); // Or return null based on your needs
    }
}