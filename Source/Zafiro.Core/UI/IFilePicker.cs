using System;
using System.Collections.Generic;
using Zafiro.Core.Files;

namespace Zafiro.Core.UI
{
    public interface IFilePicker
    {
        IObservable<ZafiroFile> Open(string title, IEnumerable<FileTypeFilter> filters);
        IObservable<ZafiroFile> Save(string title, IEnumerable<FileTypeFilter> filters);
    }
}