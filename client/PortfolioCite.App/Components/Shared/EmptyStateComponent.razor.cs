using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class EmptyStateComponent : ComponentBase
{
    [Parameter] public string Title { get; set; } = "Nothing here yet";
    [Parameter] public string? Description { get; set; }
}
