using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using Optional;

namespace Zafiro.Core.Patterns
{
    public abstract class Either<TLeft, TRight>
    {
        public abstract bool IsRight { get; }

        public abstract Option<TLeft> LeftValue { get; }
        public abstract Option<TRight> RightValue { get; }
        public abstract Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, TNewRight> map);
        public abstract Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> map);

        public abstract Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, TNewLeft> map);
        public abstract Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> map);

        public abstract TRight Handle(Func<TLeft, TRight> map);

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Left<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Right<TLeft, TRight>(right);
        }
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Success<TLeft, TRight>(TRight right)
        {
            return new Right<TLeft, TRight>(right);
        }

        public static Either<TLeft, TRight> Error<TLeft, TRight>(TLeft left)
        {
            return new Left<TLeft, TRight>(left);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, TResult>(this Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Func<T1, T2, Either<TLeft, TResult>> map, Func<TLeft, TLeft, TLeft> combineError)
        {
            var mapSuccess = a
                .MapError(el1 => b
                    .MapError(el2 => combineError(el1, el2))
                    .MapSuccess(_ => Either.Error<TLeft, T1>(el1)))
                .MapSuccess(x => b
                    .MapSuccess(y => map(x, y))
                    .MapError(el => el));

            return mapSuccess;
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, TResult>(this 
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Func<T1, T2, T3, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, (arg1, arg2) => Success<TLeft, (T1, T2)>((arg1, arg2)), combineError);
            return r.Combine(c, (o, arg3) => onSuccess(o.Item1, o.Item2, arg3), combineError);
        }

        public static Either<TLeft, TResult> Combine<TLeft, T1, T2, T3, T4, TResult>(
            Either<TLeft, T1> a,
            Either<TLeft, T2> b,
            Either<TLeft, T3> c,
            Either<TLeft, T4> d,
            Func<T1, T2, T3, T4, Either<TLeft, TResult>> onSuccess, Func<TLeft, TLeft, TLeft> combineError)
        {
            var r = Combine(a, b, c, (x, y, z) => Success<TLeft, (T1, T2, T3)>((x, y, z)), combineError);
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
            var r = Combine(a, b, c, d, (x1, x2, x3, x4) => Success<TLeft, (T1, T2, T3, T4)>((x1, x2, x3, x4)), combineError);
            return r.Combine(e, (prev, cur) => onSuccess(prev.Item1, prev.Item2, prev.Item3, prev.Item4, cur), combineError);
        }
        
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

        public static Either<TLeft, IEnumerable<TResult>> Combine<TLeft, TResult>(this IEnumerable<Either<TLeft, TResult>> eithers, Func<TLeft, TLeft, TLeft> combineError)
        {
            var errors = eithers.Partition(x => x.IsRight);

            if (errors.False.Any())
            {
                var aggregate = errors.False
                    .SelectMany(either => either.LeftValue.ToEnumerable())
                    .Aggregate(combineError);

                return Error<TLeft, IEnumerable<TResult>>(aggregate);
            }

            var p = errors.True.SelectMany(either => either.RightValue.ToEnumerable());
            return Success<TLeft, IEnumerable<TResult>>(p);
        }
    }
}