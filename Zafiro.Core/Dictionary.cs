namespace Zafiro.Core
{
    using System;
    using System.Collections.Generic;

    public static class Dictionary
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
    }
}