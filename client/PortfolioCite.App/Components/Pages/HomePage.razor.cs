using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Portfolio.Models;

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

    protected static string Initials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
            return "?";

        if (parts.Length == 1)
            return FirstLetter(parts[0]).ToString();

        return string.Concat(FirstLetter(parts[0]), FirstLetter(parts[^1]));
    }

    protected static string FormatRange(WorkExperienceDto experience)
    {
        var end = experience.EndedOn?.ToString("MMM yyyy") ?? "Present";
        return $"{experience.StartedOn:MMM yyyy} - {end}";
    }

    private static char FirstLetter(string value)
    {
        foreach (var character in value)
        {
            if (char.IsLetter(character))
                return char.ToUpperInvariant(character);
        }

        return value[0];
    }
}
