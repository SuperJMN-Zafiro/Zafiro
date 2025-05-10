namespace Zafiro.UI.Wizards.Classic.Builder;

public interface IBusy
{
    public IObservable<bool> IsBusy { get; }
}