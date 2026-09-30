using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects;

namespace PortfolioCite.Application.Services.Projects;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllForAdminAsync(CancellationToken cancellationToken);
    Task<ProjectDto?> GetForAdminAsync(int id, CancellationToken cancellationToken);
    Task<ProjectDto> CreateAsync(SaveProjectRequest request, CancellationToken cancellationToken);
    Task<ProjectDto?> UpdateAsync(int id, SaveProjectRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
