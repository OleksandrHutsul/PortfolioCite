using System.ComponentModel.DataAnnotations;

namespace PortfolioCite.Contracts.Portfolio.Rules;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SkillIconNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name) || SkillIcons.Contains(name))
            return ValidationResult.Success;

        return new ValidationResult("Choose a supported icon.", [validationContext.MemberName!]);
    }
}
