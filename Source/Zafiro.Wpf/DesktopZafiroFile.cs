using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;

namespace Zafiro.Wpf
{
    public class DesktopZafiroFile : ZafiroFile
    {
        private readonly Uri uri;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;

        public DesktopZafiroFile(Uri uri, IFileSystemOperations fileSystemOperations, IDownloader downloader)
        {
            this.uri = uri;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
        }

        public override Task<Stream> OpenForRead()
        {
            if (uri.IsFile)
            {
                return Task.FromResult(fileSystemOperations.OpenForRead(uri.LocalPath));
            }

            return downloader.GetStream(uri.ToString());
        }

        public override Task<Stream> OpenForWrite()
        {
            if (uri.IsFile)
            {
                return Task.FromResult(fileSystemOperations.OpenForWrite(uri.LocalPath));
            }

            throw new NotSupportedException();
        }

        public override string Name => uri.Segments.Last();
        public override Uri Source => uri;
    }
}