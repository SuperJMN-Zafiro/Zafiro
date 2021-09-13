using System;
using System.IO;
using System.Threading.Tasks;

namespace Zafiro.Core.Files
{
    public interface IUriBasedStore
    {
        Task<Stream> OpenRead(Uri uri);
        Task<Stream> OpenWrite(Uri uri);
    }
}