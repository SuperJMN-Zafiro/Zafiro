using System;

namespace Zafiro.Core.ProgressReporting;

public class RelativeProgress<T>
{
    public RelativeProgress(T total, T value)
    {
        Total = total;
        Value = value;
    }

    public T Total { get; }
    public T Value { get; }
    public double Proportion => Convert.ToDouble(Value) / Convert.ToDouble(Total);
}