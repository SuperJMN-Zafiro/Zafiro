using System.IO.Abstractions.TestingHelpers;

namespace FileSystem.Tests;

public class LinuxPath : MockPath
{
    public LinuxPath(LinuxFs linuxFs) : base(linuxFs)
    {
    }

    public override char DirectorySeparatorChar => '/';
    public override char AltDirectorySeparatorChar => DirectorySeparatorChar;
    public override char VolumeSeparatorChar => DirectorySeparatorChar;
    public override char PathSeparator => DirectorySeparatorChar;

    public override string Combine(params string[] paths)
    {
        return string.Join(DirectorySeparatorChar, paths);
    }

    public override string Combine(string path1, string path2)
    {
        return Combine(new[] {path1, path2});
    }

    public override bool IsPathRooted(string path)
    {
        return path.StartsWith(DirectorySeparatorChar);
    }

    public override string GetPathRoot(string path)
    {
        return DirectorySeparatorChar.ToString();
    }

    public override string GetFullPath(string path)
    {
        return DirectorySeparatorChar + base.GetFullPath(path);
    }

    public override string GetFullPath(string path, string basePath)
    {
        return DirectorySeparatorChar + base.GetFullPath(path, basePath);
    }

    
}