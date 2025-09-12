using System.Collections.ObjectModel;

namespace PriceChecker2;

public static class CollectionUtils
{
    public static void SetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull
    {
        if (!dict.TryAdd(key, value)) 
            dict[key] = value;
    }

    public static bool TrySet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull
    {
        bool contains = dict.ContainsKey(key);
        if (contains) dict[key] = value;
        return contains;
    }

    public static void Swap<T>(this ObservableCollection<T> col, int a, int b)
    {
        if (a == b) return;
        if (a > b) (a, b) = (b, a);

        col.Move(a, b);
        col.Move(b - 1, a);
    }
    public static int BinarySearchIndexOf<T>(this IList<T> list, T obj, Comparison<T> comparison) 
        => list.BinarySearchIndexOf(obj, Comparer<T>.Create(comparison));
    public static int BinarySearchIndexOf<T>(this IList<T> list, T obj, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;

        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int mid = min + (max - min) / 2;
            int comp = comparer.Compare(obj, list[mid]);

            if (comp < 0) 
                max = mid - 1;
            else if (comp > 0) 
                min = mid + 1;
            else return mid;
        }
        return min;
    }

    public static void InsertOrderedBy<T, TKey>(this IList<T> list, T obj, Func<T, TKey> keySelector) where TKey : IComparable
        => list.InsertOrdered(obj, (a, b) => keySelector(a).CompareTo(keySelector(b)));
    public static void InsertOrdered<T>(this IList<T> list, T obj, Comparison<T> comparison)
        => list.InsertOrdered(obj, Comparer<T>.Create(comparison));
    public static void InsertOrdered<T>(this IList<T> list, T obj, IComparer<T> comparer)
    {
        int i = list.BinarySearchIndexOf(obj, comparer);
        list.Insert(i, obj);
    }
}
