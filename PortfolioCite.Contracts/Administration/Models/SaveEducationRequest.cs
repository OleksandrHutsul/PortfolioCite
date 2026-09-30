using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveEducationRequest : IValidatableObject
{
    [Required, StringLength(160)]
    public string Institution { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Degree { get; set; } = string.Empty;

    [StringLength(160)]
    public string? FieldOfStudy { get; set; }

    public DateOnly StartedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? EndedOn { get; set; }

    [StringLength(1200)]
    public string? Description { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndedOn < StartedOn)
            yield return new ValidationResult("End date cannot precede the start date.", [nameof(EndedOn)]);
    }
}
