using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration;

public class SaveCertificateRequest
{
    [Required, StringLength(160)] 
    public string Name { get; set; } = string.Empty;
    
    [Required, StringLength(120)] 
    public string Issuer { get; set; } = string.Empty;
    public DateOnly IssuedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [Required, Url, StringLength(500)]
    public string CredentialUrl { get; set; } = string.Empty;
}
