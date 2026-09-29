namespace PortfolioCite.Domain.Entities;

public class SkillCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<Skill> Skills { get; set; } = [];
}
