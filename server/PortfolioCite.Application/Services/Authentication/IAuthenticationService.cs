using PortfolioCite.Contracts.Authentication;

namespace PortfolioCite.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
