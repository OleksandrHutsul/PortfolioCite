using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Administration;

public class SaveSkillRequest
{
    [Required, StringLength(80)] 
    public string Name { get; set; } = string.Empty;
    
    [Required, StringLength(600)] 
    public string Description { get; set; } = string.Empty;
    
    [Required, StringLength(40)] 
    public string Badge { get; set; } = string.Empty;
    
    [Required, StringLength(40)] 
    public string IconName { get; set; } = "code";
    
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] 
    public string AccentColor { get; set; } = "#58a6ff";
    
    [Range(0, int.MaxValue)] 
    public int DisplayOrder { get; set; }
}
