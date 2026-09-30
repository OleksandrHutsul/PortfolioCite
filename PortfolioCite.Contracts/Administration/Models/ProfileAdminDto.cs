namespace PortfolioCite.Contracts.Administration.Models;

public record ProfileAdminDto(string FullName, string Role, string Location, string Summary, string CurrentFocus, IReadOnlyList<ProfileLanguageAdminDto> Languages,
    string Email, string? AvatarUrl, string? ResumeUrl, DateTimeOffset UpdatedAt);
