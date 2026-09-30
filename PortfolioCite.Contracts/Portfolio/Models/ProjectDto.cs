namespace PortfolioCite.Contracts.Portfolio.Models;

public record ProjectDto(int Id, string Name, string ShortDescription, string Description, string? GitHubUrl, string? LiveUrl, string? ImageUrl, int DisplayOrder,
    bool IsFeatured, bool IsPublished, IReadOnlyList<string> Technologies, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
