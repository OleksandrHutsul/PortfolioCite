namespace PortfolioCite.Contracts.Portfolio.Models;

public record CertificateDto(string Name, string Issuer, DateOnly IssuedOn, string CredentialUrl);
