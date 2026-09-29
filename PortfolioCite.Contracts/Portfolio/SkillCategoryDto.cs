namespace PortfolioCite.Contracts.Portfolio;

public record SkillCategoryDto(string Name, IReadOnlyList<SkillDto> Skills);
