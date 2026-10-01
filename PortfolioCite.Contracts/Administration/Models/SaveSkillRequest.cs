using System.ComponentModel.DataAnnotations;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveSkillRequest
{
    [Required, StringLength(80)] 
    public string Name { get; set; } = string.Empty;
    
    [Required, StringLength(600)] 
    public string Description { get; set; } = string.Empty;
    
    [Required, StringLength(40)] 
    public string Badge { get; set; } = string.Empty;
    
    [Required, StringLength(40), SkillIconName]
    public string IconName { get; set; } = SkillIcons.DefaultName;
    
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] 
    public string AccentColor { get; set; } = "#58a6ff";
    
    public int DisplayOrder { get; set; }
}
