namespace Zafiro.UI;

public interface IValidatable
{
    public IObservable<bool> IsValid { get; }
}