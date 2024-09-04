namespace Zafiro.Progress;

public class ProportionalProgress : IProgress
{
    public ProportionalProgress(double ratio)
    {
        Ratio = ratio;
    }

    public double Ratio { get; }
}