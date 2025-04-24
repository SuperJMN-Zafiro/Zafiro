namespace Zafiro.UI;

public interface IBusy
{
    public IObservable<bool> IsBusy { get; }
}