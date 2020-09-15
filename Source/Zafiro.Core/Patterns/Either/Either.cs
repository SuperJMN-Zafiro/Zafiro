using System;
using Optional;

namespace Zafiro.Core.Patterns.Either
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

        protected bool Equals(Either<TLeft, TRight> other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Either<TLeft, TRight>) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right);
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