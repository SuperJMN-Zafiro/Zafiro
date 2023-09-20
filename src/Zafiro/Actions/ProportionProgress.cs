namespace Zafiro.Actions;

public class ProportionProgress : IProportionProgress
{
    public ProportionProgress()
    {
    }

    public ProportionProgress(double proportion)
    {
        Proportion = proportion;
    }

    public double Proportion { get; }
}