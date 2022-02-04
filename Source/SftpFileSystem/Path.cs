using System.IO.Abstractions;

namespace SftpFileSystem;

public class Path : IPath
{
    private const char Separator = '/';
    private readonly FileSystem fileSystem;

    public Path(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public string ChangeExtension(string path, string extension)
    {
        throw new NotImplementedException();
    }

    public string Combine(params string[] paths)
    {
        return string.Join(PathSeparator, paths.Select(s => s == Separator.ToString() ? "" : s));
    }

    public string Combine(string path1, string path2)
    {
        return Combine(new[] {path1, path2});
    }

    public string Combine(string path1, string path2, string path3)
    {
        return Combine(new[] {path1, path2, path3});
    }

    public string Combine(string path1, string path2, string path3, string path4)
    {
        return Combine(new[] {path1, path2, path3, path4});
    }

    public string GetDirectoryName(string path)
    {
        return this.GetParentPath(path);
    }

    public string? GetExtension(string? path)
    {
        if (path is null)
        {
            return null;
        }

        var strings = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var ext = strings.TakeLast(1).FirstOrDefault() ?? "";
        return ext;
    }

    public string? GetFileName(string? path)
    {
        if (path is null)
        {
            return null;
        }

        return GetChunks(path).Last();
    }

    public string GetFileNameWithoutExtension(string path)
    {
        throw new NotImplementedException();
    }

    public string GetFullPath(string path)
    {
        if (IsPathFullyQualified(path))
        {
            return path;
        }

        return Combine(fileSystem.Client.WorkingDirectory, path);
    }

    public string GetFullPath(string path, string basePath)
    {
        throw new NotImplementedException();
    }

    public char[] GetInvalidFileNameChars()
    {
        throw new NotImplementedException();
    }

    public char[] GetInvalidPathChars()
    {
        return Array.Empty<char>();
    }

    public string GetPathRoot(string path)
    {
        return PathSeparator.ToString();
    }

    public string GetRandomFileName()
    {
        throw new NotImplementedException();
    }

    public string GetTempFileName()
    {
        throw new NotImplementedException();
    }

    public string GetTempPath()
    {
        throw new NotImplementedException();
    }

    public bool HasExtension(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsPathRooted(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsPathFullyQualified(string path)
    {
        return path.StartsWith(Separator);
    }

    public string GetRelativePath(string relativeTo, string path)
    {
        throw new NotImplementedException();
        //MoreEnumerable.RightJoin(GetChunks(relativeTo), GetChunks(path), a => a, b => b, (s, a) => s);
    }

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
    {
        throw new NotImplementedException();
    }

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
    {
        throw new NotImplementedException();
    }

    public bool TryJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3,
        Span<char> destination, out int charsWritten)
    {
        throw new NotImplementedException();
    }

    public bool TryJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, Span<char> destination,
        out int charsWritten)
    {
        throw new NotImplementedException();
    }

    public bool HasExtension(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public bool IsPathFullyQualified(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public bool IsPathRooted(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> GetFileNameWithoutExtension(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public string Join(string path1, string path2)
    {
        throw new NotImplementedException();
    }

    public string Join(string path1, string path2, string path3)
    {
        throw new NotImplementedException();
    }

    public string Join(params string[] paths)
    {
        throw new NotImplementedException();
    }

    public bool EndsInDirectorySeparator(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public bool EndsInDirectorySeparator(string path)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path)
    {
        throw new NotImplementedException();
    }

    public string TrimEndingDirectorySeparator(string path)
    {
        throw new NotImplementedException();
    }

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3,
        ReadOnlySpan<char> path4)
    {
        throw new NotImplementedException();
    }

    public string Join(string path1, string path2, string path3, string path4)
    {
        throw new NotImplementedException();
    }

    public char AltDirectorySeparatorChar => Separator;
    public char DirectorySeparatorChar => Separator;
    public IFileSystem FileSystem => fileSystem;
    public char PathSeparator => Separator;
    public char VolumeSeparatorChar => Separator;
    public char[] InvalidPathChars { get; }

    private IEnumerable<string> GetChunks(string relativeTo)
    {
        return relativeTo.Split(PathSeparator);
    }
}