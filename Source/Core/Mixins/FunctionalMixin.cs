using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.Core.Mixins;

public static class FunctionalMixin
{
    public static IObservable<Unit> WhereSuccess(this IObservable<Result> self) => self.Where(a => a.IsSuccess).ToSignal();

    public static IObservable<string> WhereFailure(this IObservable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<T> WhereSuccess<T>(this IObservable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess)
            .Select(x => x.Value);
    }

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self)
    {
        return self
            .Select(a => a.IsSuccess);
    }

    public static IObservable<T> Values<T>(this IObservable<Maybe<T>> self)
    {
        return self.Where(x => x.HasValue).Select(x => x.Value);
    }
}