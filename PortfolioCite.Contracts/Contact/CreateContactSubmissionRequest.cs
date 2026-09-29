using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Contact;

public class CreateContactSubmissionRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(160, MinimumLength = 3)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(5000, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Website { get; set; }
}
