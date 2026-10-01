using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Shared;

public partial class SkillIconField : ComponentBase, IDisposable
{
    [CascadingParameter] private EditContext? EditContext { get; set; }

    [Parameter] public string Value { get; set; } = SkillIcons.DefaultName;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter, EditorRequired] public Expression<Func<string>> ValueExpression { get; set; } = default!;

    private readonly string _menuId = $"skill-icon-{Guid.NewGuid():N}";
    private EditContext? _editContext;
    private FieldValidation? _validation;
    private bool _suppressClick;
    private bool IsOpen { get; set; }
    private int Highlighted { get; set; }

    private SkillIcon? Selected => SkillIcons.Find(Value);

    private bool IsInvalid => EditContext?.GetValidationMessages(FieldIdentifier.Create(ValueExpression)).Any() == true;

    private string SelectedLabel => Selected?.Label ?? (string.IsNullOrWhiteSpace(Value) ? "Select an icon" : $"{Value.Trim()} (unsupported)");

    protected override void OnParametersSet()
    {
        if (EditContext != _editContext || _validation is null)
        {
            _validation?.Dispose();
            _editContext = EditContext;
            _validation = EditContext is null
                ? null
                : new FieldValidation(EditContext, FieldIdentifier.Create(ValueExpression), Error);
            return;
        }

        _validation.Refresh();
    }

    public void Dispose()
    {
        _validation?.Dispose();
    }

    private void OnTriggerClick()
    {
        if (_suppressClick)
        {
            _suppressClick = false;
            return;
        }

        if (IsOpen)
            Close();
        else
            Open();
    }

    private async Task OnTriggerKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                if (!IsOpen)
                    Open();
                else
                    Move(1);
                break;
            
            case "ArrowUp":
                if (!IsOpen)
                    Open();
                else
                    Move(-1);
                break;
            
            case "Home" when IsOpen:
                Highlighted = 0;
                break;
            
            case "End" when IsOpen:
                Highlighted = SkillIcons.All.Count - 1;
                break;
            
            case "Escape" when IsOpen:
                Close();
                break;
            
            case "Enter" or " " when IsOpen:
                _suppressClick = true;
                await Choose(SkillIcons.All[Highlighted]);
                break;
        }
    }

    private void Open()
    {
        Highlighted = 0;
        var canonical = SkillIcons.Canonical(Value);

        if (canonical is not null)
        {
            for (var index = 0; index < SkillIcons.All.Count; index++)
            {
                if (SkillIcons.All[index].Name == canonical)
                {
                    Highlighted = index;
                    break;
                }
            }
        }

        IsOpen = true;
    }

    private void Close()
    {
        IsOpen = false;
    }

    private void Move(int delta)
    {
        var last = SkillIcons.All.Count - 1;
        Highlighted = Math.Clamp(Highlighted + delta, 0, last);
    }

    private async Task Choose(SkillIcon icon)
    {
        IsOpen = false;

        if (!string.Equals(Value, icon.Name, StringComparison.Ordinal))
            await ValueChanged.InvokeAsync(icon.Name);
    }

    private string? Error()
    {
        return SkillIcons.Contains(Value) ? null : "Choose a supported icon.";
    }
}
