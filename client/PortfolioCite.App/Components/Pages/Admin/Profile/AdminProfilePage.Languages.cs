using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage
{
    protected void AddLanguage()
    {
        if (!CanAddLanguage) return;

        Request.Languages.Add(new SaveProfileLanguageRequest
        {
            DisplayOrder = DisplayOrderRules.Next(Request.Languages.Count)
        });

        DisplayOrderRules.Normalize(Request.Languages, language => language.DisplayOrder, (language, order) => language.DisplayOrder = order);
    }

    protected void ChangeLanguageOrder(SaveProfileLanguageRequest language, int order)
    {
        DisplayOrderRules.TryMove(Request.Languages, language, order, item => item.DisplayOrder, (item, value) => item.DisplayOrder = value);
    }

    protected void RemoveLanguage(SaveProfileLanguageRequest language)
    {
        if (IsSaving) return;

        DisplayOrderRules.Remove(Request.Languages, language, item => item.DisplayOrder, (item, value) => item.DisplayOrder = value);
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
