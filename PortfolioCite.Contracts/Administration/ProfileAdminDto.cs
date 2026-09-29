namespace PortfolioCite.Contracts.Administration;

public record ProfileAdminDto(string FullName, string Role, string Location, string Summary, string CurrentFocus, string Languages,
    string Email, string AvatarUrl, string? ResumeUrl, DateTimeOffset UpdatedAt);
