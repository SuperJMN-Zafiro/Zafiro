using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;

namespace Zafiro.Patterns.Either
{
    public static class CombineExtensions
    {
        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, TResult>(this Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Func<T1, T2, Either<TLeft, TResult>> map, Func<TLeft, TLeft, TLeft> combineError)
        {
            var mapSuccess = a
                .MapLeft(el1 => b
                    .MapLeft(el2 => combineError(el1, el2))
                    .MapRight(_ => Either.Error<TLeft, T1>(el1)))
                .MapRight(x => b
                    .MapRight(y => map(x, y))
                    .MapLeft(el => el));

            return mapSuccess;
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, TResult>(this
                Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Func<T1, T2, T3, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Either.Success<TLeft, (T1, T2)>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, T4, TResult>(
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Either<TLeft, T4> d,
            Func<T1, T2, T3, T4, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Either.Success<TLeft, (T1, T2, T3)>((x, y, z)), combineError);
            return r.Combine(d, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, cur), combineError);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, T4, T5, TResult>(
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Either<TLeft, T4> d,
            Either<TLeft, T5> e,
            Func<T1, T2, T3, T4, T5, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Either.Success<TLeft, (T1, T2, T3, T4)>((x1, x2, x3, x4)),
                combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur),
                combineError);
        }

        public static Either<TLeft, TRight> Combine<TLeft, TRight>(
            this Either<TLeft, TRight> ea,
            Either<TLeft, TRight> eb,
            Func<TRight, TRight, Either<TLeft, TRight>> mapSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            return ea
                .MapLeft(el1 => eb
                    .MapLeft(el2 => combineError(el1, el2))
                    .MapRight(_ => Either.Error<TLeft, TRight>(el1)))
                .MapRight(x => eb
                    .MapRight(y => mapSuccess(x, y))
                    .MapLeft(el => el));
        }

        public static Either<TLeft, IEnumerable<TResult>> Combine<TLeft, TResult>(
            this IEnumerable<Either<TLeft, TResult>> eithers, Func<TLeft, TLeft, TLeft> combineError)
        {
            var errors = eithers.Partition(x => x.Right.HasValue);

            if (errors.False.Any())
            {
                var aggregate = errors.False
                    .SelectMany(e => e.Left.ToEnumerable())
                    .Aggregate(combineError);

                return Either.Error<TLeft, IEnumerable<TResult>>(aggregate);
            }

            var p = errors.True.SelectMany(e => e.Right.ToEnumerable());
            return Either.Success<TLeft, IEnumerable<TResult>>(p);
        }
    }
}