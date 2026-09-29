namespace PortfolioCite.Contracts.Portfolio;

public record CertificateDto(string Name, string Issuer, DateOnly IssuedOn, string CredentialUrl);
