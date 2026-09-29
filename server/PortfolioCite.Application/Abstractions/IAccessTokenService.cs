using PortfolioCite.Contracts.Authentication;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Abstractions;

public interface IAccessTokenService
{
    LoginResponse CreateToken(Administrator administrator);
}
