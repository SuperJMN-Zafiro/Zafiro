using System.Windows.Input;

namespace Zafiro.UI.Commands;

public interface IEnhancedCommand : 
    ICommand,
    IReactiveCommand;

public interface IEnhancedUnitCommand : IReactiveCommand<Unit, Unit>;

public interface IEnhancedCommand<T, Q> : IReactiveCommand<T, Q>,
    IEnhancedCommand;