using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;

namespace Zafiro.Settings;

public sealed class JsonSettings<T> : ISettings<T>, IDisposable
{
    private readonly string path;
    private readonly ISettingsStore store;
    private readonly Func<T> createDefault;
    private readonly IEqualityComparer<T> comparer;
    private readonly ReplaySubject<T> changes = new(1);
    private readonly object gate = new();

    private Maybe<T> current = Maybe<T>.None;
    private bool disposed;

    public JsonSettings(string path, ISettingsStore store, Func<T> createDefault, IEqualityComparer<T>? equalityComparer = null)
    {
        this.path = path;
        this.store = store;
        this.createDefault = createDefault;
        comparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    public Result Update(Func<T, T> mutate)
    {
        if (mutate == null)
        {
            return Result.Failure("Mutate delegate cannot be null.");
        }

        while (true)
        {
            var snapshotResult = EnsureSnapshot();
            if (snapshotResult.IsFailure)
            {
                return Result.Failure(snapshotResult.Error);
            }

            var snapshot = snapshotResult.Value;
            T toPublish;

            lock (gate)
            {
                if (disposed)
                {
                    return Result.Failure("Settings disposed.");
                }

                if (!current.HasValue || !comparer.Equals(current.Value, snapshot))
                {
                    continue;
                }

                T next;

                try
                {
                    next = mutate(snapshot);
                }
                catch (Exception ex)
                {
                    return Result.Failure($"Update mutate error: {ex.Message}");
                }

                var save = store.Save(path, next);
                if (save.IsFailure)
                {
                    return save;
                }

                current = next;
                toPublish = next;
            }

            Publish(toPublish);
            return Result.Success();
        }
    }

    public Result<T> Get()
    {
        lock (gate)
        {
            if (disposed)
            {
                return Result.Failure<T>("Settings disposed.");
            }

            if (current.HasValue)
            {
                return Result.Success(current.Value);
            }
        }

        var load = store.Load(path, createDefault);
        if (load.IsFailure)
        {
            return load;
        }

        var publish = false;
        T value = default!;

        lock (gate)
        {
            if (disposed)
            {
                return Result.Failure<T>("Settings disposed.");
            }

            if (!current.HasValue)
            {
                current = load.Value;
                publish = true;
                value = load.Value;
            }
            else
            {
                value = current.Value;
            }
        }

        if (publish)
        {
            Publish(value);
            return load;
        }

        return Result.Success(value);
    }

    public Result Reload()
    {
        var load = store.Load(path, createDefault);
        if (load.IsFailure)
        {
            return load;
        }

        T toPublish;

        lock (gate)
        {
            if (disposed)
            {
                return Result.Failure("Settings disposed.");
            }

            current = load.Value;
            toPublish = load.Value;
        }

        Publish(toPublish);
        return Result.Success();
    }

    public IObservable<T> Changes => changes.AsObservable().DistinctUntilChanged(comparer);

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        changes.OnCompleted();
        changes.Dispose();
    }

    private Result<T> EnsureSnapshot()
    {
        lock (gate)
        {
            if (disposed)
            {
                return Result.Failure<T>("Settings disposed.");
            }

            if (current.HasValue)
            {
                return Result.Success(current.Value);
            }
        }

        var load = store.Load(path, createDefault);
        if (load.IsFailure)
        {
            return load;
        }

        lock (gate)
        {
            if (disposed)
            {
                return Result.Failure<T>("Settings disposed.");
            }

            if (!current.HasValue)
            {
                current = load.Value;
            }

            return Result.Success(current.Value);
        }
    }

    private void Publish(T value)
    {
        try
        {
            changes.OnNext(value);
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
