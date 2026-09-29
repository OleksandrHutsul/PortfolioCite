namespace PortfolioCite.Domain.Entities;

public class Skill
{
    public int Id { get; set; }
    public int SkillCategoryId { get; set; }
    public SkillCategory SkillCategory { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string IconName { get; set; } = "code";
    public string AccentColor { get; set; } = "#58a6ff";
    public int DisplayOrder { get; set; }
}
