using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveSkillCategoryRequest
{
    [Required, StringLength(80)] 
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)] 
    public int DisplayOrder { get; set; }

    [Required, MaxLength(50)] 
    public List<SaveSkillRequest> Skills { get; set; } = [];
}
