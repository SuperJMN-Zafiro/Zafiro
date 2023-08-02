using System;
using Zafiro.Zafiro.ProgressReporting;

namespace Zafiro.Zafiro
{
    public interface IOperationProgress
    {
        IObservable<Progress> Progress { get; }
        void Send(Progress current);
    }
}