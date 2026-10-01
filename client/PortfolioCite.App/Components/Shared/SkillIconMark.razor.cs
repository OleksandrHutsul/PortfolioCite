using Microsoft.AspNetCore.Components;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Shared;

public partial class SkillIconMark
{
    [Parameter] public SkillIcon? Icon { get; set; }
    [Parameter] public string? Name { get; set; }
    
    private static readonly string[] UnknownPaths =
    [
        "M12 5.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13z",
        "M12 8.5v5",
        "M12 16.2h.01"
    ];

    private IReadOnlyList<string> Paths => (Icon ?? SkillIcons.Find(Name))?.Paths ?? UnknownPaths;
}
