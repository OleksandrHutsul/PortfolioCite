namespace PortfolioCite.Domain.Entities;

public class Education
{
    public int Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public DateOnly StartedOn { get; set; }
    public DateOnly? EndedOn { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}
