using Optional;

namespace Zafiro.Core.Patterns
{
    public class Either<TLeft, TRight>
    {
        public Either(TLeft left)
        {
            Left = left.Some();
        }

        public Either(TRight right)
        {
            Right = right.Some();
        }

        public Option<TLeft> Left { get; }
        public Option<TRight> Right { get; }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Success<TLeft, TRight>(TRight right)
        {
            return new Either<TLeft, TRight>(right);
        }

        public static Either<TLeft, TRight> Error<TLeft, TRight>(TLeft left)
        {
            return new Either<TLeft, TRight>(left);
        }
    }
}