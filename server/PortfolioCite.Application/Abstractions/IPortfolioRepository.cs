using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Abstractions;

public interface IPortfolioRepository
{
    Task<T?> GetForUpdateAsync<T>(int id, CancellationToken cancellationToken) where T : class;
    Task<List<T>> ListForUpdateAsync<T>(CancellationToken cancellationToken) where T : class;
    Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
    void Remove<T>(T entity) where T : class;

    Task<Profile?> GetProfileAsync(CancellationToken cancellationToken);
    Task<Profile?> GetProfileForUpdateAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProfileFile>> GetProfileFileSummariesAsync(int profileId, CancellationToken cancellationToken);
    Task<ProfileFile?> GetProfileFileForUpdateAsync(int profileId, ProfileFileKind kind, CancellationToken cancellationToken);
    Task<ProfileFile?> GetProfileFileAsync(ProfileFileKind kind, CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken);
    Task<Project?> GetProjectAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Project>> GetAllProjectsForAdminAsync(CancellationToken cancellationToken);
    Task<Project?> GetProjectForAdminAsync(int id, CancellationToken cancellationToken);
    Task<Project?> GetProjectForUpdateAsync(int id, CancellationToken cancellationToken);
    Task<ProjectImage?> GetProjectImageAsync(int projectId, CancellationToken cancellationToken);
    Task<ProjectImage?> GetProjectImageForUpdateAsync(int projectId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, Technology>> GetTechnologiesAsync(IEnumerable<string> names, CancellationToken cancellationToken);

    Task<IReadOnlyList<SkillCategory>> GetSkillCategoriesAsync(CancellationToken cancellationToken);
    Task<SkillCategory?> GetSkillCategoryForUpdateAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Certificate>> GetCertificatesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkExperience>> GetWorkExperiencesAsync(CancellationToken cancellationToken);
    Task<WorkExperience?> GetWorkExperienceForUpdateAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Education>> GetEducationAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ContactLink>> GetContactLinksAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ContactSubmission>> GetContactSubmissionsAsync(CancellationToken cancellationToken);
    Task<ContactSubmission?> GetContactSubmissionAsync(Guid id, CancellationToken cancellationToken);

    Task<Administrator?> GetAdministratorByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<bool> HasAdministratorAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
