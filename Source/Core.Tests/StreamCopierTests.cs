using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;
using Zafiro.UI;

namespace Core.Tests;

public class StreamCopierTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new StreamCopier(
            GetResponse, 
            () => Task.FromResult((Stream)File.OpenWrite(@"D:\5 - Unimportant\Descargas\gpl2.txt")));
        var result = await sut.Start.Execute();
        result.Should().BeSuccess();
    }

    private static async Task<Stream> GetResponse()
    {
        var response = new HttpResponseMessage();
        response.Content = new StringContent("Saludos, colleiga");
        return await HttpResponseMessageStream.Create(response);
    }
}