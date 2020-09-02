using System;
using System.Threading.Tasks;
using Optional;

namespace Zafiro.Core.Patterns
{
    internal class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        public Left(TLeft value)
        {
            Value = value;
        }

        private TLeft Value { get; }

        public override bool IsRight => false;

        public override Option<TLeft> LeftValue => Value.Some();
        public override Option<TRight> RightValue => Optional.Option.None<TRight>();

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

        public override void WhenRight(Action<TRight> action)
        {
        }

        public override void WhenLeft(Action<TLeft> action)
        {
            action(Value);
        }

        public override Task WhenRight(Func<TRight, Task> action)
        {
            return Task.CompletedTask;
        }

        public override Task WhenLeft(Func<TLeft, Task> action)
        {
            return action(Value);
        }
    }
}