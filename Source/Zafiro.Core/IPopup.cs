using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Zafiro.Core
{
    public interface IPopup : IContextualizable
    {
        void Close();
        Task Show();
        string Title { get; set; }
        IObservable<Unit> Shown { get; }
    }
}