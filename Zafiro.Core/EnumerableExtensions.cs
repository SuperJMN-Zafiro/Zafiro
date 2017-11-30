namespace Zafiro.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
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
            Queue<T> buffer = new Queue<T>(n + 1);

            foreach (T x in source)
            {
                buffer.Enqueue(x);

                if (buffer.Count == n + 1)
                    yield return buffer.Dequeue();
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
            return source.DistinctUntilChangedByImpl<TSource, TSource>(Identity, comparer);
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
    }
}