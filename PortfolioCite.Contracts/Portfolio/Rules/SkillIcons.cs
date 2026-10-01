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
        ]),
        new("github", "GitHub",
        [
            "M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22"
        ]),
        new("linkedin", "LinkedIn",
        [
            "M16 8a6 6 0 0 1 6 6v7h-4v-7a2 2 0 0 0-4 0v7h-4v-7a6 6 0 0 1 6-6z",
            "M2 9h4v12H2z",
            "M4 2a2 2 0 1 0 0 4 2 2 0 0 0 0-4z"
        ]),
        new("email", "Email",
        [
            "M4.5 7.5h15a1.5 1.5 0 0 1 1.5 1.5v8a1.5 1.5 0 0 1-1.5 1.5h-15a1.5 1.5 0 0 1-1.5-1.5V9a1.5 1.5 0 0 1 1.5-1.5z",
            "m3 9.2 9 5.5 9-5.5"
        ]),
        new("website", "Website",
        [
            "M12 3.5a8.5 8.5 0 1 0 0 17 8.5 8.5 0 0 0 0-17z",
            "M3.5 12h17",
            "M12 3.5c2.2 2.2 3.4 5.1 3.4 8.5s-1.2 6.3-3.4 8.5c-2.2-2.2-3.4-5.1-3.4-8.5s1.2-6.3 3.4-8.5z"
        ]),
        new("link", "Link",
        [
            "M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71",
            "M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"
        ]),
        new("phone", "Phone",
        [
            "M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"
        ]),
        new("location", "Location",
        [
            "M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z",
            "M12 7a3 3 0 1 0 0 6 3 3 0 0 0 0-6z"
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
