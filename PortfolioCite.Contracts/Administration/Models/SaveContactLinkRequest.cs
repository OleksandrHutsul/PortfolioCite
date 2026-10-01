using System.ComponentModel.DataAnnotations;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveContactLinkRequest
{
    [Required, StringLength(80)] 
    public string Label { get; set; } = string.Empty;

    [Required, StringLength(500)]
    [RegularExpression(@"^(?i)\s*(https?://.+|mailto:.*)\s*$", ErrorMessage = "Contact URL must use HTTP, HTTPS, or mailto.")]
    public string Url { get; set; } = string.Empty;

    [Required, StringLength(40), SkillIconName]
    public string IconName { get; set; } = "link";

    public int DisplayOrder { get; set; }
}
