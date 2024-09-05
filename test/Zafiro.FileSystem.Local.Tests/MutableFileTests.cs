using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local.Tests;

public class MutableFileTests
{
    [Fact]
    public async Task Create_new_file_with_contents()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
        IMutableDirectory directory = new Directory(directoryInfo);
        var result = await directory.CreateFileWithContents("Hola.txt", Data.FromString("hola tío"));
        result.Should().Succeed();
    }
}