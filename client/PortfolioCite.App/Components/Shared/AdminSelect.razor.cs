using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PortfolioCite.App.Components.Shared;

public partial class AdminSelect<TItem> : ComponentBase
{
    private readonly string _menuId = $"admin-select-{Guid.NewGuid():N}";
    private bool _suppressClick;

    [Parameter, EditorRequired] public IReadOnlyList<TItem> Items { get; set; } = [];
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter, EditorRequired] public Func<TItem, string> ItemValue { get; set; } = default!;
    [Parameter, EditorRequired] public Func<TItem, string> ItemLabel { get; set; } = default!;
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
    [Parameter] public RenderFragment? SelectedTemplate { get; set; }
    [Parameter] public RenderFragment? LeadingContent { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Select";
    [Parameter] public string EmptyLabel { get; set; } = "Select";
    [Parameter] public bool IncludeEmpty { get; set; }
    [Parameter] public string MenuLabel { get; set; } = "Options";
    [Parameter] public bool Invalid { get; set; }
    [Parameter] public bool Required { get; set; }

    private bool IsOpen { get; set; }
    private int Highlighted { get; set; }

    private int OptionCount => Items.Count + (IncludeEmpty ? 1 : 0);

    private string CurrentLabel
    {
        get
        {
            for (var index = 0; index < Items.Count; index++)
            {
                if (string.Equals(ItemValue(Items[index]), Value, StringComparison.Ordinal))
                    return ItemLabel(Items[index]);
            }

            return string.IsNullOrEmpty(Value) ? Placeholder : Value;
        }
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
                Highlighted = Math.Max(OptionCount - 1, 0);
                break;

            case "Escape" when IsOpen:
                Close();
                break;

            case "Enter" or " " when IsOpen:
                _suppressClick = true;
                await Choose(ValueAt(Highlighted));
                break;
        }
    }

    private void Open()
    {
        Highlighted = IndexOfValue();
        IsOpen = true;
    }

    private void Close()
    {
        IsOpen = false;
    }

    private void Move(int delta)
    {
        if (OptionCount == 0) return;

        Highlighted = Math.Clamp(Highlighted + delta, 0, OptionCount - 1);
    }

    private async Task Choose(string value)
    {
        IsOpen = false;

        if (!string.Equals(Value, value, StringComparison.Ordinal))
            await ValueChanged.InvokeAsync(value);
    }

    private int IndexOfValue()
    {
        if (IncludeEmpty && string.IsNullOrEmpty(Value))
            return 0;

        var offset = IncludeEmpty ? 1 : 0;

        for (var index = 0; index < Items.Count; index++)
        {
            if (string.Equals(ItemValue(Items[index]), Value, StringComparison.Ordinal))
                return index + offset;
        }

        return 0;
    }

    private string ValueAt(int index)
    {
        if (IncludeEmpty)
        {
            if (index <= 0)
                return string.Empty;

            index--;
        }

        if (Items.Count == 0)
            return string.Empty;

        return ItemValue(Items[Math.Clamp(index, 0, Items.Count - 1)]);
    }
}
