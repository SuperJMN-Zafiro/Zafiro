using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.Misc;

public static class LogMixin
{
    public static Result LogInfo(this Result result, string str, params object[] propertyValues) => result
        .Tap(() => Log.Information(str, propertyValues))
        .TapError(Log.Error);

    public static Result LogError(this Result result, Func<string, (string, object[] propertyValues)> getError) => result
        .TapError(error =>
        {
            var valueTuple = getError(error);
            Log.Error(valueTuple.Item1, valueTuple.propertyValues);
        });

    public static Result<T> LogError<T>(this Result<T> result, Func<string, (string, object[] propertyValues)> getError) => result
        .TapError(error =>
        {
            var valueTuple = getError(error);
            Log.Error(valueTuple.Item1, valueTuple.propertyValues);
        });

    public static Result<T> LogInfo<T>(this Result<T> result, string str, params object[] propertyValues) => result
        .Tap(() => Log.Information(str, propertyValues))
        .TapError(Log.Error);

    public static Task<Result<T>> LogInfo<T>(this Task<Result<T>> result, string str, params object[] propertyValues) => result
        .Tap(() => Log.Information(str, propertyValues))
        .TapError(Log.Error);

    public static Task<Result> LogInfo(this Task<Result> result, string str, params object[] propertyValues) => result
        .Tap(() => Log.Information(str, propertyValues))
        .TapError(Log.Error);
}