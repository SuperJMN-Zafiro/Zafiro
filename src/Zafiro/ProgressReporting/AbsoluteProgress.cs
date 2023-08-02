﻿namespace Zafiro.Core.ProgressReporting
{
    public class AbsoluteProgress<T> : Progress
    {
        public AbsoluteProgress(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}