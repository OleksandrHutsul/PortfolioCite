using PortfolioCite.Contracts.Portfolio;

namespace PortfolioCite.Application.Services.PortfolioQuery;

public interface IPortfolioQueryService
{
    Task<PortfolioSnapshotDto> GetSnapshotAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken);
    Task<ProjectDto?> GetProjectAsync(int id, CancellationToken cancellationToken);
}
