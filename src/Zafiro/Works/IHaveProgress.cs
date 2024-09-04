using System;
using Zafiro.Progress;

namespace Zafiro.Works;

public interface IHaveProgress
{
    public IObservable<IProgress> Progress { get; }
}