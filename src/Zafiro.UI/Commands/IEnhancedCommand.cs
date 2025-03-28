using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.UI.Commands;

public interface IEnhancedCommand : 
    ICommand,
    IReactiveCommand;

public interface IEnhancedUnitCommand : IReactiveCommand<Unit, Unit>;

public interface IEnhancedCommand<T, Q> : IReactiveCommand<T, Q>,
    IEnhancedCommand;