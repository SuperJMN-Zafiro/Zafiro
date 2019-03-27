using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Core
{
    public static class RangeMixin
    {
        public static bool Overlap<T>(this IEnumerable<T> self, Func<T, int> start, Func<T, int> end)
        {
            return self
                .OrderBy(start)
                .Select(x => new[] {start(x), end(x)})
                .SelectMany(x => x)
                .Aggregate((x, y) => y >= x ? y : int.MaxValue) == int.MaxValue;
        }

        public static bool Overlap<T>(this IEnumerable<T> self, Func<T, long> start, Func<T, long> end)
        {
            return self
                       .OrderBy(start)
                       .Select(x => new[] {start(x), end(x)})
                       .SelectMany(x => x)
                       .Aggregate((x, y) => y >= x ? y : long.MaxValue) == long.MaxValue;
        }

        public static bool Overlap<T>(this IEnumerable<T> self, Func<T, ulong> start, Func<T, ulong> end)
        {
            return self
                       .OrderBy(start)
                       .Select(x => new[] {start(x), end(x)})
                       .SelectMany(x => x)
                       .Aggregate((x, y) => y >= x ? y : ulong.MaxValue) == ulong.MaxValue;
        }

        public static bool Overlap<T>(this IEnumerable<T> self, Func<T, uint> start, Func<T, uint> end)
        {
            return self
                       .OrderBy(start)
                       .Select(x => new[] {start(x), end(x)})
                       .SelectMany(x => x)
                       .Aggregate((x, y) => y >= x ? y : uint.MaxValue) == uint.MaxValue;
        }
    }
}