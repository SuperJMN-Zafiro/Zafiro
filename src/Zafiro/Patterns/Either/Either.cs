using System;
using System.Diagnostics;
using Optional;

namespace Zafiro.Patterns.Either
{
    [Obsolete("Use CSharpFunctionalExtensions. See https://github.com/vkhorikov/CSharpFunctionalExtensions")]
    [DebuggerTypeProxy(typeof(Either<,>.EitherProxy))]
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

        public override string ToString()
        {
            var leftOrRight = this.IsRight() ? "Right" : "Left";
            return $"{leftOrRight}({this.ValueOrDefault()})";
        }

        private class EitherProxy
        {
            private readonly Either<TLeft, TRight> instance;

            public EitherProxy(Either<TLeft, TRight> instance)
            {
                this.instance = instance;
            }

            public object Value => instance.ValueOrDefault();
            public Type Type => instance.ValueOrDefault().GetType();
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