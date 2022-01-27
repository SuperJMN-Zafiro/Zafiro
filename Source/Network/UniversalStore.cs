using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Zafiro.Core.Files;

namespace Zafiro.Network
{
    public class UniversalStore : IUriBasedStore
    {
        private readonly IFileSystem fileSystem;
        private readonly IDownloader downloader;

        public UniversalStore(IFileSystem fileSystem, IDownloader downloader)
        {
            this.fileSystem = fileSystem;
            this.downloader = downloader;
        }


        public Task<Stream> OpenRead(Uri uri)
        {
            if (uri.IsFile)
            {
                return Task.FromResult(fileSystem.File.OpenRead(uri.LocalPath));
            }

            return downloader.GetStream(uri);
        }

        public async Task<Stream> OpenWrite(Uri uri)
        {
            if (uri.IsFile)
            {
                return fileSystem.File.Open(uri.LocalPath, FileMode.Truncate);
            }

            throw new NotSupportedException();
        }
    }
}