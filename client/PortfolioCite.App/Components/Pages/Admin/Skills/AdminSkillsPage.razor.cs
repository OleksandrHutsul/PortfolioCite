using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

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
    protected int CategoryOrderItemCount => DisplayOrderRules.ItemCount(Categories.Count, EditingId is null);
    protected int SkillOrderItemCount => DisplayOrderRules.ItemCount(Request.Skills.Count, includesNewItem: false);

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
            DisplayOrder = DisplayOrderRules.Next(Categories.Count),
            Skills = [new SaveSkillRequest()]
        };

        SaveError = null;
        IsEditing = true;
    }

    protected void StartEdit(SkillCategoryAdminDto category)
    {
        EditingId = category.Id;
        Request = ToRequest(category);
        DisplayOrderRules.Normalize(Request.Skills, skill => skill.DisplayOrder, (skill, order) => skill.DisplayOrder = order);
        SaveError = null;
        IsEditing = true;
    }

    protected void AddSkill()
    {
        Request.Skills.Add(new SaveSkillRequest
        {
            DisplayOrder = DisplayOrderRules.Next(Request.Skills.Count)
        });

        DisplayOrderRules.Normalize(Request.Skills, skill => skill.DisplayOrder, (skill, order) => skill.DisplayOrder = order);
    }

    protected void ChangeSkillOrder(SaveSkillRequest skill, int order)
    {
        DisplayOrderRules.TryMove(Request.Skills, skill, order, item => item.DisplayOrder, (item, value) => item.DisplayOrder = value);
    }

    protected void RemoveSkill(SaveSkillRequest skill)
    {
        DisplayOrderRules.Remove(Request.Skills, skill, item => item.DisplayOrder, (item, value) => item.DisplayOrder = value);
    }

    protected static string RemoveSkillLabel(SaveSkillRequest skill)
    {
        var name = skill.Name.Trim();
        return name.Length == 0 ? "Remove skill" : $"Remove {name}";
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
            Categories = DisplayOrderRules.Renumber(Categories, item => item.DisplayOrder, (item, order) => item with { DisplayOrder = order });
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
