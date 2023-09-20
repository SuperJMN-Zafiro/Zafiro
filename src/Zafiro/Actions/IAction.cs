using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public interface IAction
{
    IObservable<IProportionProgress> Progress { get; }
    Task<Result> Execute(CancellationToken ct);
}