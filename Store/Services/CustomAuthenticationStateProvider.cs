using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Store.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationStateProvider(AuthenticationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Retrieve the current user from the auth service
            var user = await _authService.GetCurrentUserAsync(); // You need to implement this method in AuthenticationService

            // If the user is not authenticated, create an anonymous user
            if (user == null)
            {
                user = new ClaimsPrincipal(new ClaimsIdentity()); // Anonymous user
            }

            return new AuthenticationState(user);
        }

        public async Task SignInAsync(string email, string role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

            var identity = new ClaimsIdentity(claims, "Custom authentication");
            var user = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            // Notify that the authentication state has changed
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task SignOut()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity()); // Anonymous user
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}
