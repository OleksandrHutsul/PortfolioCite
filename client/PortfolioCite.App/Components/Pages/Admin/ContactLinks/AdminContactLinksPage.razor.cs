using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Components.Pages.Admin.ContactLinks;

public partial class AdminContactLinksPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<ContactLinkAdminDto> Links { get; private set; } = [];
    protected SaveContactLinkRequest Request { get; private set; } = new();
    protected int? EditingId { get; private set; }
    protected bool IsEditing { get; private set; }
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        var result = await AdminApi.GetContactLinksAsync();

        Links = result.Value ?? [];
        LoadError = result.Error;
        IsLoading = false;
    }

    protected void StartCreate()
    {
        EditingId = null;
        Request = new SaveContactLinkRequest();
        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(ContactLinkAdminDto item)
    {
        EditingId = item.Id;
        Request = new SaveContactLinkRequest
        {
            Label = item.Label,
            Url = item.Url,
            IconName = item.IconName,
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
            ? await AdminApi.UpdateContactLinkAsync(id, Request)
            : await AdminApi.CreateContactLinkAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        IsEditing = false;
        await LoadAsync();
    }

    protected async Task DeleteAsync(ContactLinkAdminDto item)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete '{item.Label}'?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteContactLinkAsync(item.Id);

        if (result.IsSuccess)
        {
            Links.Remove(item);
            return;
        }

        LoadError = result.Error;
    }
}
