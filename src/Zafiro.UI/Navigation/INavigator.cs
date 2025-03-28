using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation;

public interface INavigator
{
    IEnhancedCommand Back { get; }
    object Content { get; set; }

    Task Go(Func<INavigator, Task<object>> target);
    Task Go(Func<Task<object>> target);
    
    Task Go(Func<INavigator, object> target);
    Task Go<T>();
    Task Go(Func<object> target);
}