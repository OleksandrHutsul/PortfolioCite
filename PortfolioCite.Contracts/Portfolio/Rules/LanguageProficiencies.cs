using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Contracts.Portfolio.Rules;

public static class LanguageProficiencies
{
    public const int MaxNameLength = 80;
    public const int MaxCount = 20;
    public const int MaxDisplayOrder = 9999;

    public static readonly IReadOnlyList<LanguageProficiency> All =
    [
        new("Native", "Native"),
        new("C2", "Proficient"),
        new("C1", "Advanced"),
        new("B2", "Upper-Intermediate"),
        new("B1", "Intermediate"),
        new("A2", "Elementary"),
        new("A1", "Basic")
    ];

    public static bool Contains(string? code) => All.Any(item => item.Code == code);

    public static string PublicLabel(string? code)
    {
        var match = All.FirstOrDefault(item => item.Code == code);

        return match is null ? code ?? string.Empty : match.PublicLabel;
    }
}
