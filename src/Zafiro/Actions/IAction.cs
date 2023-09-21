using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public interface IAction<T> where T : IProgress
{
    IObservable<T> Progress { get; }
    Task<Result> Execute(CancellationToken ct);
}