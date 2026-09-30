using System.ComponentModel.DataAnnotations;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Contracts.Administration.Models;

public class SaveProfileRequest : IValidatableObject
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Role { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    public string Summary { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string CurrentFocus { get; set; } = string.Empty;

    public List<SaveProfileLanguageRequest> Languages { get; set; } = [];

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var error in LanguageFieldErrors(Languages))
        {
            var member = error.Index < 0
                ? nameof(Languages)
                : $"Languages[{error.Index}].{error.Field}";

            yield return new ValidationResult(error.Message, [member]);
        }
    }

    public static IEnumerable<ProfileLanguageFieldError> LanguageFieldErrors(IReadOnlyList<SaveProfileLanguageRequest>? languages)
    {
        languages ??= [];

        if (languages.Count > LanguageProficiencies.MaxCount)
        {
            yield return new ProfileLanguageFieldError(-1, string.Empty, $"Add no more than {LanguageProficiencies.MaxCount} languages.");
        }

        for (var index = 0; index < languages.Count; index++)
        {
            var language = languages[index];
            var name = language.Name?.Trim() ?? string.Empty;

            if (name.Length == 0)
                yield return new ProfileLanguageFieldError(index, nameof(SaveProfileLanguageRequest.Name), "Language name is required.");
            else if (name.Length > LanguageProficiencies.MaxNameLength)
                yield return new ProfileLanguageFieldError(index, nameof(SaveProfileLanguageRequest.Name), $"Language name must be {LanguageProficiencies.MaxNameLength} characters or fewer.");

            if (!LanguageProficiencies.Contains(language.Proficiency))
                yield return new ProfileLanguageFieldError(index, nameof(SaveProfileLanguageRequest.Proficiency), "Select a proficiency.");

            if (language.DisplayOrder < 0 || language.DisplayOrder > LanguageProficiencies.MaxDisplayOrder)
                yield return new ProfileLanguageFieldError(index, nameof(SaveProfileLanguageRequest.DisplayOrder), $"Display order must be between 0 and {LanguageProficiencies.MaxDisplayOrder}.");
        }
    }
}

public class SaveProfileLanguageRequest
{
    public string Name { get; set; } = string.Empty;

    public string Proficiency { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}

public readonly record struct ProfileLanguageFieldError(int Index, string Field, string Message);
