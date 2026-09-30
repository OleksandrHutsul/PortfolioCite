namespace PortfolioCite.Domain.Entities;

public class ProfileLanguage
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Proficiency { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
