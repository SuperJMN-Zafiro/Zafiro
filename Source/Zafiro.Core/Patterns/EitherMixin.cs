using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

namespace Zafiro.Core.Patterns
{
    public static class EitherMixin
    {
        public static Either<IEnumerable<TError>, T> Combine<T, TError>(this Either<IEnumerable<TError>, T> ea, Either<IEnumerable<TError>, T> eb,
            Func<T, T, Either<IEnumerable<TError>, T>> map)
        {
            return ea
                .MapError(el1 => eb
                    .MapError(el1.Concat)
                    .MapSuccess(_ => Either.Error<IEnumerable<TError>, T>(el1)))
                .MapSuccess(x => eb
                    .MapSuccess(y => map(x, y))
                    .MapError(el => el));
        }

        public static Either<TError, TValue> Map<TError, TValue>(this IEnumerable<Either<TError, TValue>> eithers, Func<IEnumerable<TValue>, Either<TError, TValue>> mapSuccess, Func<IEnumerable<TError>, Either<TError, TValue>> mapError)
        {
            return Map<TError, TValue, TValue>(eithers, mapSuccess, mapError);
        }

        public static Either<TError, TDest> Map<TError, TSource, TDest>(this IEnumerable<Either<TError, TSource>> eithers, Func<IEnumerable<TSource>, Either<TError, TDest>> mapSuccess, Func<IEnumerable<TError>, Either<TError, TDest>> mapError)
        {
            var partition = eithers.Partition(either => either.IsRight);

            var rightValues = partition.True.SelectMany(either => either.RightValue.ToEnumerable());
            var leftValues = partition.True.SelectMany(either => either.LeftValue.ToEnumerable()).ToList();

            return leftValues.Any() ? mapError(leftValues) : mapSuccess(rightValues);
        }
    }
}