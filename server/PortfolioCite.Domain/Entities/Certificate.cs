namespace PortfolioCite.Domain.Entities;

public class Certificate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateOnly IssuedOn { get; set; }
    public string CredentialUrl { get; set; } = string.Empty;
}
