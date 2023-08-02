using System;
using Optional.Unsafe;

namespace Zafiro.Zafiro.Patterns.Either
{
    public static class Extra
    {
        public static void WhenRight<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TRight> action)
        {
            either.Right.MatchSome(action);
        }

        public static void WhenLeft<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> action)
        {
            either.Left.MatchSome(action);
        }

        public static object ValueOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either) => either.Left
            .Map(left => (object) left)
            .Else(() => either.Right.Map(right => (object) right))
            .ValueOrDefault();

        public static bool IsRight<TLeft, TRight>(this Either<TLeft, TRight> either) => either.Right.HasValue;
    }
}