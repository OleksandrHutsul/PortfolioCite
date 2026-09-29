using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration;

public class SaveContactLinkRequest
{
    [Required, StringLength(80)] 
    public string Label { get; set; } = string.Empty;

    [Required, StringLength(500)]
    [RegularExpression(@"^(?i)\s*(https?://.+|mailto:.*)\s*$", ErrorMessage = "Contact URL must use HTTP, HTTPS, or mailto.")]
    public string Url { get; set; } = string.Empty;

    [Required, StringLength(40)] 
    public string IconName { get; set; } = string.Empty;

    [Range(0, int.MaxValue)] 
    public int DisplayOrder { get; set; }
}
