using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Shared;

public partial class DisplayOrderField : ComponentBase, IDisposable
{
    [CascadingParameter] private EditContext? EditContext { get; set; }

    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChanged { get; set; }
    [Parameter, EditorRequired] public Expression<Func<int>> ValueExpression { get; set; } = default!;
    [Parameter] public int ItemCount { get; set; }
    [Parameter] public string Label { get; set; } = "Display order";
    [Parameter] public string? CssClass { get; set; }

    private EditContext? _editContext;
    private FieldValidation? _validation;
    private int Maximum => DisplayOrderRules.Maximum(ItemCount);

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
        return DisplayOrderRules.Error(Value, ItemCount);
    }
}
