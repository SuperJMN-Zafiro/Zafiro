﻿using System;
using System.IO;
using System.IO.Abstractions;

namespace Zafiro.DataModel;

public class FileInfoData : IData
{
    public FileInfoData(IFileInfo file)
    {
        Bytes = ((Func<Stream>) file.OpenRead).Chunked(4096 * 2);
        Length = file.Length;
    }

    public long Length { get; }

    public IObservable<byte[]> Bytes { get; }
}