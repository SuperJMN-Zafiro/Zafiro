using System;
using Core.ProgressReporting;

namespace Core
{
    public interface IOperationProgress
    {
        IObservable<Progress> Progress { get; }
        void Send(Progress current);
    }
}