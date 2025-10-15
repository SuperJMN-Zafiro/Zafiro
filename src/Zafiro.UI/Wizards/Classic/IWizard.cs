using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Classic.Builder;

namespace Zafiro.UI.Wizards.Classic;

/// <summary>
/// Non-generic Classic wizard contract.
/// </summary>
public interface IWizard
{
    /// <summary>Command to navigate backward.</summary>
    IEnhancedCommand Back { get; }
    /// <summary>Command to navigate forward.</summary>
    IEnhancedCommand Next { get; }
    /// <summary>Gets the current step content.</summary>
    IStep Content { get; }
    /// <summary>Signals whether the current step is the last page.</summary>
    IObservable<bool> IsLastPage { get; }
    /// <summary>Signals whether the current step is valid.</summary>
    IObservable<bool> IsValid { get; }
    /// <summary>Signals whether the current step is busy.</summary>
    IObservable<bool> IsBusy { get; }
    /// <summary>Observable index of the current page.</summary>
    IObservable<int> PageIndex { get; }
    /// <summary>Total number of pages.</summary>
    int TotalPages { get; }
}

/// <summary>
/// Classic wizard exposing a synchronous result.
/// </summary>
public interface IWizard<out TResult> : IWizard
{
    /// <summary>Computes the final result from the last step.</summary>
    TResult GetResult();
}
