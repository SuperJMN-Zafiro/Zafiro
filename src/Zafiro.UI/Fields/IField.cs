using System.Reactive;
using ReactiveUI;

namespace Zafiro.UI.Fields;

public interface IField : IValidatable
{
    public IObservable<bool> IsDirty { get; }
    ReactiveCommandBase<Unit, Unit> Commit { get; }
    ReactiveCommandBase<Unit, Unit> Rollback { get; }
}