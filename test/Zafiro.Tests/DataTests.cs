using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Zafiro.Mixins;
using Zafiro.Reactive;

namespace Zafiro.Tests;

public class DataTests
{
    [Fact]
    public async Task DumpTo_Test()
    {
        var array = "hola"u8.ToArray();
        var p = array.ToObservable().Buffer(2).Select(list => list.ToArray());
        var memoryStream = new MemoryStream();
        await p.WriteTo(memoryStream);

        memoryStream.Position = 0;

        var readBytesToEnd = await memoryStream.ReadBytesToEnd();
        Assert.Equal(array, readBytesToEnd);
    }

    [Fact]
    public async Task Empty_DumpTo()
    {
        var p = Observable.Empty<byte[]>();
        await p.WriteTo(new MemoryStream());
    }
}