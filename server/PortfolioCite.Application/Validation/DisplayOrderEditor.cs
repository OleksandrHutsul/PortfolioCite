using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.Application.Validation;

public static class DisplayOrderEditor
{
    public static void Insert<T>(IList<T> items, T item, int requestedOrder, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        RequireRange(requestedOrder, items.Count + 1);
        DisplayOrderRules.Insert(items, item, requestedOrder, getOrder, setOrder);
    }

    public static void Move<T>(IList<T> items, T item, int requestedOrder, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        if (getOrder(item) != requestedOrder)
            RequireRange(requestedOrder, items.Count);

        if (getOrder(item) == requestedOrder)
        {
            DisplayOrderRules.Normalize(items, getOrder, setOrder);
            return;
        }

        DisplayOrderRules.Move(items, item, requestedOrder, getOrder, setOrder);
    }

    public static void CloseGap<T>(IList<T> items, T removed, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        items.Remove(removed);
        DisplayOrderRules.Normalize(items, getOrder, setOrder);
    }

    public static void RequireSequence(IReadOnlyList<int> orders, Func<int, string> fieldAt)
    {
        var validator = new ContentValidator();
        validator.AddDisplayOrderSequence(orders, fieldAt);
        validator.ThrowIfInvalid();
    }

    private static void RequireRange(int displayOrder, int itemCount)
    {
        var validator = new ContentValidator();
        validator.AddDisplayOrder(displayOrder, itemCount);
        validator.ThrowIfInvalid();
    }
}
