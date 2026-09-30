using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Contracts.Portfolio.Models;

public record ProfileDto(string FullName, string Role,  string Location, string Summary, string CurrentFocus, IReadOnlyList<ProfileLanguageDto> Languages,
    string Email, string? AvatarUrl, string? ResumeUrl);
