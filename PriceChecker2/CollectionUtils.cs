using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceChecker2;

public static class CollectionUtils
{
    public static void SetOrAdd<Tkey, TValue>(this Dictionary<Tkey, TValue> dict, Tkey key, TValue value)
    {
        if (!dict.TryAdd(key, value)) 
            dict[key] = value;
    }

    public static bool TrySet<Tkey, TValue>(this Dictionary<Tkey, TValue> dict, Tkey key, TValue value)
    {
        bool contains = dict.ContainsKey(key);
        if (contains) dict[key] = value;
        return contains;
    }
}
