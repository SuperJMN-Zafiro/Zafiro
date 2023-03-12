using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using Zafiro.UI.Transfers;

namespace Core.Tests;

public class HttpTransferUnitTests
{
    [Fact]
    public async Task TransferTest()
    {
        var input = new MemoryStream("test content"u8.ToArray());
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created))
            .Verifiable();
        var sut = new HttpPostTransferUnit("Test", () => Task.FromResult((Stream) input), () => new HttpClient(handlerMock.Object), new Uri("http://192.168.1.31:8888/file.txt"));

        await sut.Start.Execute();

        handlerMock.Verify();
    }
}