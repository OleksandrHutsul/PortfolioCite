using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.Application.Validation;

public class ContentValidator
{
    private readonly Dictionary<string, string[]> _errors = new(StringComparer.Ordinal);

    public void AddDisplayOrder(int displayOrder, int itemCount, string field = "DisplayOrder")
    {
        var error = DisplayOrderRules.Error(displayOrder, itemCount);

        if (error is not null)
            _errors[field] = [error];
    }

    public void AddDisplayOrderSequence(IReadOnlyList<int> orders, Func<int, string> fieldAt)
    {
        var count = orders.Count;

        for (var index = 0; index < count; index++)
        {
            var error = DisplayOrderRules.Error(orders[index], count);

            if (error is not null)
                _errors[fieldAt(index)] = [error];
        }

        if (orders.Distinct().Count() != count)
            _errors["DisplayOrder"] = [DisplayOrderRules.DuplicateMessage];
    }

    public void AddSkillIcon(string? iconName, string field)
    {
        if (SkillIcons.Contains(iconName)) return;

        _errors[field] = ["Choose a supported icon."];
    }

    public void ThrowIfInvalid()
    {
        if (_errors.Count > 0)
            throw new ContentValidationException(_errors);
    }
}
