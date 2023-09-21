using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public class LongProgress : IProgress
{
    public LongProgress(long current, long total)
    {
        Current = current;
        Total = total;
    }

    public LongProgress()
    {
    }

    public Maybe<long> Current { get; }
    public Maybe<long> Total { get; }
    public Maybe<double> Value => from n in Current from l in Total select (double)n / l;
}