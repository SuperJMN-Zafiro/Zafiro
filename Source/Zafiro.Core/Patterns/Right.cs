using System;
using Optional;

namespace Zafiro.Core.Patterns
{
    internal class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        public Right(TRight value)
        {
            Value = value;
        }

        private TRight Value { get; }

        public override bool IsRight => true;
        public override Option<TLeft> LeftValue => Optional.Option.None<TLeft>();
        public override Option<TRight> RightValue => Value.Some();

        public override Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, TNewRight> map)
        {
            return Either.Success<TLeft, TNewRight>(map(Value));
        }

        public override Either<TLeft, TNewRight> MapSuccess<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> map)
        {
            return map(Value);
        }

        public override Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, TNewLeft> map)
        {
            return Either.Success<TNewLeft, TRight>(Value);
        }

        public override Either<TNewLeft, TRight> MapError<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> map)
        {
            return Either.Success<TNewLeft, TRight>(Value);
        }

        public override TRight Handle(Func<TLeft, TRight> map)
        {
            return Value;
        }

        public override void WhenRight(Action<TRight> action)
        {
            action(Value);
        }

        public override void WhenLeft(Action<TLeft> action1)
        {
        }
    }
}