using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public interface INewWizard
{
    IEnhancedCommand Next { get; }
    IEnhancedCommand Back { get; }
    IPage CurrentPage { get; }
    int TotalPages { get; }
}

public interface INewWizard<out T> : INewWizard
{
    IObservable<T> Finished { get; }
}