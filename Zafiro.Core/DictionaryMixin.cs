using System.Threading.Tasks;

namespace Zafiro.Core
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryMixin
    {
        public static TValue GetCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> create)
        {
            if (dict.TryGetValue(key, out var v))
            {
                return v;
            }

            var value = create();
            dict.Add(key, value);
            return value;
        }

        public static async Task<TValue> GetCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<Task<TValue>> create)
        {
            if (dict.TryGetValue(key, out var v))
            {
                return v;
            }

            var value = await create();
            dict.Add(key, value);
            return value;
        }
    }
}