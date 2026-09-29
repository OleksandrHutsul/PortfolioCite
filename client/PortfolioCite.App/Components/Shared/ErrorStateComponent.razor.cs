using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class ErrorStateComponent : ComponentBase
{
    [Parameter] public string Title { get; set; } = "Something went wrong";
    [Parameter] public string? Description { get; set; }
    [Parameter] public EventCallback OnRetry { get; set; }
    [Parameter] public bool IsRetrying { get; set; }

    protected Task Retry()
    {
        return OnRetry.InvokeAsync();
    }
}
