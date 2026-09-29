namespace PortfolioCite.Contracts.Portfolio;

public record ProfileDto(string FullName, string Role, string Location, string Summary, string CurrentFocus, string Languages, string Email, string AvatarUrl,
    string? ResumeUrl);
