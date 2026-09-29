using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Application.Services.Content;

public interface ISkillsManagementService
{
    Task<IReadOnlyList<SkillCategoryAdminDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<SkillCategoryAdminDto> CreateAsync(SaveSkillCategoryRequest request, CancellationToken cancellationToken);
    Task<SkillCategoryAdminDto?> UpdateAsync(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
