using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Components.Pages.Admin.Skills;

public partial class AdminSkillsPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<SkillCategoryAdminDto> Categories { get; private set; } = [];
    protected SaveSkillCategoryRequest Request { get; private set; } = new();
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

        var result = await AdminApi.GetSkillCategoriesAsync();

        Categories = result.Value ?? [];
        LoadError = result.Error;
        IsLoading = false;
    }

    protected void StartCreate()
    {
        EditingId = null;
        Request = new SaveSkillCategoryRequest
        {
            Skills = [new SaveSkillRequest()]
        };

        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(SkillCategoryAdminDto category)
    {
        EditingId = category.Id;
        Request = ToRequest(category);
        SaveError = null;
        IsEditing = true;
    }

    protected void AddSkill()
    {
        Request.Skills.Add(new SaveSkillRequest
        {
            DisplayOrder = Request.Skills.Count
        });
    }

    protected void RemoveSkill(SaveSkillRequest skill)
    {
        Request.Skills.Remove(skill);
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
            ? await AdminApi.UpdateSkillCategoryAsync(id, Request)
            : await AdminApi.CreateSkillCategoryAsync(Request);

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveError = result.Error;
            return;
        }

        IsEditing = false;
        await LoadAsync();
    }

    protected async Task DeleteAsync(SkillCategoryAdminDto category)
    {
        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", $"Delete '{category.Name}' and all its skills?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteSkillCategoryAsync(category.Id);

        if (result.IsSuccess)
        {
            Categories.Remove(category);
            return;
        }

        LoadError = result.Error;
    }

    private static SaveSkillCategoryRequest ToRequest(SkillCategoryAdminDto category)
    {
        return new SaveSkillCategoryRequest
        {
            Name = category.Name,
            DisplayOrder = category.DisplayOrder,
            Skills = category.Skills.Select(skill => new SaveSkillRequest
            {
                Name = skill.Name,
                Description = skill.Description,
                Badge = skill.Badge,
                IconName = skill.IconName,
                AccentColor = skill.AccentColor,
                DisplayOrder = skill.DisplayOrder
            }).ToList()
        };
    }
}
