using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<ProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ProjectDto>>("api/admin/projects", cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> GetProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        return GetAsync<ProjectDto>($"api/admin/projects/{id}", cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> CreateProjectAsync(SaveProjectRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProjectDto>(HttpMethod.Post, "api/admin/projects", request, cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> UpdateProjectAsync(int id, SaveProjectRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProjectDto>(HttpMethod.Put, $"api/admin/projects/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/projects/{id}", cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> UploadProjectImageAsync(int id, Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        return UploadAsync<ProjectDto>($"api/admin/projects/{id}/image", content, fileName, contentType, cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> RemoveProjectImageAsync(int id, CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync<ProjectDto>(HttpMethod.Delete, $"api/admin/projects/{id}/image", cancellationToken);
    }
}
