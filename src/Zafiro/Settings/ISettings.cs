using System;
using CSharpFunctionalExtensions;

namespace Zafiro.Settings;

public interface ISettings<T>
{
    Result<T> Get();                 // Lazy load. Never throws.
    IObservable<T> Changes { get; }  // Emits after successful Get/Update/Reload.
    Result Update(Func<T, T> mutate);// Mutate->persist atomically->publish.
    Result Reload();                 // Reload from disk and publish.
}