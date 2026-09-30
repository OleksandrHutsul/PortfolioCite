using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Components.Pages.Projects;

public partial class ProjectsPage : ComponentBase
{
    [Inject] public required PortfolioContentService PortfolioContentService { get; set; }

    protected IReadOnlyList<ProjectDto> Projects { get; private set; } = [];
    protected bool IsLoading { get; private set; } = true;
    protected string? LoadError { get; private set; }

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        try
        {
            Projects = await PortfolioContentService.GetProjectsAsync();
        }
        catch (Exception)
        {
            LoadError = "Projects are temporarily unavailable.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
