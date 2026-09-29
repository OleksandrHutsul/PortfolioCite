using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PortfolioCite.Api.Configuration;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Domain.Entities;
using System.Net.Mail;

namespace PortfolioCite.Api.Services;

public class InitialAdminProvisioner
{
    private readonly IPortfolioRepository _repository;
    private readonly IPasswordHasher<Administrator> _passwordHasher;
    private readonly InitialAdminOptions _options;

    public InitialAdminProvisioner(IPortfolioRepository repository, IPasswordHasher<Administrator> passwordHasher, IOptions<InitialAdminOptions> options)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _options = options.Value;
    }

    public async Task ProvisionAsync(CancellationToken cancellationToken)
    {
        if (await _repository.HasAdministratorAsync(cancellationToken)) return;

        var email = _options.Email.Trim();
        var password = _options.Password;

        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password)) return;

        if (string.IsNullOrEmpty(email) || password.Length < 12 || !MailAddress.TryCreate(email, out _))
        {
            throw new InvalidOperationException("InitialAdmin requires a valid email and a password of at least 12 characters.");
        }

        var administrator = new Administrator
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        administrator.PasswordHash = _passwordHasher.HashPassword(administrator, password);

        await _repository.AddAsync(administrator, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
