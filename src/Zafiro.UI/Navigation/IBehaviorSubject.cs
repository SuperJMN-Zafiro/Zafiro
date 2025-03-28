namespace Zafiro.UI.Navigation;

public interface IBehaviorSubject<T> : IObservable<T>
{
    T Value { get; }
}