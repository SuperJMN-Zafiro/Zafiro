using System.IO.Abstractions;
using Microsoft.Win32.SafeHandles;

namespace FileSystem;

public class CachingFileStreamFactory : IFileStreamFactory
{
    private readonly IFileStreamFactory inner;

    public CachingFileStreamFactory(IFileStreamFactory inner)
    {
        this.inner = inner;
    }

    public Stream Create(string path, FileMode mode)
    {
        return inner.Create(path, mode);
    }

    public Stream Create(string path, FileMode mode, FileAccess access)
    {
        return inner.Create(path, mode, access);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share)
    {
        return inner.Create(path, mode, access, share);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
    {
        return inner.Create(path, mode, access, share, bufferSize);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize,
        FileOptions options)
    {
        return inner.Create(path, mode, access, share, bufferSize, options);
    }

    public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
    {
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