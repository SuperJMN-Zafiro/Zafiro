using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;
using Zafiro.Core;
using Zafiro.UI;

namespace Core.Tests;

public class StreamCopierTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new StreamCopier(
            GetResponse, 
            () => Task.FromResult((Stream)File.OpenWrite(@"D:\5 - Unimportant\Descargas\gpl3.txt")));
        var result = await sut.Start.Execute();
        result.Should().BeSuccess();
    }

    private static async Task<Stream> GetResponse()
    {
        var response = await new HttpClient().GetAsync("http://192.168.1.31:8888/devenv_fC8UCYhmLF.mp4");
        //var response = await new HttpClient().GetAsync("https://jsoncompare.org/LearningContainer/SampleFiles/Video/MP4/Sample-MP4-Video-File-for-Testing.mp4");
        return await HttpResponseMessageStream.Create(response);
    }
}