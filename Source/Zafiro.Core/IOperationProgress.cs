using System;
using Zafiro.Core.ProgressReporting;

namespace Zafiro.Core
{
    public interface IOperationProgress
    {
        IObservable<Progress> Progress { get; }
        void Send(Progress current);
    }
}