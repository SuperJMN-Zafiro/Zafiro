using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation;

public interface INavigator
{
    IEnhancedCommand Back { get; }

    object Content { get; set; }

    void Go(Func<INavigator, object> target);
    
    public void Go(Func<object> target)
    {
        Go(_ => target());
    }
}