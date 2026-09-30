using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage
{
    private bool _showLanguageErrors;
    private bool _validatingLanguages;

    protected void AddLanguage()
    {
        if (!CanAddLanguage) return;

        var order = Request.Languages.Count == 0
            ? 1
            : Math.Min(LanguageProficiencies.MaxDisplayOrder, Request.Languages.Max(language => language.DisplayOrder) + 1);

        Request.Languages.Add(new SaveProfileLanguageRequest { DisplayOrder = order });
    }

    protected void RemoveLanguage(SaveProfileLanguageRequest language)
    {
        if (IsSaving) return;

        Request.Languages.Remove(language);

        if (_showLanguageErrors)
            ProfileEditContext.Validate();
    }

    protected static string RemoveLanguageLabel(SaveProfileLanguageRequest language)
    {
        var name = language.Name.Trim();
        return name.Length == 0 ? "Remove language" : $"Remove {name}";
    }

    private void UseRequest(SaveProfileRequest request)
    {
        DetachEditor();

        Request = request;
        ProfileEditContext = new EditContext(Request);
        _languageErrors = new ValidationMessageStore(ProfileEditContext);
        ProfileEditContext.OnValidationRequested += OnValidationRequested;
        ProfileEditContext.OnFieldChanged += OnLanguageFieldChanged;
        _showLanguageErrors = false;
    }

    private void DetachEditor()
    {
        if (ProfileEditContext is null) return;

        ProfileEditContext.OnValidationRequested -= OnValidationRequested;
        ProfileEditContext.OnFieldChanged -= OnLanguageFieldChanged;
    }

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        _showLanguageErrors = true;
        WriteLanguageErrors();
    }

    private void OnLanguageFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (_validatingLanguages || !_showLanguageErrors || e.FieldIdentifier.Model is not SaveProfileLanguageRequest)
            return;

        _validatingLanguages = true;

        try
        {
            ProfileEditContext.Validate();
        }
        finally
        {
            _validatingLanguages = false;
        }
    }

    private void WriteLanguageErrors()
    {
        _languageErrors.Clear();

        foreach (var error in SaveProfileRequest.LanguageFieldErrors(Request.Languages))
        {
            if (error.Index < 0 || error.Index >= Request.Languages.Count)
                continue;

            var language = Request.Languages[error.Index];
            _languageErrors.Add(new FieldIdentifier(language, error.Field), error.Message);
        }
    }
}
