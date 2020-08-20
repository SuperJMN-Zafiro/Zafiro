using System;

namespace Zafiro.Core.Patterns
{
    internal class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        public Left(TLeft value)
        {
            Value = value;
        }

        public TLeft Value { get; }

        public override Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, TNewRight> map)
        {
            return Either.Error<TLeft, TNewRight>(Value);
        }

        public override Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> map)
        {
            return Either.Error<TLeft, TNewRight>(Value);
        }

        public override Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, TNewLeft> map)
        {
            return Either.Error<TNewLeft, TRight>(map(Value));
        }

        public override Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> map)
        {
            return map(Value);
        }

        public override TRight Handle(Func<TLeft, TRight> map)
        {
            return map(Value);
        }

        public override bool IsRight => false;
    }
}