using Microsoft.AspNetCore.Identity;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Authentication;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IPortfolioRepository _repository;
    private readonly IPasswordHasher<Administrator> _passwordHasher;
    private readonly IAccessTokenService _accessTokenService;
    private readonly Administrator _dummyAdministrator = new();
    private readonly string _dummyPasswordHash;

    public AuthenticationService(IPortfolioRepository repository, IPasswordHasher<Administrator> passwordHasher, IAccessTokenService accessTokenService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
        _dummyPasswordHash = passwordHasher.HashPassword(_dummyAdministrator, Guid.NewGuid().ToString("N"));
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var administrator = await _repository.GetAdministratorByEmailAsync(normalizedEmail, cancellationToken);

        if (administrator is null)
        {
            _passwordHasher.VerifyHashedPassword(_dummyAdministrator, _dummyPasswordHash, request.Password);
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(administrator, administrator.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            administrator.PasswordHash = _passwordHasher.HashPassword(administrator, request.Password);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        return _accessTokenService.CreateToken(administrator);
    }
}
