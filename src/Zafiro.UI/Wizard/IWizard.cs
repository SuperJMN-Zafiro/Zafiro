using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizard;

public interface IWizard
{
    int CurrentPageIndex { get; }
    int TotalPages { get; }
    object CurrentPage { get; }
    public MaybeViewModel<string> CurrentTitle { get; }
    ReactiveCommand<Unit, Unit> BackCommand { get; }
    ReactiveCommand<Unit, Unit> NextCommand { get; }
    public string NextText { get; }
    IObservable<object?> Finished { get; }
}

public interface IWizard<out T> : IWizard
{
    new T CurrentPage { get; }
    new IObservable<T> Finished { get; }
}

public class MaybeViewModel<T>(Maybe<T> maybe)
{
    public T? Value => maybe.GetValueOrDefault();
    public bool HasValue => maybe.HasValue;
    public bool HasNoValue => maybe.HasNoValue;
}