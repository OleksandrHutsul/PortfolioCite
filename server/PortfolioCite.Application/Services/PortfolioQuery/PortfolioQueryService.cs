using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Models;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.PortfolioQuery;

public class PortfolioQueryService : IPortfolioQueryService
{
    private readonly IPortfolioRepository _repository;

    public PortfolioQueryService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PortfolioSnapshotDto> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileAsync(cancellationToken);

        var categories = await _repository.GetSkillCategoriesAsync(cancellationToken);
        var projects = await _repository.GetProjectsAsync(cancellationToken);
        var certificates = await _repository.GetCertificatesAsync(cancellationToken);
        var experiences = await _repository.GetWorkExperiencesAsync(cancellationToken);
        var education = await _repository.GetEducationAsync(cancellationToken);
        var links = await _repository.GetContactLinksAsync(cancellationToken);

        IReadOnlyList<ProfileFile> files = profile is null
            ? []
            : await _repository.GetProfileFileSummariesAsync(profile.Id, cancellationToken);

        return new PortfolioSnapshotDto(
            profile is null ? null : MapProfile(profile, files),
            categories.Select(MapCategory).ToList(),
            projects.Select(MapProject).ToList(),
            certificates.Select(MapCertificate).ToList(),
            experiences.Select(MapExperience).ToList(),
            education.Select(MapEducation).ToList(),
            links.Select(MapContactLink).ToList());
    }

    public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        var projects = await _repository.GetProjectsAsync(cancellationToken);

        return projects.Select(MapProject).ToList();
    }

    public async Task<ProjectDto?> GetProjectAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _repository.GetPublishedProjectAsync(id, cancellationToken);

        return project is null ? null : MapProject(project);
    }

    public Task<ProfileFileDownload?> GetAvatarAsync(CancellationToken cancellationToken)
    {
        return GetFileAsync(ProfileFileKind.Avatar, cancellationToken);
    }

    public Task<ProfileFileDownload?> GetResumeAsync(CancellationToken cancellationToken)
    {
        return GetFileAsync(ProfileFileKind.Resume, cancellationToken);
    }

    private async Task<ProfileFileDownload?> GetFileAsync(ProfileFileKind kind, CancellationToken cancellationToken)
    {
        var file = await _repository.GetProfileFileAsync(kind, cancellationToken);

        return file is null ? null : new ProfileFileDownload(file.FileName, file.ContentType, file.Content);
    }

    private static ProjectDto MapProject(Project project)
    {
        var technologies = project.ProjectTechnologies
            .Select(item => item.Technology.Name)
            .OrderBy(name => name)
            .ToList();

        return new ProjectDto(project.Id, project.Name, project.ShortDescription, project.Description, project.GitHubUrl, project.LiveUrl, project.ImageUrl, project.DisplayOrder, project.IsFeatured, project.IsPublished, technologies, project.CreatedAt, project.UpdatedAt);
    }

    private static ProfileDto MapProfile(Profile profile, IReadOnlyList<ProfileFile> files)
    {
        var languages = profile.Languages
            .OrderBy(language => language.DisplayOrder)
            .ThenBy(language => language.Id)
            .Select(language => new ProfileLanguageDto(language.Name, language.Proficiency, language.DisplayOrder))
            .ToList();

        return new ProfileDto(profile.FullName, profile.Role, profile.Location, profile.Summary, profile.CurrentFocus, languages, profile.Email,
            ProfileFileLocations.Avatar(files, profile.UpdatedAt), ProfileFileLocations.Resume(files, profile.UpdatedAt));
    }

    private static SkillCategoryDto MapCategory(SkillCategory category)
    {
        var skills = category.Skills
            .OrderBy(skill => skill.DisplayOrder)
            .ThenBy(skill => skill.Name)
            .Select(skill => new SkillDto(skill.Name, skill.Description, skill.Badge, skill.IconName, skill.AccentColor))
            .ToList();

        return new SkillCategoryDto(category.Name, skills);
    }

    private static CertificateDto MapCertificate(Certificate certificate)
    {
        return new CertificateDto(certificate.Name, certificate.Issuer, certificate.IssuedOn, certificate.CredentialUrl);
    }

    private static WorkExperienceDto MapExperience(WorkExperience experience)
    {
        var highlights = experience.Highlights
            .OrderBy(highlight => highlight.DisplayOrder)
            .Select(highlight => highlight.Text)
            .ToList();

        return new WorkExperienceDto(experience.Company, experience.Position, experience.StartedOn, experience.EndedOn, experience.Summary, highlights);
    }

    private static EducationDto MapEducation(Education education)
    {
        return new EducationDto(education.Id, education.Institution, education.Degree, education.FieldOfStudy, education.StartedOn, education.EndedOn, education.Description);
    }

    private static ContactLinkDto MapContactLink(ContactLink link)
    {
        return new ContactLinkDto(link.Label, link.Url, link.IconName);
    }
}
