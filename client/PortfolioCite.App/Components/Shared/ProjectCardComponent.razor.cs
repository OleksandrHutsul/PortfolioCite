using Microsoft.AspNetCore.Components;
using PortfolioCite.Contracts.Portfolio;

namespace PortfolioCite.App.Components.Shared;

public partial class ProjectCardComponent : ComponentBase
{
    [Parameter] public required ProjectDto Project { get; set; }
}
