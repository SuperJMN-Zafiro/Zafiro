using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zafiro.Core.FileSystem
{
    public interface IFileSystemOperations
    {
        Task Copy(string source, string destination, CancellationToken cancellationToken = default);
        Task CopyDirectory(string source, string destination, string fileSearchPattern = null , CancellationToken cancellationToken = default);
        Task DeleteDirectory(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void CreateDirectory(string path);
        void EnsureDirectoryExists(string directoryPath);
        Task DeleteFile(string filePath);
        string GetTempFileName();
        string ReadAllText(string path);
        string WorkingDirectory { get; set; }
        void WriteAllText(string path, string text);
        IEnumerable<string> QueryDirectory(string root, Func<string, bool> selector = null);
        Stream OpenForWrite(string path);
        string GetTempDirectoryName();
    }
}