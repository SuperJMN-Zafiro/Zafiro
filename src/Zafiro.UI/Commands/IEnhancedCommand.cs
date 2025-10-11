using System.Windows.Input;

namespace Zafiro.UI.Commands;

public interface IEnhancedCommand :
    IReactiveObject,
    ICommand,
    IReactiveCommand
{
    public string? Name { get; }
    public string? Text { get; }
}

public interface IEnhancedCommand<in T, out Q> : IReactiveCommand<T, Q>, IEnhancedCommand;

public interface IEnhancedCommand<out T> : IReactiveCommand<Unit, T>, IEnhancedCommand;