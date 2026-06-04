using System;
using System.Collections.Generic;
using System.Threading;

namespace ErmineGames.Utils
{
    public interface IWeightedItem<out T>
    {
        int Weight { get; }
        T Item { get; }
    }
    
    /// <summary>
    /// Thread-safe random utils.
    /// </summary>
    public static class RandomUtils
    {
        private static readonly ThreadLocal<Random> threadRandom = 
            new(() => new Random(Guid.NewGuid().GetHashCode()));
        
        private static Random GetRandom() => threadRandom.Value;
        
        public static T WeightRandom<T>(IReadOnlyList<IWeightedItem<T>> items)
        {
            if (items == null || items.Count == 0)
            {
                return default;
            }

            var summaryWeight = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Weight < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(items), "Weight cannot be negative.");
                }

                summaryWeight += items[i].Weight;
            }

            if (summaryWeight == 0)
            {
                return default;
            }

            var random = GetRandom().Next(0, summaryWeight);
            var accumulatedWeight = 0;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                
                if (random < accumulatedWeight + item.Weight)
                {
                    return item.Item;
                }

                accumulatedWeight += item.Weight;
            }

            return default;
        }
        
        public static T GetRandom<T>(this IList<T> list)
        {
            return list.Count == 0 ? default : list[GetRandom().Next(0, list.Count)];
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = GetRandom();
            int n = list.Count;

            for (int i = 0; i < n - 1; i++)
            {
                int j = random.Next(i, n);

                if (j != i)
                {
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }

        public static TEnum EnumRandom<TEnum>(bool skipDefaultValue = true) where TEnum : struct, Enum
        {
            var values = EnumCache<TEnum>.Values;
            
            if (values.Length == 0) 
                return default;
            
            var randomIndex = GetRandom().Next(skipDefaultValue ? 1 : 0, values.Length);
            return values[randomIndex];
        }
        
        private static class EnumCache<TEnum> where TEnum : struct, Enum
        {
            public static readonly TEnum[] Values = (TEnum[])Enum.GetValues(typeof(TEnum));
        }
    }
}
