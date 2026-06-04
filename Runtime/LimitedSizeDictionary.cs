using System;
using System.Collections;
using System.Collections.Generic;

namespace ErmineGames.Utils
{
    public class LimitedSizeDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly LinkedList<KeyValuePair<TKey, TValue>> linkedList;

        public IEnumerable<TKey> Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;
        
        private readonly int capacity;

        public int Count => dictionary.Count;
        public int Capacity => capacity;

        public LimitedSizeDictionary(int capacity, IEqualityComparer<TKey> comparer = null)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            }

            this.capacity = capacity;
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dictionary.TryGetValue(key, out _))
            {
                throw new ArgumentException($"An item with the same key '{key}' has already been added.", nameof(key));
            }

            if (dictionary.Count >= capacity)
            {
                var oldestNode = linkedList.First;

                if (oldestNode != null)
                {
                    dictionary.Remove(oldestNode.Value.Key);
                    linkedList.RemoveFirst();
                }
            }

            linkedList.AddLast(new KeyValuePair<TKey, TValue>(key, value));
            dictionary.Add(key, value);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dictionary.TryGetValue(key, out var oldValue))
            {
                linkedList.Remove(new KeyValuePair<TKey, TValue>(key, oldValue)); 
                linkedList.AddLast(new KeyValuePair<TKey, TValue>(key, value));
                dictionary[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }
        
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                
                throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
            }
            set => AddOrUpdate(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!dictionary.TryGetValue(key, out var value))
            {
                return false;
            }

            linkedList.Remove(new KeyValuePair<TKey, TValue>(key, value));
            dictionary.Remove(key);
            return true;
        }

        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
