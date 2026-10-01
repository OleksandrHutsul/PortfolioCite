using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Experience;

public partial class AdminExperiencePage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<ExperienceAdminDto> Experiences { get; private set; } = [];
    protected SaveExperienceRequest Request { get; private set; } = new();
    protected bool IsCurrent { get; private set; }
    protected string HighlightsText { get; set; } = string.Empty;
    protected int? EditingId { get; private set; }
    protected bool IsEditing { get; private set; }
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }
    protected int OrderItemCount => DisplayOrderRules.ItemCount(Experiences.Count, EditingId is null);

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        var result = await AdminApi.GetExperiencesAsync();

        Experiences = result.Value ?? [];
        LoadError = result.Error;
        IsLoading = false;
    }

    protected void StartCreate()
    {
        EditingId = null;
        Request = new SaveExperienceRequest
        {
            DisplayOrder = DisplayOrderRules.Next(Experiences.Count)
        };
        IsCurrent = false;
        HighlightsText = string.Empty;
        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(ExperienceAdminDto item)
    {
        EditingId = item.Id;
        Request = new SaveExperienceRequest
        {
            Company = item.Company,
            Position = item.Position,
            StartedOn = item.StartedOn,
            EndedOn = item.EndedOn,
            Summary = item.Summary,
            DisplayOrder = item.DisplayOrder
        };
        IsCurrent = item.EndedOn is null;
        HighlightsText = string.Join(Environment.NewLine, item.Highlights);
        SaveError = null;
        IsEditing = true;
    }

    protected void SetCurrent(ChangeEventArgs args)
    {
        IsCurrent = args.Value is true;

        if (IsCurrent)
            Request.EndedOn = null;
    }

    protected void CancelEdit()
    {
        IsEditing = false;
    }

    protected async Task SaveAsync()
    {
        if (IsCurrent)
            Request.EndedOn = null;

        Request.Highlights = HighlightsText
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        IsSaving = true;
        SaveError = null;

        var result = EditingId is int id
            ? await AdminApi.UpdateExperienceAsync(id, Request)
            : await AdminApi.CreateExperienceAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        IsEditing = false;
        await LoadAsync();
    }

    protected async Task DeleteAsync(ExperienceAdminDto item)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete the {item.Position} role at {item.Company}?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteExperienceAsync(item.Id);

        if (!result.IsSuccess)
        {
            LoadError = result.Error;
            return;
        }

        Experiences.Remove(item);
    }
}
