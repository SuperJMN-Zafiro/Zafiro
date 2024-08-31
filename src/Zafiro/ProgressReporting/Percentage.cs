namespace Zafiro.ProgressReporting;

public class Percentage : Progress
{
    public Percentage(double value)
    {
        Value = value;
    }

    public double Value { get; }
}