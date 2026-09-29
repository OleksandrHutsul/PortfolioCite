using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public class SkillsManagementService : ISkillsManagementService
{
    private readonly IPortfolioRepository _repository;

    public SkillsManagementService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SkillCategoryAdminDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _repository.GetSkillCategoriesAsync(cancellationToken);

        return categories.Select(Map).ToList();
    }

    public async Task<SkillCategoryAdminDto> CreateAsync(SaveSkillCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new SkillCategory();

        Apply(category, request);

        await _repository.AddAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<SkillCategoryAdminDto?> UpdateAsync(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetSkillCategoryForUpdateAsync(id, cancellationToken);
        if (category is null) return null;

        Apply(category, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _repository.GetSkillCategoryForUpdateAsync(id, cancellationToken);
        if (category is null) return false;

        _repository.Remove(category);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void Apply(SkillCategory category, SaveSkillCategoryRequest request)
    {
        category.Name = request.Name.Trim();
        category.DisplayOrder = request.DisplayOrder;

        category.Skills.Clear();
        category.Skills.AddRange(request.Skills.Select(skill => new Skill
        {
            Name = skill.Name.Trim(),
            Description = skill.Description.Trim(),
            Badge = skill.Badge.Trim(),
            IconName = skill.IconName.Trim(),
            AccentColor = skill.AccentColor.Trim(),
            DisplayOrder = skill.DisplayOrder
        }));
    }

    private static SkillCategoryAdminDto Map(SkillCategory category)
    {
        var skills = category.Skills
            .OrderBy(skill => skill.DisplayOrder)
            .Select(skill => new SkillAdminDto(skill.Id, skill.Name, skill.Description, skill.Badge, skill.IconName, skill.AccentColor, skill.DisplayOrder))
            .ToList();

        return new SkillCategoryAdminDto(category.Id, category.Name, category.DisplayOrder, skills);
    }
}