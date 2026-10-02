using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioCite.App.Components.Shared;

public partial class ProficiencyField : ComponentBase
{
    [CascadingParameter] private EditContext? EditContext { get; set; }

    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter, EditorRequired] public Expression<Func<string>> ValueExpression { get; set; } = default!;
    [Parameter] public string Label { get; set; } = "Proficiency";

    private bool IsInvalid => EditContext?.GetValidationMessages(FieldIdentifier.Create(ValueExpression)).Any() == true;

    private async Task OnChanged(string value)
    {
        await ValueChanged.InvokeAsync(value);
        EditContext?.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
    }
}
