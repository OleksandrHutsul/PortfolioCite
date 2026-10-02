using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Validation;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects.Models;
using PortfolioCite.Contracts.Projects.Rules;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Projects;

public partial class ProjectService : IProjectService
{
    private readonly IPortfolioRepository _repository;

    public ProjectService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllForAdminAsync(CancellationToken cancellationToken)
    {
        var projects = await _repository.GetAllProjectsForAdminAsync(cancellationToken);
        return projects.Select(Map).ToList();
    }

    public async Task<ProjectDto?> GetForAdminAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _repository.GetProjectForAdminAsync(id, cancellationToken);
        return project is null ? null : Map(project);
    }

    public async Task<ProjectDto> CreateAsync(SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var projects = await _repository.ListForUpdateAsync<Project>(cancellationToken);
        EnsureCanBeVisible(projects, current: null, request.IsPublished);

        var now = DateTimeOffset.UtcNow;
        var project = new Project
        {
            CreatedAt = now,
            UpdatedAt = now
        };

        DisplayOrderEditor.Insert(projects, project, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        await ApplyAsync(project, request, cancellationToken);

        await _repository.AddAsync(project, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(project);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetProjectForUpdateAsync(id, cancellationToken);
        if (project is null) return null;

        var projects = await _repository.ListForUpdateAsync<Project>(cancellationToken);
        EnsureCanBeVisible(projects, project, request.IsPublished);

        DisplayOrderEditor.Move(projects, project, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        await ApplyAsync(project, request, cancellationToken);

        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(project);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _repository.GetProjectForUpdateAsync(id, cancellationToken);
        if (project is null) return false;

        var projects = await _repository.ListForUpdateAsync<Project>(cancellationToken);

        _repository.Remove(project);
        DisplayOrderEditor.CloseGap(projects, project, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task ApplyAsync(Project project, SaveProjectRequest request, CancellationToken cancellationToken)
    {
        project.Name = request.Name.Trim();
        project.ShortDescription = request.ShortDescription.Trim();
        project.Description = request.Description.Trim();
        project.GitHubUrl = NullIfWhiteSpace(request.GitHubUrl);
        project.LiveUrl = NullIfWhiteSpace(request.LiveUrl);
        project.IsFeatured = request.IsFeatured;
        project.IsPublished = request.IsPublished;

        var technologyNames = request.Technologies
            .Select(name => name.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var technologies = await _repository.GetTechnologiesAsync(technologyNames, cancellationToken);
        var desiredNames = technologyNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        project.ProjectTechnologies.RemoveAll(item => !desiredNames.Contains(item.Technology.Name));

        foreach (var name in technologyNames)
        {
            var alreadyAdded = project.ProjectTechnologies.Any(item => item.Technology.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (alreadyAdded) continue;

            var technology = technologies.GetValueOrDefault(name) ?? new Technology
            {
                Name = name,
                NormalizedName = name.ToUpperInvariant()
            };

            project.ProjectTechnologies.Add(new ProjectTechnology
            {
                Technology = technology
            });
        }
    }

    private static ProjectDto Map(Project project)
    {
        var technologies = project.ProjectTechnologies
            .Select(item => item.Technology.Name)
            .OrderBy(name => name)
            .ToList();

        return new ProjectDto(project.Id, project.Name, project.ShortDescription, project.Description, project.GitHubUrl, project.LiveUrl, project.ImageUrl, project.DisplayOrder, project.IsFeatured, project.IsPublished, technologies, project.CreatedAt, project.UpdatedAt);
    }

    private static void EnsureCanBeVisible(IEnumerable<Project> projects, Project? current, bool makeVisible)
    {
        if (!makeVisible || current is { IsPublished: true })
            return;

        var visibleCount = projects.Count(project => project.IsPublished && !ReferenceEquals(project, current));

        if (visibleCount < ProjectVisibilityRules.MaxVisible)
            return;

        throw new ContentValidationException(new Dictionary<string, string[]>
        {
            [nameof(SaveProjectRequest.IsPublished)] = [ProjectVisibilityRules.LimitMessage]
        });
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
