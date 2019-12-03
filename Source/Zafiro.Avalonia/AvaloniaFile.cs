using System.IO;
using System.Threading.Tasks;
using Zafiro.Core.Files;

namespace Zafiro.Avalonia
{
    public class AvaloniaFile : ZafiroFile
    {
        private readonly string path;

        public AvaloniaFile(string path)
        {
            this.path = path;
        }

        public override Task<Stream> OpenForRead()
        {
            return Task.FromResult<Stream>(File.OpenRead(path));
        }

        public override Task<Stream> OpenForWrite()
        {
            return Task.FromResult<Stream>(File.OpenWrite(path));
        }

        public override string Name => Path.GetFileName(path);
    }
}