using System.Reactive;

namespace Zafiro.UI;

public interface IView : IContextualizable
{
    string Title { get; set; }
    IObservable<Unit> Shown { get; }
    void Close();
    Task ShowAsModal();
}