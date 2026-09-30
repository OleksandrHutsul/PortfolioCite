namespace PortfolioCite.Contracts.Portfolio.Models;

public record LanguageProficiency(string Code, string Description)
{
    public string AdminLabel => Code == Description ? Description : $"{Code} — {Description}";

    public string PublicLabel => Code == Description ? Description : $"{Code} · {Description}";
}