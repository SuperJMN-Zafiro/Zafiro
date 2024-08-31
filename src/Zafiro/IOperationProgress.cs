using System;
using Zafiro.ProgressReporting;

namespace Zafiro;

public interface IOperationProgress
{
    IObservable<Progress> Progress { get; }
    void Send(Progress current);
}