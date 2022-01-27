using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Zafiro.Core.Files
{
    public class ZafiroFile : IZafiroFile
    {
        private readonly Uri uri;
        private readonly IUriBasedStore store;

        public ZafiroFile(Uri uri, IUriBasedStore store)
        {
            this.uri = uri;
            this.store = store;
        }

        public Task<Stream> OpenForRead()
        {
            return store.OpenRead(uri);
        }

        public Task<Stream> OpenForWrite()
        {
            return store.OpenWrite(uri);
        }

        public string Name => uri.Segments.Last();
        public Uri Source => uri;
    }
}