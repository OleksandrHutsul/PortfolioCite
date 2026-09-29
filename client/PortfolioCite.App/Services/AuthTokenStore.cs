using Microsoft.JSInterop;
using PortfolioCite.Contracts.Authentication;

namespace PortfolioCite.App.Services;

public class AuthTokenStore
{
    private const string TokenKey = "portfoliocite.admin.token";
    private const string ExpirationKey = "portfoliocite.admin.expires";

    private readonly IJSRuntime _jsRuntime;

    public AuthTokenStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var expiresValue = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", cancellationToken, ExpirationKey);

        if (!DateTimeOffset.TryParse(expiresValue, out var expiresAt) || expiresAt <= DateTimeOffset.UtcNow)
        {
            await ClearAsync(cancellationToken);
            return null;
        }

        var token = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", cancellationToken, TokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            await ClearAsync(cancellationToken);
            return null;
        }

        return token;
    }

    public async Task StoreAsync(LoginResponse response, CancellationToken cancellationToken = default)
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", cancellationToken, TokenKey, response.AccessToken);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", cancellationToken, ExpirationKey, response.ExpiresAt.ToString("O"));
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", cancellationToken, TokenKey);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", cancellationToken, ExpirationKey);
    }
}
