using System.ComponentModel.DataAnnotations;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveProfileRequest
{
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

    [MaxLength(LanguageProficiencies.MaxCount)]
    public List<SaveProfileLanguageRequest> Languages { get; set; } = [];

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; set; } = string.Empty;
}
