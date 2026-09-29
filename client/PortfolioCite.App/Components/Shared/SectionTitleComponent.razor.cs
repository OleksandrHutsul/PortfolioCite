using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class SectionTitleComponent : ComponentBase
{
    [Parameter] public required string Eyebrow { get; set; }
    [Parameter] public required string Title { get; set; }
}
