using Microsoft.AspNetCore.Components;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Components.Shared;

public partial class TechnologyCardComponent : ComponentBase
{
    [Parameter] public required SkillDto Skill { get; set; }

    protected string AccentStyle => $"--skill-accent: {Skill.AccentColor};";
}
