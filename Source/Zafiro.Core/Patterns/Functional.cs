using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.Core.Patterns
{
    public static class Functional
    {
        public static Maybe<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return Maybe<TValue>.From(source.TryGetValue(key, out var val) ? val : default);
        }

        public static Maybe<TValue> TryGetValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source
                .TryGetFirst(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                .Map(pair => pair.Value);
        }

        public static Maybe<TValue> TryGetFirst<TValue>(this IEnumerable<TValue> source, Func<TValue, bool> selector)
        {
            var val = source.FirstOrDefault(selector);
            return Maybe<TValue>.From(val);
        }

        public static Maybe<TValue> TryGetFirst<TValue>(this IEnumerable<TValue> source)
        {
            var val = source.FirstOrDefault();
            return Maybe<TValue>.From(val);
        }
    }
}