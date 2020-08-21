using System;
using Optional;

namespace Zafiro.Core.Patterns
{
    public abstract class Either<TLeft, TRight>
    {
        public abstract Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, TNewRight> map);
        public abstract Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> map);

        public abstract Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, TNewLeft> map);
        public abstract Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> map);

        public abstract TRight Handle(Func<TLeft, TRight> map);

        public abstract bool IsRight { get; }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Left<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Right<TLeft, TRight>(right);
        }

        public abstract Option<TLeft> LeftValue { get; }
        public abstract Option<TRight> RightValue { get; }
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
    }
}