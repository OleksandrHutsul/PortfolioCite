using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveExperienceRequest : IValidatableObject
{
    [Required, StringLength(120)]
    public string Company { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Position { get; set; } = string.Empty;

    public DateOnly StartedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EndedOn { get; set; }

    [Required, StringLength(2000)]
    public string Summary { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    [MaxLength(30)]
    public List<string> Highlights { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndedOn < StartedOn)
            yield return new ValidationResult("End date cannot precede the start date.", [nameof(EndedOn)]);

        if (Highlights.Any(value => string.IsNullOrWhiteSpace(value) || value.Trim().Length > 500))
            yield return new ValidationResult("Highlights must contain between 1 and 500 characters.", [nameof(Highlights)]);
    }
}
