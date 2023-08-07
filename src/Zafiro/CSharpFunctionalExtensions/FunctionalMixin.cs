using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Mixins;

namespace Zafiro.CSharpFunctionalExtensions;

public static class FunctionalMixin
{
    public static IObservable<Unit> Successes(this IObservable<Result> self) => self.Where(a => a.IsSuccess).ToSignal();

    public static IObservable<T> Successes<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsSuccess).Select(x => x.Value);

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self) => self.Select(a => a.IsSuccess);

    public static IObservable<bool> IsSuccess(this IObservable<Result> self) => self.Select(a => a.IsSuccess);

    public static IObservable<bool> IsFailure(this IObservable<Result> self) => self.Select(a => a.IsFailure);

    public static IObservable<string> Failures(this IObservable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<string> Failures<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);
    
    public static IObservable<T> Values<T>(this IObservable<Maybe<T>> self) => self.Where(x => x.HasValue).Select(x => x.Value);

    public static IEnumerable<string> Failures(this IEnumerable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<string> Failures<T>(this IEnumerable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess)
            .Select(x => x.Value);
    }

    public static IEnumerable<string> NotNullOrEmpty(this IEnumerable<string> self)
    {
        return self.Where(s => !string.IsNullOrWhiteSpace(s));
    }
}