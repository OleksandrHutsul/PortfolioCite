namespace PortfolioCite.Domain.Entities;

public class WorkHighlight
{
    public int Id { get; set; }
    public int WorkExperienceId { get; set; }
    public WorkExperience WorkExperience { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
