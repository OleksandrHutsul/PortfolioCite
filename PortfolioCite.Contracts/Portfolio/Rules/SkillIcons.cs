using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Contracts.Portfolio.Rules;

public static class SkillIcons
{
    public const string DefaultName = "component";

    public static readonly IReadOnlyList<SkillIcon> All =
    [
        new("api", "API",
        [
            "M10 5c-2 0-3 1.2-3 3v2.2c0 .7-.5 1.2-1.5 1.8 1 .6 1.5 1.1 1.5 1.8V16c0 1.8 1 3 3 3",
            "M14 5c2 0 3 1.2 3 3v2.2c0 .7.5 1.2 1.5 1.8-1 .6-1.5 1.1-1.5 1.8V16c0 1.8-1 3-3 3"
        ]),
        new("component", "Component",
        [
            "M9.5 7 5 12l4.5 5",
            "M14.5 7 19 12l-4.5 5"
        ]),
        new("container", "Container",
        [
            "M7 4.5h10a2.5 2.5 0 0 1 2.5 2.5v10a2.5 2.5 0 0 1-2.5 2.5H7A2.5 2.5 0 0 1 4.5 17V7A2.5 2.5 0 0 1 7 4.5z"
        ]),
        new("database", "Database",
        [
            "M5 7a7 2.6 0 1 0 14 0 7 2.6 0 1 0-14 0",
            "M5 7v10c0 1.5 3.1 2.6 7 2.6s7-1.1 7-2.6V7",
            "M5 12c0 1.5 3.1 2.6 7 2.6s7-1.1 7-2.6"
        ]),
        new("device", "Device",
        [
            "M6.5 4.5h11a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2h-11a2 2 0 0 1-2-2v-7a2 2 0 0 1 2-2z",
            "M8 20.5h8",
            "M12 15.5v5"
        ]),
        new("layers", "Layers",
        [
            "m12 3.5 8 3.8-8 3.8-8-3.8 8-3.8z",
            "m4 12 8 3.8 8-3.8",
            "m4 16.2 8 3.8 8-3.8"
        ]),
        new("layout", "Layout",
        [
            "M6.5 4.5h11a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2h-11a2 2 0 0 1-2-2v-11a2 2 0 0 1 2-2z",
            "M4.5 9.5h15",
            "M12 9.5v10"
        ]),
        new("pipeline", "Pipeline",
        [
            "M4 12h12.5",
            "m12 6 6 6-6 6"
        ]),
        new("query", "Query",
        [
            "M10.5 5a5.5 5.5 0 1 0 0 11 5.5 5.5 0 0 0 0-11z",
            "m15 15 4.5 4.5"
        ]),
        new("signal", "Signal",
        [
            "M3 12h3.2l2.2-6.5L12 18.5l2.4-6.5H21"
        ]),
        new("storage", "Storage",
        [
            "M4.5 3.8h15a1.2 1.2 0 0 1 1.2 1.2v2.4a1.2 1.2 0 0 1-1.2 1.2h-15a1.2 1.2 0 0 1-1.2-1.2V5a1.2 1.2 0 0 1 1.2-1.2z",
            "M4.5 10.2h15a1.2 1.2 0 0 1 1.2 1.2v2.4a1.2 1.2 0 0 1-1.2 1.2h-15a1.2 1.2 0 0 1-1.2-1.2v-2.4a1.2 1.2 0 0 1 1.2-1.2z",
            "M4.5 16.6h15a1.2 1.2 0 0 1 1.2 1.2v2.4a1.2 1.2 0 0 1-1.2 1.2h-15a1.2 1.2 0 0 1-1.2-1.2v-2.4a1.2 1.2 0 0 1 1.2-1.2z"
        ])
    ];

    public static bool Contains(string? name) => Canonical(name) is not null;

    public static string? Canonical(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return All.FirstOrDefault(icon => icon.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))?.Name;
    }

    public static SkillIcon? Find(string? name)
    {
        var canonical = Canonical(name);

        return canonical is null ? null : All.First(icon => icon.Name == canonical);
    }
}
