namespace PortfolioCite.Contracts.Administration.Models;

public record CertificateAdminDto(int Id, string Name, string Issuer, DateOnly IssuedOn, string CredentialUrl);
