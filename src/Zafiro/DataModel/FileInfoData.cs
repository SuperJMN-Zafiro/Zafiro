using System;
using System.IO.Abstractions;
using System.Reactive.Linq;
using Zafiro.Reactive;

namespace Zafiro.DataModel;

public class FileInfoData : IData
{
    public FileInfoData(IFileInfo file)
    {
        Bytes = Observable.Using(file.OpenRead, stream => stream.ToObservableChunked());
        Length = file.Length;
    }

    public long Length { get; }

    public IObservable<byte[]> Bytes { get; }
}