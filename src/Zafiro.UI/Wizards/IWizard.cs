using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public interface IWizard
{
    int CurrentPageIndex { get; }
    int TotalPages { get; }
    object CurrentPage { get; }
    public MaybeViewModel<string> CurrentTitle { get; }
    IEnhancedUnitCommand BackCommand { get; }
    IEnhancedCommand<Result<object>> NextCommand { get; }
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