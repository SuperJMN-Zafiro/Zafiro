using System.IO.Abstractions;

namespace FileSystem;

public class SmartFileComparer : IFileComparer
{
    public bool AreEqual(IFileInfo one, IFileInfo another)
    {
        return ReadContent(one).Equals(ReadContent(another));
    }

    private static string ReadContent(IFileInfo one)
    {
        using var openRead = one.OpenRead();
        var streamReader = new StreamReader(openRead);
        var anotherContent = streamReader.ReadToEnd();
        return anotherContent;
    }
}