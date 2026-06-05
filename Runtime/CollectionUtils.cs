using System;
using System.Collections.Generic;

namespace ErmineGames.Utils
{
    public static class CollectionUtils
    {
        public static void RemoveAtFast<T>(this List<T> list, int index)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            list[index] = list[^1]; 
            list.RemoveAt(list.Count - 1);       
        }
        
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, 
            TKey key, 
            Func<TValue> construct)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            
            if (construct == null)
            {
                throw new ArgumentNullException(nameof(construct));
            }

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = construct();
                dictionary.Add(key, value);
            }
            
            return value;
        }
        
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, 
            TKey key) 
            where TValue : new()
        {
            return dictionary.GetOrCreate(key, static () => new TValue());
        }
    }
}
