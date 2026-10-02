using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Shared;

public partial class SkillIconField : ComponentBase, IDisposable
{
    [CascadingParameter] private EditContext? EditContext { get; set; }

    [Parameter] public string Value { get; set; } = SkillIcons.DefaultName;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter, EditorRequired] public Expression<Func<string>> ValueExpression { get; set; } = default!;

    private EditContext? _editContext;
    private FieldValidation? _validation;

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

    private string? Error()
    {
        return SkillIcons.Contains(Value) ? null : "Choose a supported icon.";
    }
}
