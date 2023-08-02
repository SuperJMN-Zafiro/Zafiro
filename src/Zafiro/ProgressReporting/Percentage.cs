namespace Zafiro.ProgressReporting
{
    public class Percentage : Progress
    {
        public double Value { get; }

        public Percentage(double value)
        {
            Value = value;
        }
    }
}