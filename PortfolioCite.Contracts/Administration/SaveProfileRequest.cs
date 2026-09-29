using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration;

public class SaveProfileRequest
{
    private const string WebOrRelativeUrl = @"^(?i)\s*(https?://.+|/(?!/)[^\r\n\\]*|(?!/|\\)[^\r\n:\\]+)?\s*$";

    [Required, StringLength(120)] 
    public string FullName { get; set; } = string.Empty;
    
    [Required, StringLength(120)] 
    public string Role { get; set; } = string.Empty;
    
    [Required, StringLength(160)] 
    public string Location { get; set; } = string.Empty;
    
    [Required, StringLength(4000)] 
    public string Summary { get; set; } = string.Empty;
    
    [Required, StringLength(1200)] 
    public string CurrentFocus { get; set; } = string.Empty;
    
    [Required, StringLength(500)]
    public string Languages { get; set; } = string.Empty;
    
    [Required, EmailAddress, StringLength(320)]
    public string Email { get; set; } = string.Empty;
    
    [Required, StringLength(500), RegularExpression(WebOrRelativeUrl, ErrorMessage = "Avatar URL must be relative or use HTTP/HTTPS.")]
    public string AvatarUrl { get; set; } = string.Empty;

    [StringLength(500), RegularExpression(WebOrRelativeUrl, ErrorMessage = "Resume URL must be relative or use HTTP/HTTPS.")]
    public string? ResumeUrl { get; set; }
}
