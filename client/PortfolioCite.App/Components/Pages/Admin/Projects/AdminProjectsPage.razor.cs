using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Portfolio;
using PortfolioCite.Contracts.Projects;

namespace PortfolioCite.App.Components.Pages.Admin.Projects;

public partial class AdminProjectsPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<ProjectDto> Projects { get; private set; } = [];
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? Error { get; private set; }

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        Error = null;

        var result = await AdminApi.GetProjectsAsync();

        Projects = result.Value ?? [];
        Error = result.Error;
        IsLoading = false;
    }

    protected async Task TogglePublishAsync(ProjectDto project)
    {
        if (IsSaving) return;

        IsSaving = true;
        Error = null;

        var result = await AdminApi.UpdateProjectAsync(project.Id, ToRequest(project, !project.IsPublished));

        IsSaving = false;

        if (!result.IsSuccess)
        {
            Error = result.Error;
            return;
        }

        if (result.Value is null) return;

        var index = Projects.FindIndex(item => item.Id == project.Id);

        if (index >= 0)
            Projects[index] = result.Value;
    }

    protected async Task DeleteAsync(ProjectDto project)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete '{project.Name}'? This cannot be undone.");
        if (!confirmed) return;

        var result = await AdminApi.DeleteProjectAsync(project.Id);

        if (result.IsSuccess)
        {
            Projects.Remove(project);
            return;
        }

        Error = result.Error;
    }

    private static SaveProjectRequest ToRequest(ProjectDto project, bool isPublished)
    {
        return new SaveProjectRequest
        {
            Name = project.Name,
            ShortDescription = project.ShortDescription,
            Description = project.Description,
            GitHubUrl = project.GitHubUrl,
            LiveUrl = project.LiveUrl,
            ImageUrl = project.ImageUrl,
            DisplayOrder = project.DisplayOrder,
            IsFeatured = project.IsFeatured,
            IsPublished = isPublished,
            Technologies = project.Technologies
        };
    }
}
