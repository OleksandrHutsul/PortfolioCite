namespace PortfolioCite.Api.Configuration;

public class InitialAdminOptions
{
    public const string SectionName = "InitialAdmin";

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
