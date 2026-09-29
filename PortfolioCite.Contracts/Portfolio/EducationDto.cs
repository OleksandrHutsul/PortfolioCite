namespace PortfolioCite.Contracts.Portfolio;

public record EducationDto(int Id, string Institution, string Degree, string? FieldOfStudy, DateOnly StartedOn, DateOnly? EndedOn, string? Description);
