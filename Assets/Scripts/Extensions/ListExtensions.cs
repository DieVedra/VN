
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static Dictionary<TKey, T> ToDictionaryDistinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : notnull
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        
        var dictionary = new Dictionary<TKey, T>();
        
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (key != null)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, item);
                }
            }
        }
        
        return dictionary;
    }

    public static Dictionary<TKey, T> ToDictionaryDistinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) where TKey : notnull
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        var dictionary = new Dictionary<TKey, T>(comparer);
        
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (key != null)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, item);
                }
            }
        }
        
        return dictionary;
    }
}