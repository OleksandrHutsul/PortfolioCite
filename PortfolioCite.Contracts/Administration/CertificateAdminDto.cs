namespace PortfolioCite.Contracts.Administration;

public record CertificateAdminDto(int Id, string Name, string Issuer, DateOnly IssuedOn, string CredentialUrl);
