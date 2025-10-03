// Comments in English. No underscores for private fields.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace Zafiro.Settings;


public sealed class JsonSettingsStore(JsonSerializerOptions? options = null) : ISettingsStore
{
    readonly JsonSerializerOptions options = options ?? new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() }
    };

    // Tolerant but predictable JSON.
    public Result<T> Load<T>(string path, Func<T> createDefault)
    {
        try
        {
            if (!File.Exists(path))
            {
                var def = createDefault();
                var save = Save(path, def);
                return save.IsFailure ? Result.Failure<T>(save.Error) : Result.Success(def);
            }

            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var loaded = JsonSerializer.Deserialize<T>(stream, options);
            return loaded is null
                ? Result.Failure<T>($"Invalid JSON in settings file: {path}.")
                : Result.Success(loaded);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"Load error at {path}: {ex.Message}");
        }
    }

    public Result Save<T>(string path, T instance)
    {
        try
        {
            var dir = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(dir);

            // Write to a temp file in the same directory and replace atomically.
            var tmp = Path.Combine(dir, Path.GetRandomFileName());
            using (var stream = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, instance, options);
                stream.Flush(true);
            }

            if (File.Exists(path))
                File.Replace(tmp, path, null, ignoreMetadataErrors: true);
            else
                File.Move(tmp, path);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Save error at {path}: {ex.Message}");
        }
    }
}