using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Education;

public partial class AdminEducationPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<EducationAdminDto> EducationItems { get; private set; } = [];
    protected SaveEducationRequest Request { get; private set; } = new();
    protected int? EditingId { get; private set; }
    protected bool IsEditing { get; private set; }
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }
    protected int OrderItemCount => DisplayOrderRules.ItemCount(EducationItems.Count, EditingId is null);

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        var result = await AdminApi.GetEducationAsync();

        EducationItems = result.Value ?? [];
        LoadError = result.Error;
        IsLoading = false;
    }

    protected void StartCreate()
    {
        EditingId = null;
        Request = new SaveEducationRequest
        {
            DisplayOrder = DisplayOrderRules.Next(EducationItems.Count)
        };
        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(EducationAdminDto item)
    {
        EditingId = item.Id;
        Request = new SaveEducationRequest
        {
            Institution = item.Institution,
            Degree = item.Degree,
            FieldOfStudy = item.FieldOfStudy,
            StartedOn = item.StartedOn,
            EndedOn = item.EndedOn,
            Description = item.Description,
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
            ? await AdminApi.UpdateEducationAsync(id, Request)
            : await AdminApi.CreateEducationAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        IsEditing = false;
        await LoadAsync();
    }

    protected async Task DeleteAsync(EducationAdminDto item)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete '{item.Degree}' at {item.Institution}?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteEducationAsync(item.Id);

        if (result.IsSuccess)
        {
            EducationItems.Remove(item);
            EducationItems = DisplayOrderRules.Renumber(EducationItems, education => education.DisplayOrder, (education, order) => education with { DisplayOrder = order });
            return;
        }

        LoadError = result.Error;
    }
}
