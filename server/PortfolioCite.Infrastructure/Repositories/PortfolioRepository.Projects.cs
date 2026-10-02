using Microsoft.EntityFrameworkCore;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Repositories;

public partial class PortfolioRepository
{
    public async Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .OrderBy(project => project.DisplayOrder)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Project?> GetProjectAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllProjectsForAdminAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .OrderBy(project => project.DisplayOrder)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Project?> GetProjectForAdminAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public Task<Project?> GetProjectForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public Task<ProjectImage?> GetProjectImageAsync(int projectId, CancellationToken cancellationToken)
    {
        return _dbContext.ProjectImages
            .AsNoTracking()
            .FirstOrDefaultAsync(image => image.ProjectId == projectId, cancellationToken);
    }

    public Task<ProjectImage?> GetProjectImageForUpdateAsync(int projectId, CancellationToken cancellationToken)
    {
        return _dbContext.ProjectImages
            .FirstOrDefaultAsync(image => image.ProjectId == projectId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Technology>> GetTechnologiesAsync(IEnumerable<string> names, CancellationToken cancellationToken)
    {
        var normalizedNames = names
            .Select(name => name.ToUpperInvariant())
            .Distinct()
            .ToList();

        if (normalizedNames.Count == 0)
            return new Dictionary<string, Technology>(StringComparer.OrdinalIgnoreCase);

        var technologies = await _dbContext.Technologies
            .Where(technology => normalizedNames.Contains(technology.NormalizedName))
            .ToListAsync(cancellationToken);

        return technologies.ToDictionary(technology => technology.Name, StringComparer.OrdinalIgnoreCase);
    }
}
