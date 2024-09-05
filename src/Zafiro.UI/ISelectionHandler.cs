using System.Reactive;
using ReactiveUI;

namespace Zafiro.UI;

public interface ISelectionHandler
{
    ReactiveCommand<Unit, Unit> SelectNone { get; }
    ReactiveCommand<Unit, Unit> SelectAll { get; }
    IObservable<int> SelectionCount { get; }
    IObservable<int> TotalCount { get; }
}