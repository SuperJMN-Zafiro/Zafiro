namespace Zafiro.UI.Navigation;

public interface ISectionScope : IDisposable
{
    INavigator Navigator { get; }
}