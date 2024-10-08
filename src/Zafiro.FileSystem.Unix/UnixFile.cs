﻿using Zafiro.DataModel;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.Unix;

public class UnixFile : UnixNode, IFile
{
    public UnixFile(IFile file, UnixFileProperties properties) : this(file.Name, file, properties)
    {
    }

    public UnixFile(string name, IData data) : this(name, data, Maybe<UnixFileProperties>.None)
    {
    }

    public UnixFile(string name, IData data, Maybe<UnixFileProperties> properties) : base(name)
    {
        Data = data;
        Properties = properties.GetValueOrDefault(UnixFileProperties.RegularFileProperties);
    }

    public UnixFile(string name) : this(name, DataModel.Data.FromByteArray(Array.Empty<byte>()), Maybe<UnixFileProperties>.None)
    {
    }

    public IData Data { get; }
    public UnixFileProperties Properties { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;

    public override string ToString() => Name;
}