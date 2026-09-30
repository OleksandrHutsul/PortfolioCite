using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<EducationAdminDto>>> GetEducationAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<EducationAdminDto>>("api/admin/education", cancellationToken);
    }

    public Task<ApiResult<EducationAdminDto>> CreateEducationAsync(SaveEducationRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<EducationAdminDto>(HttpMethod.Post, "api/admin/education", request, cancellationToken);
    }

    public Task<ApiResult<EducationAdminDto>> UpdateEducationAsync(int id, SaveEducationRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<EducationAdminDto>(HttpMethod.Put, $"api/admin/education/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteEducationAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/education/{id}", cancellationToken);
    }
}
