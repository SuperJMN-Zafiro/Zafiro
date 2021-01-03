using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Zafiro.Core.FileSystem
{
    public class FileSystemOperations : IFileSystemOperations
    {
        public Task DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                CreateDirectory(directoryPath);
            }
        }

        public Task DeleteFile(string filePath)
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string WorkingDirectory
        {
            get => Environment.CurrentDirectory;
            set
            {
                if (Environment.CurrentDirectory == value)
                {
                    return;
                }

                if (value.Trim().Length == 0)
                {
                    return;
                }

                Environment.CurrentDirectory = value;
            }
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public IEnumerable<string> QueryDirectory(string root, Func<string, bool> selector = null)
        {
            var allDirectories = Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories);

            if (selector != null)
            {
                allDirectories = allDirectories.Where(selector);
            }

            return allDirectories;
        }

        public Stream OpenForWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public async Task Copy(string source, string destination, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.EndsWith(char.ToString(Path.DirectorySeparatorChar)))
            {
                destination = Path.Combine(destination, Path.GetFileName(source));
            }

            var dir = Path.GetDirectoryName(destination);

            if (dir == null)
            {
                throw new InvalidOperationException("The directory name could be retrieved");
            }

            EnsureDirectoryExists(dir);

            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;

            using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,
                fileOptions))
            using (var destinationStream = new FileStream(destination, FileMode.Create, FileAccess.Write,
                FileShare.None, bufferSize, fileOptions))
            {
                await sourceStream.CopyToAsync(destinationStream, bufferSize).ConfigureAwait(false);
            }
        }

        public Task CopyDirectory(string source, string destination, string fileSearchPattern = null,
            CancellationToken cancellationToken = default)
        {
            return CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(destination), fileSearchPattern, false,
                cancellationToken);
        }

        public void CreateDirectory(string destPath)
        {
            if (!IsExistingPath(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        public string GetTempDirectoryName()
        {
            var randomFilename = Path.GetRandomFileName();
            return Path.Combine(Path.GetTempPath(), randomFilename);
        }

        public Stream OpenForRead(string path)
        {
            return File.OpenRead(path);
        }

        public Task Truncate(string path)
        {
            using (new FileStream(path, FileMode.Truncate))
            {
                return Task.CompletedTask;
            }
        }

        private async Task CopyDirectory(DirectoryInfo source, DirectoryInfo destination, string fileSearchPattern,
            bool skipEmptyDirectories, CancellationToken cancellationToken)
        {
            try
            {
                var files = fileSearchPattern == null ? source.GetFiles() : source.GetFiles(fileSearchPattern);

                foreach (var dir in source.GetDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    var subDirFiles = fileSearchPattern == null ? dir.GetFiles() : dir.GetFiles(fileSearchPattern);
                    if (!subDirFiles.Any() && skipEmptyDirectories)
                    {
                        continue;
                    }

                    var subdirectory = destination.CreateSubdirectory(dir.Name);
                    await CopyDirectory(dir, subdirectory, fileSearchPattern, skipEmptyDirectories, cancellationToken);
                }

                foreach (var file in files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    var destFileName = Path.Combine(destination.FullName, file.Name);
                    await Copy(file.FullName, destFileName, cancellationToken);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Log.Warning(e, "Unauthorized folder {Folder}", source);
            }
        }

        private static bool IsExistingPath(string path)
        {
            var isExistingPath = File.Exists(path) || Directory.Exists(path);
            var status = isExistingPath ? "exists" : "does not exist";

            Log.Verbose($"Checking path: {{Path}} {status}", path);

            return isExistingPath;
        }
    }
}