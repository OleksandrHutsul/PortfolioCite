namespace PortfolioCite.Contracts.Administration.Models;

public record ExperienceAdminDto(int Id, string Company, string Position, DateOnly StartedOn, DateOnly? EndedOn, string Summary, int DisplayOrder, IReadOnlyList<string> Highlights);
