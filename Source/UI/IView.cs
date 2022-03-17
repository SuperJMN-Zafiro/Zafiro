using System;
using System.Reactive;
using System.Threading.Tasks;
using Core;

namespace UI
{
    public interface IView : IContextualizable
    {
        void Close();
        Task ShowAsModal();
        string Title { get; set; }
        IObservable<Unit> Shown { get; }
    }
}