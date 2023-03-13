using System;

namespace Zafiro.Core.IO;

public interface IPositionable 
{
    IObservable<long> Positions { get; }
}

public interface IHaveProgress
{
    IObservable<double> Progress { get; }
}