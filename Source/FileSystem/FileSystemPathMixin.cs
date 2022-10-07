namespace Zafiro.FileSystem;

public static class FileSystemPathMixin
{
    /// <summary>
    /// Translates <paramref name="pathToMap"/> located in <paramref name="origin"/> to its corresponding path in <paramref name="destination"/>
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="pathToMap"></param>
    /// <param name="destination"></param>
    /// <example>
    /// <code>
    /// var origin = new FileSystemPath(oneFs, "c:\source");
    /// var destination = new FileSystemPath(anotherFs, "/home/somedir");
    /// var result = origin.Translate("c:\source\repos\file.txt", destination);
    /// </code>
    /// result will be "/home/somedir/repos/file.txt"
    /// </example>
    /// <returns></returns>
    public static string Translate(this FileSystemPath origin, string pathToMap, FileSystemPath destination)
    {
        var rel = origin.MakeRelative(pathToMap);
        var subDir = destination.FileSystem.DirectoryInfo.FromDirectoryName(destination.Path);
        var newFullPath = destination.FileSystem.Path.Combine(subDir.FullName, rel);
        return newFullPath;
    }

    private static string MakeRelative(this FileSystemPath fileSystemPath, string path)
    {
        return fileSystemPath.FileSystem.Path.GetRelativePath(fileSystemPath.FileSystem.Path.GetFullPath(fileSystemPath.Path), path);
    }
}