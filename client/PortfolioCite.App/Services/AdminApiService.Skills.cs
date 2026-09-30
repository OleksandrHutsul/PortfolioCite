using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<SkillCategoryAdminDto>>> GetSkillCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<SkillCategoryAdminDto>>("api/admin/skill-categories", cancellationToken);
    }

    public Task<ApiResult<SkillCategoryAdminDto>> CreateSkillCategoryAsync(SaveSkillCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<SkillCategoryAdminDto>(HttpMethod.Post, "api/admin/skill-categories", request, cancellationToken);
    }

    public Task<ApiResult<SkillCategoryAdminDto>> UpdateSkillCategoryAsync(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<SkillCategoryAdminDto>(HttpMethod.Put, $"api/admin/skill-categories/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteSkillCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/skill-categories/{id}", cancellationToken);
    }
}
