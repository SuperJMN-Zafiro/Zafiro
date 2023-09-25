using System;
using System.Collections.Generic;

namespace Zafiro.Intervals
{
    public class Interval<T> : IInterval<T> where T : IComparable
    {
        public Interval(T start, T end)
        {
            Start = start;
            End = end;
        }

        public static Interval<T> Empty => new Interval<T>(default, default);
        
        public T Start { get; }
        public T End { get; }

        protected bool Equals(Interval<T> other)
        {
            return EqualityComparer<T>.Default.Equals(End, other.End) &&
                   EqualityComparer<T>.Default.Equals(Start, other.Start);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;

            return Equals((Interval<T>) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(End, Start);
        }

        public static Interval<T> Intersection(Interval<T> a, Interval<T> b)
        {
            if (b.Start.GreaterThanOrEqual(a.End) || a.Start.GreaterThanOrEqual(b.End)) return Empty;

            var start = a.Start.Max(b.Start);
            var end = a.End.Min(b.End);

            return new Interval<T>(start, end);
        }

        public override string ToString()
        {
            return Equals(Start, End) ? "<Empty>" :  $"[{Start}, {End})";
        }

        public static bool Overlaps(Interval<T> a, Interval<T> b)
        {
            return a.Start.LessThanOrEqual(b.End) && b.Start.LessThanOrEqual(a.End);
        }

        public bool Overlaps(Interval<T> another)
        {
            return Overlaps(this, another);
        }

        public Interval<T> Intersection(Interval<T> another)
        {
            return Intersection(this, another);
        }

        public bool IsEmpty => Equals(Empty);
    }
}