using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Application.Services.PortfolioQuery;

public interface IPortfolioQueryService
{
    Task<PortfolioSnapshotDto> GetSnapshotAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken);
    Task<ProjectDto?> GetProjectAsync(int id, CancellationToken cancellationToken);
    Task<ProfileFileDownload?> GetAvatarAsync(CancellationToken cancellationToken);
    Task<ProfileFileDownload?> GetResumeAsync(CancellationToken cancellationToken);
    Task<ProfileFileDownload?> GetProjectImageAsync(int projectId, CancellationToken cancellationToken);
}
