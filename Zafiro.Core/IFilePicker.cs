using System;
using System.IO;

namespace Zafiro.Core
{
    public interface IFilePicker
    {
        IObservable<Stream> Pick(string title, string[] extensions);
    }
}