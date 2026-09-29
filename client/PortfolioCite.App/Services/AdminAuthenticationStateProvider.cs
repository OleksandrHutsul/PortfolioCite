using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using PortfolioCite.Contracts.Authentication;

namespace PortfolioCite.App.Services;

public class AdminAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    private readonly AuthTokenStore _tokenStore;

    public AdminAuthenticationStateProvider(AuthTokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        var principal = string.IsNullOrWhiteSpace(token) ? Anonymous : CreatePrincipal(token);

        return new AuthenticationState(principal);
    }

    public async Task SignInAsync(LoginResponse response)
    {
        await _tokenStore.StoreAsync(response);

        var authenticationState = new AuthenticationState(CreatePrincipal(response.AccessToken));
        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
    }

    public async Task SignOutAsync()
    {
        await _tokenStore.ClearAsync();

        var authenticationState = new AuthenticationState(Anonymous);
        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
    }

    private static ClaimsPrincipal CreatePrincipal(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return Anonymous;

            var payload = parts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            using var document = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            var claims = new List<Claim>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(property.Value.EnumerateArray()
                        .Select(value => new Claim(property.Name, value.ToString())));

                    continue;
                }

                claims.Add(new Claim(property.Name, property.Value.ToString()));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", "email", "role"));
        }
        catch
        {
            return Anonymous;
        }
    }
}
