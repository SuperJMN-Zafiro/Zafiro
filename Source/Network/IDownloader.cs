using System;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.Files;

namespace Network
{
    public interface IDownloader
    {
        Task Download(Uri uri, IZafiroFile destination, IOperationProgress progressObserver = null,
            int timeout = 30);

        Task<Stream> GetStream(Uri uri, IOperationProgress progress = null, int timeout = 30);
    }
}