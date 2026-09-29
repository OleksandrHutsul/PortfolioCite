using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Portfolio;

namespace PortfolioCite.App.Components.Pages;

public partial class HomePage : ComponentBase
{
    [Inject] public required PortfolioContentService PortfolioContentService { get; set; }

    protected PortfolioSnapshotDto? Portfolio { get; private set; }
    protected bool IsLoading { get; private set; }
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
            Portfolio = await PortfolioContentService.GetSnapshotAsync();
        }
        catch (Exception)
        {
            LoadError = "Portfolio content is temporarily unavailable.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected static string FormatRange(WorkExperienceDto experience)
    {
        var end = experience.EndedOn?.ToString("MMM yyyy") ?? "Present";
        return $"{experience.StartedOn:MMM yyyy} - {end}";
    }
}
