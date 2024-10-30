using DataEntities;
using Store.Components.Models;
using System.Text.Json;

namespace Store.Services;

public class AuthenticationService
{
    HttpClient httpClient;
    public AuthenticationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
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
    public async Task<bool> Login(string email, string password)
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

            foreach(var a in authentications)
            {
                if (a.email == email && a.password == password)
                {
                    return true;
                }
            }

        }

        return false;
    }

}