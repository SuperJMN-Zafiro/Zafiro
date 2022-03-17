﻿using Core;
using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem.Smart;

public class SmartZafiroFileSystem : IZafiroFileSystem
{
    private readonly HashSet<CopyOperationMetadata> hashSet;
    private readonly IZafiroFileSystem inner;

    public SmartZafiroFileSystem(IZafiroFileSystem inner, Host host, HashSet<CopyOperationMetadata> hashSet,
        Maybe<ILogger> logger)
    {
        this.inner = inner;
        Host = host;
        Logger = logger;
        this.hashSet = hashSet;
    }

    public Host Host { get; }
    public Maybe<ILogger> Logger { get; }

    public Result<IZafiroFile> GetFile(ZafiroPath path)
    {
        return inner.GetFile(path)
            .Map(file => (IZafiroFile) new SmartZafiroFile(file, this));
    }

    public Result<IZafiroDirectory> GetDirectory(ZafiroPath path)
    {
        return inner.GetDirectory(path)
            .Map(dir => (IZafiroDirectory) new SmartZafiroDirectory(dir, this));
    }

    public void RemoveHash(ZafiroPath getHash)
    {
        hashSet.RemoveWhere(d => d.Source.Equals(getHash));
    }

    public bool ContainsHash(IZafiroFile origin, IZafiroFile destination, Hash hash)
    {
        return hashSet.Contains(new CopyOperationMetadata(Host, origin.Path, destination.Path, hash));
    }

    public void AddHash(IZafiroFile source, IZafiroFile destination, Hash hash)
    {
        hashSet.Add(new CopyOperationMetadata(Host, source.Path, destination.Path, hash));
    }
}