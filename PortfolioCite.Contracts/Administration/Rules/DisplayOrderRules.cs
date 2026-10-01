namespace PortfolioCite.Contracts.Administration.Rules;

public static class DisplayOrderRules
{
    public const int Minimum = 0;
    public const string DuplicateMessage = "Display order values must be unique.";

    public static int ItemCount(int existingCount, bool isCreating)
    {
        return isCreating ? existingCount + 1 : Math.Max(existingCount, 1);
    }

    public static int Maximum(int itemCount)
    {
        return itemCount < 1 ? Minimum : itemCount - 1;
    }

    public static int Next(int existingCount)
    {
        return existingCount;
    }

    public static string? Error(int displayOrder, int itemCount)
    {
        var maximum = Maximum(itemCount);

        if (displayOrder >= Minimum && displayOrder <= maximum)
            return null;

        return maximum == Minimum ? "Display order must be 0." : $"Display order must be between 0 and {maximum}.";
    }

    public static string? SequenceError(IReadOnlyList<int> orders)
    {
        for (var index = 0; index < orders.Count; index++)
        {
            var error = Error(orders[index], orders.Count);

            if (error is not null)
                return error;
        }

        return orders.Distinct().Count() == orders.Count ? null : DuplicateMessage;
    }

    public static bool TryMove<T>(IList<T> items, T item, int newOrder, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        if (Error(newOrder, items.Count) is not null)
        {
            setOrder(item, newOrder);
            return false;
        }

        Move(items, item, newOrder, getOrder, setOrder);
        return true;
    }

    public static void Move<T>(IList<T> items, T item, int newOrder, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        var ordered = Sort(items, getOrder);
        var oldIndex = IndexOf(ordered, item);

        if (oldIndex < 0) return;

        var targetIndex = Math.Clamp(newOrder, Minimum, ordered.Count - 1);

        if (oldIndex == targetIndex) return;

        ordered.RemoveAt(oldIndex);
        ordered.Insert(targetIndex, item);
        Assign(ordered, setOrder);
    }

    public static void Insert<T>(IList<T> items, T item, int newOrder, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        if (!items.Contains(item))
            items.Add(item);

        Move(items, item, newOrder, getOrder, setOrder);
    }

    public static void Remove<T>(IList<T> items, T item, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        if (!items.Remove(item)) return;

        Normalize(items, getOrder, setOrder);
    }

    public static void Normalize<T>(IList<T> items, Func<T, int> getOrder, Action<T, int> setOrder)
    {
        Assign(Sort(items, getOrder), setOrder);
    }

    public static List<T> Renumber<T>(IReadOnlyList<T> items, Func<T, int> getOrder, Func<T, int, T> withOrder)
    {
        return Sort(items, getOrder)
            .Select((item, index) => withOrder(item, index))
            .ToList();
    }

    private static List<T> Sort<T>(IEnumerable<T> items, Func<T, int> getOrder)
    {
        return items.OrderBy(getOrder).ToList();
    }

    private static void Assign<T>(IReadOnlyList<T> items, Action<T, int> setOrder)
    {
        for (var index = 0; index < items.Count; index++)
            setOrder(items[index], index);
    }

    private static int IndexOf<T>(IReadOnlyList<T> items, T item)
    {
        for (var index = 0; index < items.Count; index++)
        {
            if (ReferenceEquals(items[index], item))
                return index;
        }

        return -1;
    }
}
