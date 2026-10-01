using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Components.Pages.Certificates;

public partial class CertificatesPage : ComponentBase
{
    [Inject] public required PortfolioContentService PortfolioContentService { get; set; }

    protected IReadOnlyList<CertificateDto> Certificates { get; private set; } = [];
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
            var snapshot = await PortfolioContentService.GetSnapshotAsync();
            Certificates = snapshot.Certificates.OrderBy(certificate => certificate.DisplayOrder).ToList();
        }
        catch (Exception)
        {
            LoadError = "Certificates are temporarily unavailable.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
