using System;

namespace Zafiro.Core.Patterns.Either
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

    }
}