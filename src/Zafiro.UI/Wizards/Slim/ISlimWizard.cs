using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public interface ISlimWizard
{
    IEnhancedCommand Next { get; }
    IEnhancedCommand Back { get; }
    IPage CurrentPage { get; }
    int TotalPages { get; }
}

public interface ISlimWizard<out T> : ISlimWizard
{
    IObservable<T> Finished { get; }
}