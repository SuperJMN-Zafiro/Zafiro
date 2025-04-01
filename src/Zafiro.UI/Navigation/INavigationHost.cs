namespace Zafiro.UI.Navigation;

public interface INavigationHost : IDisposable
{
    INavigator Navigator { get; }
}