using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class LoadingSkeletonComponent : ComponentBase
{
    [Parameter] public string Variant { get; set; } = "cards";
    [Parameter] public int Count { get; set; } = 3;
}
