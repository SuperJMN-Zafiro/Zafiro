using System;

namespace Zafiro.Core
{
    public interface IOperationProgress
    {
        IObservable<Progress> Progress { get; }
        void Send(Progress current);
    }

    public abstract class Progress
    {
    }

    public class Unknown : Progress
    {
    }

    public class UndefinedProgress<T> : Progress
    {
        public UndefinedProgress(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }

    public class Percentage : Progress
    {
        public double Value { get; }

        public Percentage(double value)
        {
            Value = value;
        }
    }

    public class Done : Progress
    {
    }
}