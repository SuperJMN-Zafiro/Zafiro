using System;
using System.Reactive;
using System.Threading.Tasks;
using Zafiro.Core;

namespace Zafiro.UI
{
    public interface IView : IContextualizable
    {
        void Close();
        Task ShowAsModal();
        string Title { get; set; }
        IObservable<Unit> Shown { get; }
    }
}