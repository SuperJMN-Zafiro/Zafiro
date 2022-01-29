using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Tests;

public class CacheFixture
{
    private int openedStreams;

    public int OpenedStreams => openedStreams;

    public async Task StoreSequence(params string[] items)
    {
        var fs = new ObservableMockFileSystem();
        fs.FileStreamCreated.Subscribe(s => openedStreams = OpenedStreams + 1);
        
        foreach (var item in items)
        {
            var cache = await StreamCache.CreateInstance(fs, "C:\\");
            await cache.Store("C:\\File.txt", () => Content(item));
            await StreamCache.Save(cache, fs, "C:\\");
        }
    }

    private static MemoryStream Content(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}