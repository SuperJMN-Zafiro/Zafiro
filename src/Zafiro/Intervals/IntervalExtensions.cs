using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Intervals;

public static class IntervalExtensions
{
    public static IEnumerable<Interval<T>> Overlaps<T>(this IEnumerable<Interval<T>> intervals) where T : IComparable
    {
        var overlap = intervals
            .OrderBy(x => x.Start)
            .Buffer(Interval<T>.Intersection)
            .Where(x => !x.IsEmpty);

        return overlap;
    }

    public static bool IsFullyContainedIn<T>(this Interval<T> a, Interval<T> b) where T : IComparable
    {
        return Equals(b.Intersection(a), a);
    }

    public static bool ContainsFully<T>(this Interval<T> a, Interval<T> b) where T : IComparable
    {
        return Equals(b.Intersection(a), b);
    }

    public static IEnumerable<Interval<T>> Simplify<T>(this IEnumerable<Interval<T>> list) where T : IComparable
    {
        var sorted = list.OrderBy(x => x.Start).ToList();
        var initial = new List<Interval<T>> { sorted.First() };

        var seq = sorted
            .Aggregate(initial, (previous, candidate) =>
            {
                var last = previous.Last();
                if (candidate.Start.LessThanOrEqual(last.End))
                {
                    var start = last.Start.Min(candidate.Start);
                    var end = last.End.Max(candidate.End);

                    var toAdd = new Interval<T>(start, end);
                    return previous.Take(previous.Count - 1).Concat(new[] { toAdd }).ToList();
                }

                return previous.Concat(new[] { candidate }).ToList();
            });

        return seq;
    }

    public static IEnumerable<Interval<T>> Intersections<T>(this IEnumerable<Interval<T>> a,
        Interval<T> b) where T : IComparable
    {
        return a.Select(x => x.Intersection(b)).Where(interval => !interval.IsEmpty);
    }

    public static IEnumerable<Interval<T>> Intersections<T>(this IEnumerable<Interval<T>> a,
        IEnumerable<Interval<T>> b) where T : IComparable
    {
        return a.SelectMany(b.Intersections).Where(interval => !interval.IsEmpty);
    }
}