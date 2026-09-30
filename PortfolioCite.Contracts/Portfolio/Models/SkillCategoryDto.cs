namespace PortfolioCite.Contracts.Portfolio.Models;

public record SkillCategoryDto(string Name, IReadOnlyList<SkillDto> Skills);
