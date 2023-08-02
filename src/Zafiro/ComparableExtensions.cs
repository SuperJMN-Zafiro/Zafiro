using System;

namespace Zafiro
{
    public static class ComparableExtensions 
    {
        public static T Max<T>(this T x, T y) where T : IComparable
        {
            return x.GreaterThan(y) ? x : y;
        }

        public static T Min<T>(this T x, T y) where T : IComparable
        {
            return x.LessThan(y) ? x : y;
        }

        public static bool LessThan<T>(this T x, T y) where T : IComparable
        {
            return x.CompareTo(y) < 0;
        }

        public static bool GreaterThan<T>(this T x, T y) where T : IComparable
        {
            return x.CompareTo(y) > 0;
        }

        public static bool LessThanOrEqual<T>(this T x, T y) where T : IComparable
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool GreaterThanOrEqual<T>(this T x, T y) where T : IComparable
        {
            return x.CompareTo(y) >= 0;
        }
    }
}