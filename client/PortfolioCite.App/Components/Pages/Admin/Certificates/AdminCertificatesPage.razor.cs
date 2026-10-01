using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Certificates;

public partial class AdminCertificatesPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<CertificateAdminDto> Certificates { get; private set; } = [];
    protected SaveCertificateRequest Request { get; private set; } = new();
    protected int? EditingId { get; private set; }
    protected bool IsEditing { get; private set; }
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }
    protected int OrderItemCount => DisplayOrderRules.ItemCount(Certificates.Count, EditingId is null);

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        var result = await AdminApi.GetCertificatesAsync();

        Certificates = result.Value ?? [];
        LoadError = result.Error;
        IsLoading = false;
    }

    protected void StartCreate()
    {
        EditingId = null;
        Request = new SaveCertificateRequest
        {
            DisplayOrder = DisplayOrderRules.Next(Certificates.Count)
        };
        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(CertificateAdminDto item)
    {
        EditingId = item.Id;
        Request = new SaveCertificateRequest
        {
            Name = item.Name,
            Issuer = item.Issuer,
            IssuedOn = item.IssuedOn,
            CredentialUrl = item.CredentialUrl,
            DisplayOrder = item.DisplayOrder
        };

        SaveError = null;
        IsEditing = true;
    }

    protected void CancelEdit()
    {
        IsEditing = false;
    }

    protected async Task SaveAsync()
    {
        IsSaving = true;
        SaveError = null;

        var result = EditingId is int id
            ? await AdminApi.UpdateCertificateAsync(id, Request)
            : await AdminApi.CreateCertificateAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        IsEditing = false;
        await LoadAsync();
    }

    protected async Task DeleteAsync(CertificateAdminDto item)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete '{item.Name}'?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteCertificateAsync(item.Id);

        if (result.IsSuccess)
        {
            Certificates.Remove(item);
            Certificates = DisplayOrderRules.Renumber(Certificates, certificate => certificate.DisplayOrder, (certificate, order) => certificate with { DisplayOrder = order });
            return;
        }

        LoadError = result.Error;
    }
}
