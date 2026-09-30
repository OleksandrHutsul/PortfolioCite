using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<ExperienceAdminDto>>> GetExperiencesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ExperienceAdminDto>>("api/admin/experience", cancellationToken);
    }

    public Task<ApiResult<ExperienceAdminDto>> CreateExperienceAsync(SaveExperienceRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ExperienceAdminDto>(HttpMethod.Post, "api/admin/experience", request, cancellationToken);
    }

    public Task<ApiResult<ExperienceAdminDto>> UpdateExperienceAsync(int id, SaveExperienceRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ExperienceAdminDto>(HttpMethod.Put, $"api/admin/experience/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteExperienceAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/experience/{id}", cancellationToken);
    }
}
