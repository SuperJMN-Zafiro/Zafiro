using System;
using System.IO;
using System.Threading.Tasks;
using Zafiro.Core;
using Zafiro.FileSystem;

namespace Zafiro.Network
{
    public interface IDownloader
    {
        Task Download(Uri uri, IZafiroFile destination, IOperationProgress progressObserver = null,
            int timeout = 30);

        Task<Stream> GetStream(Uri uri, IOperationProgress progress = null, int timeout = 30);
    }
}