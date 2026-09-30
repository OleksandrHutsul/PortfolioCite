namespace PortfolioCite.Contracts.Administration.Models;

public record SkillCategoryAdminDto(int Id, string Name, int DisplayOrder, IReadOnlyList<SkillAdminDto> Skills);
