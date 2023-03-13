using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.Core.IO;
using Zafiro.Core.Mixins;
using Zafiro.UI.Transfers;

namespace Core.Tests;

public class TransferUnitTests
{
    [Fact]
    public async Task TransferTest()
    {
        var input = new MemoryStream("test content"u8.ToArray());
        var output = new MemoryStream();
        var sut = new RegularTransferUnit("Test", () => Task.FromResult((Stream)input), () => Task.FromResult(new ProgressNotifyingStream(output, () => input.Length)));

        var obs = new TestScheduler().CreateObserver<double>();

        sut.Percent.Subscribe(obs);
        await sut.Start.Execute();
        obs.Messages.Should().NotBeEmpty();
        output.Position = 0;
        var readToEnd = await output.ReadToEnd();
        readToEnd.Should().Be("test content");
    }
}