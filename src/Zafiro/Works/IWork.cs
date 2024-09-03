using System;
using CSharpFunctionalExtensions;
using Zafiro.Progress;

namespace Zafiro.Works;

public interface IWork
{
    IObservable<IProgress> Progress { get; }
    IObservable<Result> Execute();
}