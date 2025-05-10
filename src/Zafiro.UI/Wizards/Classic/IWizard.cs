using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Classic.Builder;

namespace Zafiro.UI.Wizards.Classic;

public interface IWizard
{
    IEnhancedCommand Back { get; }
    IEnhancedCommand Next { get; }
    IStep Content { get; }
    IObservable<bool> IsLastPage { get; }
    IObservable<bool> IsValid { get; }
    IObservable<bool> IsBusy { get; }
    IObservable<int> PageIndex { get; }
    int TotalPages { get; }
}

public interface IWizard<out TResult> : IWizard
{
    TResult GetResult();
}