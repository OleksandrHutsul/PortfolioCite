namespace PortfolioCite.Domain.Entities;

public class WorkExperience
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateOnly StartedOn { get; set; }
    public DateOnly? EndedOn { get; set; }
    public string Summary { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<WorkHighlight> Highlights { get; set; } = [];
}
