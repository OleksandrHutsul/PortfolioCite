using Microsoft.EntityFrameworkCore;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Domain.Entities;
using PortfolioCite.Infrastructure.Data;

namespace PortfolioCite.Infrastructure.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _dbContext;

    public PortfolioRepository(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<T?> GetForUpdateAsync<T>(int id, CancellationToken cancellationToken) where T : class
    {
        return _dbContext.Set<T>().FindAsync([id], cancellationToken).AsTask();
    }

    public Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        return _dbContext.Set<T>().AddAsync(entity, cancellationToken).AsTask();
    }

    public void Remove<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public Task<Profile?> GetProfileAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Profiles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Profile?> GetProfileForUpdateAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Profiles.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SkillCategory>> GetSkillCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SkillCategories
            .AsNoTracking()
            .Include(category => category.Skills)
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<SkillCategory?> GetSkillCategoryForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.SkillCategories
            .Include(category => category.Skills)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(project => project.IsPublished)
            .Include(project => project.ProjectTechnologies)
                .ThenInclude(projectTechnology => projectTechnology.Technology)
            .OrderBy(project => project.DisplayOrder)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Project?> GetPublishedProjectAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Where(project => project.IsPublished)
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

    public async Task<IReadOnlyList<Certificate>> GetCertificatesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Certificates
            .AsNoTracking()
            .OrderByDescending(certificate => certificate.IssuedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkExperience>> GetWorkExperiencesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.WorkExperiences
            .AsNoTracking()
            .Include(experience => experience.Highlights)
            .OrderBy(experience => experience.DisplayOrder)
            .ThenByDescending(experience => experience.StartedOn)
            .ToListAsync(cancellationToken);
    }

    public Task<WorkExperience?> GetWorkExperienceForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.WorkExperiences
            .Include(experience => experience.Highlights)
            .FirstOrDefaultAsync(experience => experience.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Education>> GetEducationAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Education
            .AsNoTracking()
            .OrderBy(education => education.DisplayOrder)
            .ThenByDescending(education => education.StartedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactLink>> GetContactLinksAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ContactLinks
            .AsNoTracking()
            .OrderBy(link => link.DisplayOrder)
            .ThenBy(link => link.Label)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactSubmission>> GetContactSubmissionsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ContactSubmissions
            .AsNoTracking()
            .OrderBy(submission => submission.IsRead)
            .ThenByDescending(submission => submission.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<ContactSubmission?> GetContactSubmissionAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.ContactSubmissions.FirstOrDefaultAsync(submission => submission.Id == id, cancellationToken);
    }

    public Task<Administrator?> GetAdministratorByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Administrators.FirstOrDefaultAsync(
            administrator => administrator.NormalizedEmail == normalizedEmail,
            cancellationToken);
    }

    public Task<bool> HasAdministratorAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Administrators.AnyAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
