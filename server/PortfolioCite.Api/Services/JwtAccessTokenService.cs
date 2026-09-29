using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortfolioCite.Api.Configuration;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Authentication;
using PortfolioCite.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioCite.Api.Services;

public class JwtAccessTokenService : IAccessTokenService
{
    private readonly JwtOptions _options;

    public JwtAccessTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public LoginResponse CreateToken(Administrator administrator)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, administrator.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, administrator.Email),
            new Claim("role", "Admin")
        };

        var token = new JwtSecurityToken(issuer: _options.Issuer, audience: _options.Audience, claims: claims, expires: expiresAt.UtcDateTime, signingCredentials: credentials);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt, administrator.Email);
    }
}
