using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public static class AsyncUtils
    {
        public static async Task<IEnumerable<TResult>> AsyncSelect<TInput, TResult>(this IEnumerable<TInput> items,
            Func<TInput, Task<TResult>> selector)
        {
            var results = new List<TResult>();

            foreach (var item in items)
            {
                results.Add(await selector(item));
            }

            return results;
        }
    }
}