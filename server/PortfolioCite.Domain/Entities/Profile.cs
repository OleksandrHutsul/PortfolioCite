namespace PortfolioCite.Domain.Entities;

public class Profile
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CurrentFocus { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string? ResumeUrl { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
