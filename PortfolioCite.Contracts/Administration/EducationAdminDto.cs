namespace PortfolioCite.Contracts.Administration;

public record EducationAdminDto(int Id, string Institution, string Degree, string? FieldOfStudy, DateOnly StartedOn, DateOnly? EndedOn, string? Description, 
    int DisplayOrder);
