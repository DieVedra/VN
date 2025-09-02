using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Dictionary<TKey, TValue> collection,
        bool overwrite = true)
    {
        if (collection == null) return;
        foreach (var pair in collection)
        {
            if (overwrite || !source.ContainsKey(pair.Key))
            {
                source[pair.Key] = pair.Value;
            }
        }
    }
}