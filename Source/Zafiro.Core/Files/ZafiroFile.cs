﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zafiro.Core.FileSystem;

namespace Zafiro.Core.Files
{
    public class ZafiroFile : IZafiroFile
    {
        private readonly Uri uri;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;

        public ZafiroFile(Uri uri, IFileSystemOperations fileSystemOperations, IDownloader downloader)
        {
            this.uri = uri;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
        }

        public Task<Stream> OpenForRead()
        {
            if (uri.IsFile)
            {
                return Task.FromResult(fileSystemOperations.OpenForRead(uri.LocalPath));
            }

            return downloader.GetStream(uri.ToString());
        }

        public async Task<Stream> OpenForWrite()
        {
            if (uri.IsFile)
            {
                await fileSystemOperations.Truncate(uri.LocalPath);
                return fileSystemOperations.OpenForWrite(uri.LocalPath);
            }

            throw new NotSupportedException();
        }

        public string Name => uri.Segments.Last();
        public Uri Source => uri;
    }
}