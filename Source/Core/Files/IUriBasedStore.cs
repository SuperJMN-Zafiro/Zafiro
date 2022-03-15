using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Files
{
    public interface IUriBasedStore
    {
        Task<Stream> OpenRead(Uri uri);
        Task<Stream> OpenWrite(Uri uri);
    }
}