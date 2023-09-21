using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public class Progress : IProgress
{
    public Progress()
    {
    }

    public Progress(double value)
    {
        Value = value;
    }

    public Maybe<double> Value { get; }
}