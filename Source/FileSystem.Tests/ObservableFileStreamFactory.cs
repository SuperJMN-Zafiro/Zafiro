using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Win32.SafeHandles;

namespace FileSystem.Tests;

internal class ObservableFileStreamFactory : IFileStreamFactory
{
    private readonly Subject<string> created = new();
    private readonly IFileStreamFactory inner;

    public ObservableFileStreamFactory(IMockFileDataAccessor observableMockFileSystem)
    {
        inner = new MockFileStreamFactory(observableMockFileSystem);
    }

    public IObservable<string> Created => created.AsObservable();

    public Stream Create(string path, FileMode mode)
    {
        created.OnNext(path);
        return inner.Create(path, mode);
    }

    public Stream Create(string path, FileMode mode, FileAccess access)
    {
        created.OnNext(path);
        return inner.Create(path, mode, access);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share)
    {
        created.OnNext(path);
        return inner.Create(path, mode, access, share);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
    {
        created.OnNext(path);
        return inner.Create(path, mode, access, share, bufferSize);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize,
        FileOptions options)
    {
        created.OnNext(path);
        return inner.Create(path, mode, access, share, bufferSize, options);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
    {
        created.OnNext(path);
        return inner.Create(path, mode, access, share, bufferSize, useAsync);
    }

    public Stream Create(SafeFileHandle handle, FileAccess access)
    {
        return inner.Create(handle, access);
    }

    public Stream Create(SafeFileHandle handle, FileAccess access, int bufferSize)
    {
        return inner.Create(handle, access, bufferSize);
    }

    public Stream Create(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
    {
        return inner.Create(handle, access, bufferSize, isAsync);
    }

    public Stream Create(IntPtr handle, FileAccess access)
    {
        return inner.Create(handle, access);
    }

    public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle)
    {
        return inner.Create(handle, access, ownsHandle);
    }

    public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
    {
        return inner.Create(handle, access, ownsHandle, bufferSize);
    }

    public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
    {
        return inner.Create(handle, access, ownsHandle, bufferSize, isAsync);
    }
}