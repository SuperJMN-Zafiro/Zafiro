using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using Optional.Linq;

namespace Zafiro.Core.Patterns
{
    public static class EitherMixin
    {
        public static Either<IEnumerable<TLeft>, TRight> Combine<TLeft, TRight>(this Either<IEnumerable<TLeft>, TRight> ea, Either<IEnumerable<TLeft>, TRight> eb,
            Func<TRight, TRight, Either<IEnumerable<TLeft>, TRight>> map) =>
            ea
                .MapError(el1 => eb
                    .MapError(el1.Concat)
                    .MapSuccess(_ => Either.Error<IEnumerable<TLeft>, TRight>(el1)))
                .MapSuccess(x => eb
                    .MapSuccess(y => map(x, y))
                    .MapError(el => el));

        public static Either<TLeft, TRight> Combine<TLeft, TRight>(
            this Either<TLeft, TRight> ea,
            Either<TLeft, TRight> eb,
            Func<TRight, TRight, Either<TLeft, TRight>> mapSuccess, Func<TLeft, TLeft, TLeft> combineError) => ea
            .MapError(el1 => eb
                .MapError(el2 => combineError(el1, el2))
                .MapSuccess(_ => Either.Error<TLeft, TRight>(el1)))
            .MapSuccess(x => eb
                .MapSuccess(y => mapSuccess(x, y))
                .MapError(el => el));

        public static Either<TLeft, TRight> Map<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers, Func<IEnumerable<TRight>, Either<TLeft, TRight>> mapSuccess, Func<IEnumerable<TLeft>, Either<TLeft, TRight>> mapError)
        {
            return Map<TLeft, TRight, TRight>(eithers, mapSuccess, mapError);
        }

        public static Either<TLeft, TNewRight> Map<TLeft, TRight, TNewRight>(this IEnumerable<Either<TLeft, TRight>> eithers, Func<IEnumerable<TRight>, Either<TLeft, TNewRight>> mapSuccess, Func<IEnumerable<TLeft>, Either<TLeft, TNewRight>> mapError)
        {
            var partition = eithers.Partition(either => either.IsRight);

            var rightValues = partition.True.SelectMany(either => either.RightValue.ToEnumerable());
            var leftValues = partition.True.SelectMany(either => either.LeftValue.ToEnumerable()).ToList();

            return leftValues.Any() ? mapError(leftValues) : mapSuccess(rightValues);
        }
    }
}