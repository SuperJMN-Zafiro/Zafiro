using System;
using System.Collections.Generic;

namespace Zafiro;

public static class Calculate
{
    public static T Min<T>(params T[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException("values");
        }

        var comparer = Comparer<T>.Default;
        switch (values.Length)
        {
            case 0: throw new ArgumentException();
            case 1: return values[0];
            case 2:
                return comparer.Compare(values[0], values[1]) < 0
                    ? values[0]
                    : values[1];
            default:
                var best = values[0];
                for (var i = 1; i < values.Length; i++)
                    if (comparer.Compare(values[i], best) < 0)
                    {
                        best = values[i];
                    }

                return best;
        }
    }

    public static T Max<T>(params T[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException("values");
        }

        var comparer = Comparer<T>.Default;
        switch (values.Length)
        {
            case 0: throw new ArgumentException();
            case 1: return values[0];
            case 2:
                return comparer.Compare(values[0], values[1]) > 0
                    ? values[0]
                    : values[1];
            default:
                var best = values[0];
                for (var i = 1; i < values.Length; i++)
                    if (comparer.Compare(values[i], best) > 0)
                    {
                        best = values[i];
                    }

                return best;
        }
    }
}