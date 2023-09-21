using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public interface IProgress
{
    public Maybe<double> Value { get; }
}