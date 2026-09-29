namespace PortfolioCite.Contracts.Portfolio;

public record WorkExperienceDto(string Company, string Position, DateOnly StartedOn, DateOnly? EndedOn, string Summary, IReadOnlyList<string> Highlights);
