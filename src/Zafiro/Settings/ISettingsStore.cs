using System;
using CSharpFunctionalExtensions;

namespace Zafiro.Settings;

public interface ISettingsStore
{
    Result<T> Load<T>(string path, Func<T> createDefault);
    Result Save<T>(string path, T instance);
}