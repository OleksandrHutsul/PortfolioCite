using System.Net.Http.Headers;

namespace PortfolioCite.App.Services;

public class AuthorizedHttpHandler : DelegatingHandler
{
    private readonly AuthTokenStore _tokenStore;

    public AuthorizedHttpHandler(AuthTokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
