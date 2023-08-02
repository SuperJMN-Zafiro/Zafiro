using System;
using Optional.Unsafe;

namespace Zafiro.Patterns.Either
{
    public static class MapExtensions
    {
        public static Either<TLeft, TNewRight> MapRight<TLeft, TRight, TNewRight>(this Either<TLeft, TRight> self,
            Func<TRight, TNewRight> map)
        {
            return self.Right.Match(
                right => new Either<TLeft, TNewRight>(map(right)),
                () => new Either<TLeft, TNewRight>(self.Left.ValueOrDefault()));
        }

        public static Either<TNewLeft, TRight> MapLeft<TLeft, TRight, TNewLeft>(this Either<TLeft, TRight> self,
            Func<TLeft, TNewLeft> map)
        {
            return self.Left.Match(
                left => new Either<TNewLeft, TRight>(map(left)),
                () => new Either<TNewLeft, TRight>(self.Right.ValueOrDefault()));
        }

        public static Either<TLeft, TNewRight> MapRight<TLeft, TRight, TNewRight>(this Either<TLeft, TRight> self,
            Func<TRight, Either<TLeft, TNewRight>> map)
        {
            return self.Right.Match(
                right => map(right),
                () => new Either<TLeft, TNewRight>(self.Left.ValueOrDefault()));
        }

        public static Either<TNewLeft, TRight> MapLeft<TNewLeft, TLeft, TRight>(this Either<TLeft, TRight> self,
            Func<TLeft, Either<TNewLeft, TRight>> map)
        {
            return self.Left.Match(
                left => map(left),
                () => new Either<TNewLeft, TRight>(self.Right.ValueOrDefault()));
        }

        public static TRight Handle<TLeft, TRight>(this Either<TLeft, TRight> self, Func<TLeft, TRight> turnRight)
        {
            return self.Left.Match(turnRight, () => self.Right.ValueOrDefault());
        }
    }
}