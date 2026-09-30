using Microsoft.AspNetCore.Components;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Components.Shared;

public partial class TechnologyCardComponent : ComponentBase
{
    private static readonly Dictionary<string, string> Icons = new()
    {
        ["api"] = "{}",
        ["component"] = "<>",
        ["container"] = "[]",
        ["database"] = "DB",
        ["device"] = "UI",
        ["layers"] = "//",
        ["layout"] = "#",
        ["pipeline"] = "=>",
        ["query"] = "Q",
        ["signal"] = "~",
        ["storage"] = "SQL"
    };

    [Parameter] public required SkillDto Skill { get; set; }

    protected string AccentStyle => $"--skill-accent: {Skill.AccentColor};";

    protected string IconGlyph => Icons.TryGetValue(Skill.IconName, out var icon) ? icon : "*";
}
