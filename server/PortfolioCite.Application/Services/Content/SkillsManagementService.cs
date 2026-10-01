using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Validation;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Rules;
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
        var categories = await _repository.ListForUpdateAsync<SkillCategory>(cancellationToken);
        EnsureValid(request, categories.Count + 1);

        var category = new SkillCategory();

        DisplayOrderEditor.Insert(categories, category, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(category, request);

        await _repository.AddAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<SkillCategoryAdminDto?> UpdateAsync(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetSkillCategoryForUpdateAsync(id, cancellationToken);
        if (category is null) return null;

        var categories = await _repository.ListForUpdateAsync<SkillCategory>(cancellationToken);
        EnsureValid(request, categories.Count);

        DisplayOrderEditor.Move(categories, category, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(category, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _repository.GetSkillCategoryForUpdateAsync(id, cancellationToken);
        if (category is null) return false;

        var categories = await _repository.ListForUpdateAsync<SkillCategory>(cancellationToken);

        _repository.Remove(category);
        DisplayOrderEditor.CloseGap(categories, category, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void EnsureValid(SaveSkillCategoryRequest request, int categoryItemCount)
    {
        var validator = new ContentValidator();
        validator.AddDisplayOrder(request.DisplayOrder, categoryItemCount);
        validator.AddDisplayOrderSequence(request.Skills.Select(skill => skill.DisplayOrder).ToList(), index => $"Skills[{index}].DisplayOrder");

        for (var index = 0; index < request.Skills.Count; index++)
            validator.AddSkillIcon(request.Skills[index].IconName, $"Skills[{index}].IconName");

        validator.ThrowIfInvalid();
    }

    private static void Apply(SkillCategory category, SaveSkillCategoryRequest request)
    {
        category.Name = request.Name.Trim();

        category.Skills.Clear();
        category.Skills.AddRange(request.Skills.Select(skill => new Skill
        {
            Name = skill.Name.Trim(),
            Description = skill.Description.Trim(),
            Badge = skill.Badge.Trim(),
            IconName = SkillIcons.Canonical(skill.IconName) ?? skill.IconName.Trim(),
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