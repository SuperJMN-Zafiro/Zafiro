using System;

namespace Zafiro.Zafiro.Intervals
{
    public interface IInterval<out T> where T : IComparable
    {
        T Start { get; }
        T End { get; }
    }
}