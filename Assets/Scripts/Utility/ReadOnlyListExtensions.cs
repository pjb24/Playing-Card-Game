using System;
using System.Collections.Generic;

public static class ReadOnlyListExtensions
{
    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < list.Count; i++)
        {
            if (comparer.Equals(list[i], item))
            {
                return i;
            }
        }
        return -1;
    }
}
