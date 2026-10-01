using Microsoft.EntityFrameworkCore;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Repositories;

public partial class PortfolioRepository
{
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

    public async Task<IReadOnlyList<Certificate>> GetCertificatesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Certificates
            .AsNoTracking()
            .OrderBy(certificate => certificate.DisplayOrder)
            .ThenByDescending(certificate => certificate.IssuedOn)
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
}
