using System;
using System.Linq;
using Optional;
using Optional.Linq;

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

        public static Either<ErrorList, TResult> Summarize<T1, T2, TResult>(Either<ErrorList, T1> a,
            Either<ErrorList, T2> b,
            Func<T1, T2, Either<ErrorList, TResult>> onSuccess)
        {
            var success = from av in a.RightValue
                from bv in b.RightValue
                select onSuccess(av, bv);
            var errorList = new[]
                {
                    a.LeftValue,
                    b.LeftValue
                }
                .SelectMany(x => x.ToEnumerable())
                .Aggregate((x, y) => new ErrorList(x.Concat(y)));

            return success.ValueOr(() => errorList);
        }

        public static Either<ErrorList, TResult> Summarize<T1, T2, T3, TResult>(Either<ErrorList, T1> a,
            Either<ErrorList, T2> b, Either<ErrorList, T3> c,
            Func<T1, T2, T3, Either<ErrorList, TResult>> onSuccess)
        {
            var success =
                from av in a.RightValue
                from bv in b.RightValue
                from cv in c.RightValue
                select onSuccess(av, bv, cv);

            var errorList = new[]
                {
                    a.LeftValue,
                    b.LeftValue,
                    c.LeftValue
                }
                .SelectMany(x => x.ToEnumerable())
                .Aggregate((x, y) => new ErrorList(x.Concat(y)));

            return success.ValueOr(() => errorList);
        }

        public static Either<ErrorList, TResult> Summarize<T1, T2, T3, T4, TResult>(Either<ErrorList, T1> a,
            Either<ErrorList, T2> b, Either<ErrorList, T3> c, Either<ErrorList, T4> d,
            Func<T1, T2, T3, T4, Either<ErrorList, TResult>> onSuccess)
        {
            var success = from av in a.RightValue
                from bv in b.RightValue
                from cv in c.RightValue
                from dv in d.RightValue
                select onSuccess(av, bv, cv, dv);

            var errorList = new[]
                {
                    a.LeftValue,
                    b.LeftValue,
                    c.LeftValue,
                    d.LeftValue
                }
                .SelectMany(x => x.ToEnumerable())
                .Aggregate((x, y) => new ErrorList(x.Concat(y)));

            return success.ValueOr(() => errorList);
        }

        public static Either<ErrorList, TResult> Summarize<T1, T2, T3, T4, T5, TResult>(
            Either<ErrorList, T1> a,
            Either<ErrorList, T2> b,
            Either<ErrorList, T3> c,
            Either<ErrorList, T4> d,
            Either<ErrorList, T5> e,
            Func<T1, T2, T3, T4, T5, Either<ErrorList, TResult>> onSuccess)
        {
            var success = from av in a.RightValue
                from bv in b.RightValue
                from cv in c.RightValue
                from dv in d.RightValue
                from ev in e.RightValue
                select onSuccess(av, bv, cv, dv, ev);

            var errorList = new[]
                {
                    a.LeftValue,
                    b.LeftValue,
                    c.LeftValue,
                    d.LeftValue,
                    e.LeftValue
                }
                .SelectMany(x => x.ToEnumerable())
                .Aggregate((x, y) => new ErrorList(x.Concat(y)));

            return success.ValueOr(() => errorList);
        }
    }
}