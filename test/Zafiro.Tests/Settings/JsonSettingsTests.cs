using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Settings;

namespace Zafiro.Tests.Settings;

public class JsonSettingsTests
{
    private sealed record CounterSettings(int Value);

    [Fact]
    public async Task Update_should_invoke_mutation_once_per_successful_update()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "settings.json");

        try
        {
            var store = new JsonSettingsStore();
            using var settings = new JsonSettings<CounterSettings>(path, store, () => new CounterSettings(0));

            const int updates = 50;
            var mutationCount = 0;

            var tasks = Enumerable.Range(0, updates)
                .Select(_ => Task.Run(() =>
                {
                    return settings.Update(current =>
                    {
                        Interlocked.Increment(ref mutationCount);
                        return current with { Value = current.Value + 1 };
                    });
                }))
                .ToArray();

            var results = await Task.WhenAll(tasks);
            results.Should().OnlyContain(result => result.IsSuccess);

            var final = settings.Get();
            final.IsSuccess.Should().BeTrue();
            final.Value.Value.Should().Be(updates);
            mutationCount.Should().Be(updates);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
