using System;
using System.IO.Abstractions;
using Zafiro.Reactive;

namespace Zafiro.DataModel;

public class FileInfoData : IData
{
    public FileInfoData(IFileInfo file)
    {
        Bytes = StreamMixin.ToObservableChunked(file.OpenRead);
        Length = file.Length;
    }

    public long Length { get; }

    public IObservable<byte[]> Bytes { get; }
}