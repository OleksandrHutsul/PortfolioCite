namespace PortfolioCite.Contracts.Projects.Rules;

public static class ProjectVisibilityRules
{
    public const int MaxVisible = 3;

    public const string LimitMessage = "Only 3 projects can be visible on the home page. Hide another project first.";
}
