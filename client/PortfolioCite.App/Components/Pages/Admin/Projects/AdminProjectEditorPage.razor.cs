using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects;

namespace PortfolioCite.App.Components.Pages.Admin.Projects;

public partial class AdminProjectEditorPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    [Parameter] public int? Id { get; set; }

    protected SaveProjectRequest Request { get; private set; } = new();
    protected string TechnologyNames { get; set; } = string.Empty;
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }
    protected int OrderItemCount { get; private set; } = 1;

    protected override Task OnParametersSetAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        LoadError = null;
        SaveError = null;
        IsLoading = true;

        var projects = await AdminApi.GetProjectsAsync();

        if (!projects.IsSuccess)
        {
            LoadError = projects.Error;
            IsLoading = false;
            return;
        }

        var existingCount = projects.Value?.Count ?? 0;

        if (Id is null)
        {
            OrderItemCount = DisplayOrderRules.ItemCount(existingCount, includesNewItem: true);
            Request = new SaveProjectRequest
            {
                DisplayOrder = DisplayOrderRules.Next(existingCount)
            };
            TechnologyNames = string.Empty;
            IsLoading = false;
            return;
        }

        var result = await AdminApi.GetProjectAsync(Id.Value);

        IsLoading = false;

        if (result.Value is null)
        {
            LoadError = result.Error ?? "The project was not found.";
            return;
        }

        OrderItemCount = DisplayOrderRules.ItemCount(existingCount, includesNewItem: false);
        Request = ToRequest(result.Value);
        TechnologyNames = string.Join(", ", result.Value.Technologies);
    }

    protected async Task SaveAsync()
    {
        if (IsSaving) return;

        IsSaving = true;
        SaveError = null;

        Request.Technologies = TechnologyNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var result = Id is int id
            ? await AdminApi.UpdateProjectAsync(id, Request)
            : await AdminApi.CreateProjectAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        Navigation.NavigateTo("/admin/projects");
    }

    private static SaveProjectRequest ToRequest(ProjectDto project)
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
            IsPublished = project.IsPublished
        };
    }
}
