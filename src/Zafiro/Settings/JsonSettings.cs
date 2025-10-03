using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;

namespace Zafiro.Settings;

public sealed class JsonSettings<T>(string path, ISettingsStore store, Func<T> createDefault, IEqualityComparer<T>? equalityComparer = null)
    : ISettings<T>, IDisposable
{
    private readonly ReplaySubject<T> subject = new(1);
    private readonly object gate = new();

    Maybe<T> current = Maybe<T>.None;
    bool disposed;
    private readonly IEqualityComparer<T> equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

    public Result Update(Func<T, T> mutate)
    {
        while (true)
        {
            // 1) Snapshot
            var snapshotResult = EnsureSnapshot();
            if (snapshotResult.IsFailure) return snapshotResult.ConvertFailure();
            var snapshot = snapshotResult.Value;

            // 2) Mutate outside lock
            T next;
            try
            {
                next = mutate(snapshot);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Update mutate error: {ex.Message}");
            }

            // 3) Persist
            var save = store.Save(path, next);
            if (save.IsFailure) return save;

            // 4) Commit-if-unchanged
            bool committed;
            lock (gate)
            {
                if (disposed) return Result.Failure("Settings disposed.");
                committed = current.HasValue && equalityComparer.Equals(current.Value, snapshot);
                if (committed) current = next;
            }

            if (committed)
            {
                try
                {
                    subject.OnNext(next);
                }
                catch (ObjectDisposedException)
                {
                }

                return Result.Success();
            }
            // else: alguien tocó current; reintenta con el nuevo snapshot.
        }
    }

    // Lazily ensures current is loaded and returns a snapshot to base the update on.
    private Result<T> EnsureSnapshot()
    {
        // Fast path: already loaded
        lock (gate)
        {
            if (disposed) return Result.Failure<T>("Settings disposed.");
            if (current.HasValue) return current.Value;
        }

        // Load outside the lock
        var load = store.Load<T>(path, createDefault);
        if (load.IsFailure) return load;

        // Install if still empty and return snapshot
        lock (gate)
        {
            if (disposed) return Result.Failure<T>("Settings disposed.");
            if (!current.HasValue) current = load.Value;
            return current.Value;
        }
    }

    public Result<T> Get()
    {
        T? toPublish;
        Result<T> result;

        lock (gate)
        {
            if (disposed) return Result.Failure<T>("Settings disposed.");
            if (current.HasValue) return current.Value;

            var load = store.Load(path, createDefault);
            if (load.IsFailure) return load;

            current = load.Value;
            toPublish = load.Value;
            result = load;
        }

        try
        {
            subject.OnNext(toPublish!);
        }
        catch (ObjectDisposedException)
        {
        }

        return result;
    }

    public Result Reload()
    {
        var load = store.Load(path, createDefault);
        if (load.IsFailure) return load;

        T value = load.Value;
        lock (gate)
        {
            if (disposed) return Result.Failure("Settings disposed.");
            current = value;
        }

        try
        {
            subject.OnNext(value);
        }
        catch (ObjectDisposedException)
        {
        }

        return Result.Success();
    }

    public IObservable<T> Changes => subject.AsObservable().DistinctUntilChanged(equalityComparer);

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        subject.OnCompleted();
        subject.Dispose();
    }
}