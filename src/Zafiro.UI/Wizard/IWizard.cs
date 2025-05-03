using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizard;

public interface IWizard
{
    int CurrentIndex { get; }

    object CurrentPage { get; }

    public MaybeViewModel<string> CurrentTitle { get; }

    ReactiveCommand<Unit, Unit> BackCommand { get; }
    ReactiveCommand<Unit, Unit> NextCommand { get; }
}

public class MaybeViewModel<T>(Maybe<T> maybe)
{
    public T? Value => maybe.GetValueOrDefault();
    public bool HasValue => maybe.HasValue;
    public bool HasNoValue => maybe.HasNoValue;
}