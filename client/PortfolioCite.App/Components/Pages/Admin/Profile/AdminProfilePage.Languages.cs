using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage
{
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
    }

    protected static string RemoveLanguageLabel(SaveProfileLanguageRequest language)
    {
        var name = language.Name.Trim();
        return name.Length == 0 ? "Remove language" : $"Remove {name}";
    }

    private void UseRequest(SaveProfileRequest request)
    {
        Request = request;
        ProfileEditContext = new EditContext(Request);
    }
}
