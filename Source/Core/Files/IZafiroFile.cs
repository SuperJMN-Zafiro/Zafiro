using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Files
{
    public interface IZafiroFile
    {
        public Task<Stream> OpenForRead();
        public Task<Stream> OpenForWrite();
        public string Name { get; }
        public Uri Source { get; }
    }
}