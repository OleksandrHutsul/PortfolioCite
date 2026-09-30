using System.ComponentModel.DataAnnotations;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveProfileLanguageRequest
{
    [Required, StringLength(LanguageProficiencies.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Proficiency { get; set; } = string.Empty;

    [Range(0, LanguageProficiencies.MaxDisplayOrder)]
    public int DisplayOrder { get; set; }
}
