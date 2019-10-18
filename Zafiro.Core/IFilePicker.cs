using System;
using System.IO;
using System.Threading.Tasks;

namespace Zafiro.Core
{
    public interface IFilePicker
    {
        IObservable<ZafiroFile> Pick(string title, string[] extensions);
    }

    public abstract class ZafiroFile
    {
        public abstract Func<Task<Stream>> Open();
        public abstract string Name { get; }
    }
}