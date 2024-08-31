using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro;

public static class SequenceExtensions
{
    public static IEnumerable<TResult> Buffer<T, TResult>(this IEnumerable<T> list, Func<T, T, TResult> selector)
    {
        return list.Zip(list.Skip(1), selector);
    }
}