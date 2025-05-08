using System.Windows.Input;

namespace Zafiro.UI.Commands;

public interface IEnhancedCommand :
    ICommand,
    IReactiveCommand;

public interface IEnhancedUnitCommand : IReactiveCommand<Unit, Unit>, IEnhancedCommand;

public interface IEnhancedCommand<in T, Q> : IReactiveCommand<T, Q>,
    IEnhancedCommand;

public interface IEnhancedCommand<out T> : IReactiveCommand<Unit, T>,
    IEnhancedCommand;