using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.Core.Mixins;

public static class FunctionalMixin
{
    public static IObservable<Unit> WhereSuccess(this IObservable<Result> self) => self.Where(a => a.IsSuccess).ToSignal();

    public static IObservable<T> WhereSuccess<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsSuccess)
        .Select(x => x.Value);

    public static IObservable<string> WhereFailure(this IObservable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<string> WhereFailure<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<string> WhereFailure(this IEnumerable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<string> WhereFailure<T>(this IEnumerable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self) => self.Select(a => a.IsSuccess);

    public static IObservable<T> Values<T>(this IObservable<Maybe<T>> self) => self.Where(x => x.HasValue).Select(x => x.Value);
}