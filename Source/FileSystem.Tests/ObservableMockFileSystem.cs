using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace FileSystem.Tests;

public class ObservableMockFileSystem : MockFileSystem
{
    private readonly ObservableFileStreamFactory fileStream;

    public ObservableMockFileSystem()
    {
        fileStream = new ObservableFileStreamFactory(this);
    }

    public override IFileStreamFactory FileStream => fileStream;
    public IObservable<string> FileStreamCreated => fileStream.Created;
}