using CSharpFunctionalExtensions;
using FluentAssertions;
using Serilog;
using Xunit.Abstractions;
using Zafiro.Deployment.Core;

namespace Zafiro.Deployment.Tests;

public class CommandTests
{
    public CommandTests(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(output)
            .CreateLogger();
    }

    [Fact]
    public async Task TestGit()
    {
        var command = new Command(Log.Logger.AsMaybe());
        var execute = await command.Execute("git", "clone");
        execute.Should().Fail();
    }
}