﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Mixins;

public static class EnumerableMixin
{
    public static string AsCommaSeparatedList<T>(this IEnumerable<T> enumerable)
    {
        return string.Join(",", enumerable.Select(node => node.ToString()));
    }

    public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source, int n)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Argument n should be non-negative.");
        }

        return InternalDropLast(source, n);
    }

    private static IEnumerable<T> InternalDropLast<T>(IEnumerable<T> source, int n)
    {
        var buffer = new Queue<T>(n + 1);

        foreach (var x in source)
        {
            buffer.Enqueue(x);

            if (buffer.Count == n + 1)
            {
                yield return buffer.Dequeue();
            }
        }
    }

    /// <summary>
    ///     Returns a sequence with distinct adjacent elements from the input sequence based on the specified comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <c>source</c>.</typeparam>
    /// <param name="source">The sequence to return distinct elements from.</param>
    /// <param name="comparer">A comparer used to test equality between elements (can be null).</param>
    /// <returns>A sequence that contains only distinct adjacent elements</returns>
    public static IEnumerable<TSource> DistinctUntilChanged<TSource>(
        this IEnumerable<TSource> source,
        IEqualityComparer<TSource> comparer = null)
    {
        return source.DistinctUntilChangedByImpl(Identity, comparer);
    }

    /// <summary>
    ///     Returns a sequence with distinct adjacent elements from the input sequence based on the specified key and key
    ///     comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <c>source</c>.</typeparam>
    /// <typeparam name="TKey">The type of the key used for testing equality between elements.</typeparam>
    /// <param name="source">The sequence to return distinct elements from.</param>
    /// <param name="keySelector">A delegate that returns the key used to test equality between elements.</param>
    /// <param name="keyComparer">A comparer used to test equality between keys (can be null).</param>
    /// <returns>A sequence whose elements have distinct adjacent values for the specified key.</returns>
    public static IEnumerable<TSource> DistinctUntilChangedBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return source.DistinctUntilChangedByImpl(keySelector, keyComparer);
    }

    private static IEnumerable<TSource> DistinctUntilChangedByImpl<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer)
    {
        keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        using (var en = source.GetEnumerator())
        {
            if (!en.MoveNext())
            {
                yield break;
            }

            yield return en.Current;
            var prevKey = keySelector(en.Current);

            while (en.MoveNext())
            {
                var key = keySelector(en.Current);
                if (!keyComparer.Equals(prevKey, key))
                {
                    yield return en.Current;
                    prevKey = key;
                }
            }
        }
    }

    private static T Identity<T>(T arg)
    {
        return arg;
    }

    public static IEnumerable<T> GetCommon<T>(this IList<IEnumerable<T>> lists)
    {
        return lists.GetCommon(EqualityComparer<T>.Default);
    }

    public static IEnumerable<T> GetCommon<T>(this IList<IEnumerable<T>> lists, IEqualityComparer<T> comparer)
    {
        return lists
            .Skip(1)
            .Aggregate(
                new HashSet<T>(lists.First(), comparer),
                (h, e) =>
                {
                    h.IntersectWith(e);
                    return h;
                }
            );
    }

    public static IEnumerable<T> Do<T>(this IEnumerable<T> self, Action<T> action)
    {
        return self.Select(x =>
        {
            action(x);
            return x;
        });
    }

    /// <summary>
    ///     Grows a sequence. From a list of items, like {1, 2, 3}, it generates {{1}, {1, 2}, {1, 2, 3}}
    /// </summary>
    /// <typeparam name="T">The type of the sequence</typeparam>
    /// <param name="sequence">Sequence to grow</param>
    /// <returns>Grow sequences</returns>
    public static IEnumerable<IEnumerable<T>> Grow<T>(this IEnumerable<T> sequence)
    {
        return sequence.Select((x, i) => sequence.Take(i + 1));
    }

    /// <summary>
    ///     Flattens a tree-like structure
    /// </summary>
    /// <typeparam name="T">Type of the nodes</typeparam>
    /// <param name="nodes">Root nodes</param>
    /// <param name="getChildren">The children selector</param>
    /// <returns></returns>
    public static IEnumerable<T> FlattenTree<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> getChildren)
    {
        return nodes.SelectMany(node => FlattenTree(node, getChildren));
    }

    /// <summary>
    ///     Flattens a tree-like structure
    /// </summary>
    /// <typeparam name="T">Type of the nodes</typeparam>
    /// <param name="node">Root node</param>
    /// <param name="getChildren">The children selector</param>
    /// <returns></returns>
    public static IEnumerable<T> FlattenTree<T>(this T node, Func<T, IEnumerable<T>> getChildren)
    {
        return new[] { node }.Concat(getChildren(node).SelectMany(x => FlattenTree(x, getChildren)));
    }

    public static IEnumerable<bool> Not(this IEnumerable<bool> self)
    {
        return self.Select(b => !b);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return enumerable.SelectMany(x => x);
    }
}