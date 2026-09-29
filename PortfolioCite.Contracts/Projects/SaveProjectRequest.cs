using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Projects;

public class SaveProjectRequest
{
    [Required, StringLength(120)] 
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(300)] 
    public string ShortDescription { get; set; } = string.Empty;

    [Required, StringLength(4000)] 
    public string Description { get; set; } = string.Empty;
    
    [Url, StringLength(500)]
    public string? GitHubUrl { get; set; }
    
    [Url, StringLength(500)]
    public string? LiveUrl { get; set; }
    
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
    
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    
    [MaxLength(30)] 
    public IReadOnlyList<string> Technologies { get; set; } = [];
}
