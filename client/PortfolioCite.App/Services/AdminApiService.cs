using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    private readonly HttpClient _httpClient;
    private readonly AdminAuthenticationStateProvider _authenticationStateProvider;

    public AdminApiService(HttpClient httpClient, AdminAuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = response.StatusCode == HttpStatusCode.Unauthorized
                    ? "The email or password is incorrect."
                    : "Sign-in is temporarily unavailable.";

                return ApiResult<LoginResponse>.Failure(error);
            }

            var login = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions, cancellationToken);

            if (login is null)
                return ApiResult<LoginResponse>.Failure("The server returned an invalid response.");

            await _authenticationStateProvider.SignInAsync(login);

            return ApiResult<LoginResponse>.Success(login);
        }
        catch (Exception)
        {
            return ApiResult<LoginResponse>.Failure("Sign-in is temporarily unavailable.");
        }
    }

    public Task LogoutAsync()
    {
        return _authenticationStateProvider.SignOutAsync();
    }
}
