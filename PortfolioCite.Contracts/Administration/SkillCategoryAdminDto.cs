namespace PortfolioCite.Contracts.Administration;

public record SkillCategoryAdminDto(int Id, string Name, int DisplayOrder, IReadOnlyList<SkillAdminDto> Skills);
