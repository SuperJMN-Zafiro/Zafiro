using System;
using System.IO;
using System.Threading.Tasks;
using Zafiro.Core.Files;

namespace Zafiro.Avalonia
{
    public class AvaloniaFile : IZafiroFile
    {
        private readonly string path;

        public AvaloniaFile(string path)
        {
            this.path = path;
        }

        public Task<Stream> OpenForRead()
        {
            return Task.FromResult<Stream>(File.OpenRead(path));
        }

        public Task<Stream> OpenForWrite()
        {
            return Task.FromResult<Stream>(File.OpenWrite(path));
        }

        public string Name => Path.GetFileName(path);
        public Uri Source => new Uri(path);
    }
}