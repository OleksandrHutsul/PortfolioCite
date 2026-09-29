namespace PortfolioCite.Contracts.Authentication;

public record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, string Email);
