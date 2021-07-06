using System.IO;
using System.Threading.Tasks;
using Zafiro.Core.Files;

namespace Zafiro.Core
{
    public interface IDownloader
    {
        Task Download(string url, IZafiroFile destination, IOperationProgress progressObserver = null,
            int timeout = 30);

        Task<Stream> GetStream(string url, IOperationProgress progress = null, int timeout = 30);
    }
}